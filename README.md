# Fortnite Replay Parser

A .NET console application that scans a directory of Fortnite replay files, extracts match and player statistics, stores the data in a database using Entity Framework Core, and archives processed replay files.

## Features

- Parses `.replay` files using Fortnite replay reader libraries.
- Stores match information in a database.
- Stores individual player results for each match.
- Automatically updates existing matches when a replay is reprocessed.
- Ignores bot players.
- Archives processed replay files into a separate folder.
- Prevents duplicate matches by using the Fortnite session ID.

## How It Works

1. The application scans the `replays/` directory for `.replay` files.
2. Each replay is parsed and the match session ID is extracted.
3. The database is checked for an existing record:
   - If the match does not exist, a new record is created.
   - If the match already exists, existing player results are removed and refreshed.
4. Player statistics are extracted and saved.
5. The replay file is moved to `replays/processed/` with a unique timestamped filename.

## Folder Structure

```text
replays/
├── match1.replay
├── match2.replay
└── processed/
    ├── 20260706-2100-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.replay
    └── ...
```

## Requirements

- .NET 8.0 or later
- Microsoft SQL Server (or compatible SQL Server instance)
- Database configured through `Database.ConnectToDatabase()`
- Fortnite replay parsing libraries:
  - FortniteReplayReader
  - FortniteReplayParser

## Quick Start

1. Clone the repository.
2. Rename `appsettings.json.rename` to `appsettings.json`.
3. Update the database connection string.
4. Create the database if it does not already exist.
5. Place `.replay` files into the `replays/` folder.
6. Run:

```bash
dotnet run
```

## Configuration

Before running the application, rename:

```text
appsettings.json.rename
```

to:

```text
appsettings.json
```

Then update the database connection settings inside `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FortniteReplayParser;User Id=username;Password=password;TrustServerCertificate=True;"
  }
}
```

### Example SQL Server Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=192.168.1.100;Database=FortniteReplayParser;User Id=sa;Password=YourStrongPasswordHere;TrustServerCertificate=True;"
  }
}
```

### Example LocalDB Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=FortniteReplayParser;Trusted_Connection=True;"
  }
}
```

After updating the connection string, ensure the target database exists and that the configured account has permission to read and write data.

## Database Records

### FortniteReplay

Stores match-level information:

| Field | Description |
|---------|-------------|
| SessionId | Fortnite match session identifier |
| OriginalReplayFilename | Original replay filename |
| ProcesssedReplayFilename | Archived replay filename |
| MatchStartTime | Match start timestamp |
| TotalTeams | Number of teams in the match |
| TeamSize | Team size for the match |
| CreatedAt | Record creation timestamp |
| UpdatedAt | Last update timestamp |

### FortniteReplayResult

Stores player-level information:

| Field | Description |
|---------|-------------|
| EpicId | Player Epic Games ID |
| PlayerName | Player display name |
| Placement | Final placement |
| Eliminations | Total eliminations |
| TeamIndex | Team identifier |
| CreatedAt | Record creation timestamp |
| UpdatedAt | Last update timestamp |

## Running

Build the application:

```bash
dotnet build
```

Run the application:

```bash
dotnet run
```

## Replay Processing Logic

The application attempts to identify matches using:

1. `GameData.GameSessionId`
2. `Header.Guid` (fallback when the session ID is unavailable)

This ensures that older or incomplete replay files can still be uniquely identified.

When a replay is processed:

- A match record is created if one does not already exist.
- Existing match records are updated when the same session is reprocessed.
- Existing player results are removed and rebuilt from the replay data.
- Bot players are skipped.
- The original replay file is moved to the `processed` folder after successful processing.

## Notes

- Bot players are excluded from stored results.
- Existing match results are fully replaced when a replay is reprocessed.
- Processed replay files are renamed using a timestamp and GUID to prevent filename collisions.
- Failed file moves are logged but do not stop processing of subsequent replay files.
- Match start times are converted to the configured local timezone when available.