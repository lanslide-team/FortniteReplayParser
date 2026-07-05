using FortniteReplayReader;
using FortniteReplayParser.Data;
using FortniteReplayParser;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;


async Task main()
{
    string folderPath = @"replays";

    if (Directory.Exists(folderPath))
    {
        string[] replayFiles = Directory.GetFiles(folderPath, "*.replay");
        await ProcessFiles(replayFiles, folderPath);
    }
}

async Task ProcessFiles(string[] replayFiles, string folderPath)
{
    var serviceProvider = Database.ConnectToDatabase();
    using var scope = serviceProvider.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    foreach (string replayFile in replayFiles)
    {
        var reader = new ReplayReader();
        var replay = reader.ReadReplay(replayFile);
        var sessionId = replay.GameData.GameSessionId;
        if (sessionId is null) {
            sessionId = replay.Header.Guid;
        }

        var fortniteReplay = await db.Set<FortniteReplay>()
            .Include(r => r.Results)
            .SingleOrDefaultAsync(r => r.SessionId == sessionId);

        string targetPath = $"{folderPath}/processed";
        string uniqueId = Guid.NewGuid().ToString("N");
        string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmm");
        string targetFilename = $"{timestamp}-{uniqueId}.replay";

        if (fortniteReplay is null)
        {
            fortniteReplay = new FortniteReplay
            {
                OriginalReplayFilename = Path.GetFileName(replayFile),
                ProcesssedReplayFilename = targetFilename,
                SessionId = sessionId,
                MatchStartTime = replay.GameData.UtcTimeStartedMatch is null ? replay.Info.Timestamp : Database.ConvertToLocalTimezone(replay.GameData.UtcTimeStartedMatch.Value),
                TotalTeams = replay.GameData.TotalTeams,
                TeamSize = replay.GameData.TeamSize is null ? replay.TeamData.Count() : replay.GameData.TeamSize,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            db.Set<FortniteReplay>().Add(fortniteReplay);
        }
        else
        {
            fortniteReplay.OriginalReplayFilename = Path.GetFileName(replayFile);
            fortniteReplay.ProcesssedReplayFilename = targetFilename;
            fortniteReplay.SessionId = sessionId;
            fortniteReplay.MatchStartTime = replay.GameData.UtcTimeStartedMatch is null ? null : Database.ConvertToLocalTimezone(replay.GameData.UtcTimeStartedMatch.Value);
            fortniteReplay.TotalTeams = replay.GameData.TotalTeams;
            fortniteReplay.TeamSize = replay.GameData.TeamSize;
            fortniteReplay.UpdatedAt = DateTime.Now;

            db.RemoveRange(fortniteReplay.Results);
            fortniteReplay.Results.Clear();
        }

        foreach (var player in replay.PlayerData)
        {
            if (player.IsBot) continue;
            fortniteReplay.Results.Add(new FortniteReplayResult
            {
                EpicId                  = player.EpicId,
                PlayerName              = player.PlayerName,
                Placement               = player.Placement,
                Eliminations            = player.Kills ?? 0,
                TeamIndex               = player.TeamIndex,
                CreatedAt               = DateTime.Now,
                UpdatedAt               = DateTime.Now,
            });
        }

        await db.SaveChangesAsync();

        Console.WriteLine($"Replay inserted! Id={fortniteReplay.Id}");

        try
        {
            Directory.CreateDirectory(targetPath);
            File.Move(replayFile, $"{targetPath}/{targetFilename}", overwrite: true);
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

await main();