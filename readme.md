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

## Folder Structure

```text
replays/
├── match1.replay
├── match2.replay
└── processed/
    ├── 20260706-2100-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.replay
    └── ...