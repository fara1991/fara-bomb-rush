# FaraBombRush

Beat Saber mod that spawns bombs during gameplay. Supports Twitch chat commands for interactive play and an auto mode that generates bomb patterns synced to the song's BPM.

## Requirements

- Beat Saber 1.29.1 or 1.40.8 (build for each version, see [Building](#building))
- Twitch chat is read via ChatPlexSDK (BeatSaberPlus must be installed in both 1.29.1 and 1.40.8)
- [BSIPA](https://github.com/bsmg/BeatSaber-IPA-Reloaded) >= 4.2.0

## Installation

1. Download the latest release from [Releases](https://github.com/fara1991/FaraBombRush/releases)
2. Extract the zip file
3. Copy `FaraBombRush.dll` to `Beat Saber/Plugins/`
4. Copy `manifest.json` to `Beat Saber/Plugins/FaraBombRush/`

## Game Modes

| Mode | Description |
|------|-------------|
| **None** | Disabled |
| **Interactive** | Twitch chat viewers spawn bombs via commands |
| **Auto** | Bombs are generated automatically, synced to the song's BPM |
| **Battle** | Battle mode |

## Twitch Chat Commands (Interactive Mode)

| Command | Description |
|---------|-------------|
| `!bomb` | Spawn a single bomb at a random position |
| `!bomb <1-12>` | Spawn a single bomb at a specific position |
| `!bombline` | Spawn a line of bombs at a random position |
| `!bombline <1-12>` | Spawn a line of bombs at a specific position |
| `!bombreset` | Spawn a reset pattern of bombs |
| `!bombreset <1-12>` | Spawn a reset pattern at a specific position |

## Settings

Settings are available in the in-game Mod Settings menu.

- **Play Mode** - Select the game mode
- **Player Bomb Level** - Difficulty level for auto mode (Beginner / Easy / Normal / Hard / Expert / ExpertPlus / Lawless)
- **Valid Bomb Collision** - Enable/disable bomb hit detection
- **One Shot Bomb Line Count** - Number of bombs in a line (5-10)

## Building

### Prerequisites

- .NET Framework 4.8.1 SDK
- Beat Saber installed (for reference DLLs)

### Build

Build with MSBuild (Visual Studio). Pick the Beat Saber version with the `GameVersion` property; the references are taken from `%USERPROFILE%\BSManager\BSInstances\<GameVersion>`.

```bash
# Beat Saber 1.29.1 (default)
msbuild FaraBombRush/FaraBombRush.csproj -restore -p:Configuration=Release

# Beat Saber 1.40.8
msbuild FaraBombRush/FaraBombRush.csproj -restore -p:Configuration=Release -p:GameVersion=1.40.8
```

- Output: `FaraBombRush/bin/<Configuration>/<GameVersion>/FaraBombRush.dll`
- The DLL is copied to that version's `Plugins` folder by BeatSaberModdingTools (add `-p:DisableCopyToGame=True` to skip). `Assets/farabomb.particle` is copied to its `UserData/FaraBombRush` if it is not there yet.
- `manifest.json` is embedded for 1.29.1 and `manifest.1.40.8.json` for 1.40.8 (different `gameVersion`).
- Code that differs between versions is wrapped in `#if GAME_1_40_8` / `#else`.

## License

Copyright (c) Fara 2024
