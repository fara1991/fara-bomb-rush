# FaraBombRush

Beat Saber mod that spawns bombs during gameplay. Supports Twitch chat commands for interactive play and an auto mode that generates bomb patterns synced to the song's BPM.

## Requirements

- Beat Saber 1.29.1
- [BSIPA](https://github.com/bsmg/BeatSaber-IPA-Reloaded) >= 4.2.0

## Installation

1. Download the latest release from [Releases](https://github.com/fara1991/FaraBombRush/releases)
2. Extract the zip file
3. Copy `FaraBombRush.dll` to `Beat Saber/Plugins/`
4. Copy `manifest.json` to `Beat Saber/Plugins/FaraBombRush/`
5. Copy `FaraBombRush/Assets/farabomb.particle` from this repository to `Beat Saber/UserData/FaraBombRush/`. The release zip does not include it, and the mod loads it from that folder.

## Game Modes

| Mode | Description |
|------|-------------|
| **None** | Disabled |
| **Interactive** | Twitch chat viewers spawn bombs via commands |
| **Auto** | Bombs are generated automatically, synced to the song's BPM |
| **Battle** | Not implemented yet. Selecting it starts no game mode controller. |

## Twitch Chat Commands (Interactive Mode)

| Command | Description |
|---------|-------------|
| `!bomb` | Spawn a single bomb at a random position |
| `!bomb <1-10>` | Spawn a single bomb at a specific position |
| `!bombline` | Spawn a line of bombs at a random position |
| `!bombline <1-10>` | Spawn a line of bombs at a specific position |
| `!bombreset` | Spawn a reset pattern of bombs |
| `!bombreset <1-10>` | Spawn a reset pattern at a specific position |

Position numbers (`<1-10>`):

| Number | Position |
|--------|----------|
| 1 | Top left |
| 2 | Top middle left |
| 3 | Top middle right |
| 4 | Top right |
| 5 | Center left |
| 6 | Center right |
| 7 | Bottom left |
| 8 | Bottom middle left |
| 9 | Bottom middle right |
| 10 | Bottom right |

- Without a number, the position is picked at random from 1 to 10.
- `!bombline` spawns the number of bombs set by **One Shot Bomb Line Count** (default 5) at the same position, one beat apart.
- `!bombreset` spawns 4 bombs at the same time across one row. Numbers 1 to 5 use the top row, and numbers 6 to 10 use the bottom row.
- Numbers outside 1 to 10 (and anything that is not a number) do not spawn a bomb.

## Settings

Open the **Fara Bomb Rush** button in the main menu to change these settings.

- **Play Mode** - Select the game mode (None / Interactive / Auto / Battle)
- **Player Bomb Level** - Difficulty level for auto mode (Beginner / Easy / Normal / Hard / Expert / ExpertPlus / Lawless)
- **Valid Bomb Collision** - Enable/disable bomb hit detection
- **One Shot Bomb Line Count** - Number of bombs in a line, 5 to 10 (default 5). Used by `!bombline` and by the line patterns in auto mode.

## Building

### Prerequisites

- .NET Framework 4.8.1 SDK
- Beat Saber installed (for reference DLLs)

### Build

```bash
dotnet build -c Release
```

- Output: `FaraBombRush/bin/Release/FaraBombRush.dll`
- The reference DLLs are read from the `GameDirectory` property in `FaraBombRush/FaraBombRush.csproj` (default `C:\Program Files (x86)\Steam\steamapps\common\Beat Saber`). Change it, or pass `-p:GameDirectory="<path>"`, if Beat Saber is installed somewhere else.

## License

Copyright (c) Fara 2024
