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
| `!bomb <1-12>` | Spawn a single bomb at a specific position |
| `!bombline` | Spawn a line of bombs at a random position |
| `!bombline <1-12>` | Spawn a line of bombs at a specific position |
| `!bombreset` | Spawn a reset pattern of bombs at a random position |
| `!bombreset <1-12>` | Spawn a reset pattern at a specific position |

Position numbers (`<1-12>`):

| Number | Position |
|--------|----------|
| 1 to 4 | Top row, from the left (top left, top middle left, top middle right, top right) |
| 5 | Center left |
| 6, 7 | No fixed position (between the note positions). A random position is used. |
| 8 | Center right |
| 9 to 12 | Bottom row, from the left (bottom left, bottom middle left, bottom middle right, bottom right) |

- Without a number, the position is picked at random, uniformly from the 10 positions above.
- `!bombline` spawns the number of bombs set by **One Shot Bomb Line Count** (default 5) at the same position, one beat apart.
- `!bombreset` spawns 4 bombs at the same time across one row. Position 5 and the top positions reset the **top row**. Position 8 and the bottom positions reset the **bottom row**. For 6, 7 or no number, the row depends on the random position.
- Numbers outside 1 to 12 (for example `0` or `13`) and anything that is not a number do not spawn a bomb.

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
