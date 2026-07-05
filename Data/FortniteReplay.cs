using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FortniteReplayParser.Data;

[Table("fortnite_replays")]
public class FortniteReplay
{
    [Key]
    [Column("id", TypeName = "char(36)")]
    public Guid Id { get; set; }

    [Column("original_replay_filename")]
    public string? OriginalReplayFilename { get; set; }

    [Column("processed_replay_filename")]
    public string? ProcesssedReplayFilename { get; set; }

    [Column("session_id")]
    public string? SessionId { get; set; }

    [Column("match_start_time")]
    public DateTime? MatchStartTime { get; set; }

    [Column("total_teams")]
    public int? TotalTeams { get; set; }

    [Column("team_size")]
    public int? TeamSize { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    public ICollection<FortniteReplayResult> Results { get; set; } = new List<FortniteReplayResult>();
}
