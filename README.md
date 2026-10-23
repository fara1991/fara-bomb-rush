# FaraBombRush

Beat Saber mod that spawns bombs during gameplay. Supports Twitch chat commands for interactive play and an auto mode that generates bomb patterns synced to the song's BPM.

## Requirements

- Beat Saber 1.29.1 or 1.40.8 (build for each version, see [Building](#building))
- BeatSaberPlus (provides ChatPlexSDK) is a soft dependency: Twitch chat commands need it. Without it, chat is disabled (expected; not verified in game). Install it for both 1.29.1 and 1.40.8.
- [BSIPA](https://github.com/bsmg/BeatSaber-IPA-Reloaded) >= 4.2.0

## Installation

1. Download the zip for your Beat Saber version from [Releases](https://github.com/fara1991/fara-bomb-rush/releases): `FaraBombRush-v<version>-BS1.29.1.zip` or `FaraBombRush-v<version>-BS1.40.8.zip`
2. Extract it into the Beat Saber folder. It contains `Plugins/FaraBombRush.dll` and `UserData/FaraBombRush/farabomb.particle` (the bomb asset the mod loads at runtime).

Releases up to 1.0.0 were laid out differently; follow the notes on each release page.

## Game Modes

The mod is enabled or disabled with the **Enabled** setting. The mode is chosen with **Play Mode**.

| Mode | Description |
|------|-------------|
| **None** | Does nothing: no bombs are spawned and chat is not read. |
| **Interactive** | Twitch chat viewers spawn bombs via commands (the default for new installs) |
| **Auto** | Bombs are generated automatically, synced to the song's BPM |

Battle is not implemented and is not offered. If an old setting still says `Battle`, it is treated like **None** (and shown as None in the settings screen).

Updating from 1.0.0: the 1.0.0 default was **None**, and a saved **None** is kept (the mod stays off until you pick another mode). New installs from 1.1.0 start with **Interactive**.

## Twitch Chat Commands (Interactive Mode)

| Command | Description |
|---------|-------------|
| `!bomb` | Spawn a single bomb at a random position |
| `!bomb <1-12>` | Spawn a single bomb at a specific position |
| `!bombline` | Spawn a line of 5 bombs at a random position |
| `!bombline <1-12>` | Spawn a line of 5 bombs at a specific position |
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
- `!bombline` spawns 5 bombs at the same position, 1 m apart in the lane (almost touching), regardless of BPM. Auto mode lines use the same spacing.
- `!bombreset` spawns 4 bombs at the same time across one row. Position 5 (center left) and the top positions reset the **top row**. Position 8 (center right) and the bottom positions reset the **bottom row**. For 6, 7 or no number, the row depends on the random position (top 5 of 10 positions, bottom 5 of 10).
- Numbers outside 1 to 12 (for example `0` or `13`) and anything that is not a number do not spawn a bomb. `!bombs` and `!bomberman` are not commands. Only ASCII digits count (a full-width `５` is rejected); a leading zero (`05`) is fine.
- A Twitch reply works: one leading `@name` is skipped (`@name !bomb 5`). Control characters and invisible format characters (for example the ones 7TV adds) are removed before the check, and any run of spaces is fine.

## Settings

Open the **Fara Bomb Rush** button in the main menu to change these settings.

- **Language** - English / Japanese. Changes the settings screen and the menu button text. Defaults to English; unknown values are treated as English.
- **Enabled** - Enable or disable the mod. The menu bomb preview follows this setting.
- **Play Mode** - Game mode (None / Interactive / Auto). The choice is saved. None does nothing (no bombs, chat is not read).
- **Player Bomb Level** - Difficulty level for auto mode (Beginner / Easy / Normal / Hard / Expert / ExpertPlus / Lawless)
- **Valid Bomb Collision** - Enable/disable bomb hit detection

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

`dotnet build FaraBombRush/FaraBombRush.csproj -c Release -p:GameVersion=1.40.8` works the same way.

- Output: `FaraBombRush/bin/<Configuration>/<GameVersion>/FaraBombRush.dll`
- The DLL is copied to that version's `Plugins` folder by BeatSaberModdingTools (add `-p:DisableCopyToGame=True` to skip). `Assets/farabomb.particle` is copied to its `UserData/FaraBombRush` if it is not there yet.
- `manifest.json` is embedded for 1.29.1 and `manifest.1.40.8.json` for 1.40.8 (different `gameVersion`).
- Code that differs between versions is wrapped in `#if GAME_1_40_8` / `#else`.

## Releasing

Releases are built on a local machine, not on GitHub Actions: the build needs the Beat Saber reference DLLs, which cannot be put on a CI runner. One run builds both 1.29.1 and 1.40.8 and attaches both zips to a single release tagged `v<version>` (for example `v1.1.0`).

### Prerequisites

- .NET SDK (`dotnet`), Windows PowerShell 5.1 or later (the default execution policy blocks scripts, so run it with `-ExecutionPolicy Bypass` as below)
- Reference DLLs for both versions in `%USERPROFILE%\BSManager\BSInstances\1.29.1` and `%USERPROFILE%\BSManager\BSInstances\1.40.8` (install them with BSManager)
- For publishing: [GitHub CLI](https://cli.github.com/) (`gh auth login`) and push access to this repository

### Steps

1. Set the same version in `FaraBombRush/manifest.json`, `FaraBombRush/manifest.1.40.8.json` and `FaraBombRush/Properties/AssemblyInfo.cs` (`AssemblyVersion` / `AssemblyFileVersion`), and merge it to `master` through `develop`.
2. On `master`, dry run first. It runs the unit tests (`FaraBombRush.Tests`, no game DLLs needed; the script stops if one fails), deletes `bin\Release\<game version>`, builds both versions, checks that the versions match, makes the zips under `dist\` and lists their contents. Nothing is tagged or published. If there is no previous tag to compare with, it warns and lists the last 30 commits in the notes instead.

   ```powershell
   powershell -ExecutionPolicy Bypass -File .\tools\release.ps1 1.1.0
   ```

3. Publish. This runs `gh release create v1.1.0 --target <commit>` with both zips and generated release notes (commits since the previous tag). GitHub creates the tag `v1.1.0` at that commit together with the release, so nothing is pushed beforehand and a failed run can simply be repeated. If it fails, the script prints the commands to remove a leftover release or tag.

   ```powershell
   powershell -ExecutionPolicy Bypass -File .\tools\release.ps1 1.1.0 -Publish
   ```

   `-Publish` stops if the working tree is dirty, the branch is not `master` (override with `-AllowNonMaster`), local `master` differs from `origin/master`, the commit is not on `origin`, or the tag or a release for it already exists.

Output: `dist\FaraBombRush-v1.1.0-BS1.29.1.zip`, `dist\FaraBombRush-v1.1.0-BS1.40.8.zip` and `dist\release-notes-v1.1.0.md`. Each zip has `Plugins/FaraBombRush.dll`, `Plugins/FaraBombRush.pdb` and `UserData/FaraBombRush/farabomb.particle`. The script builds with `-p:DisableCopyToGame=True`, so it does not touch your installed games.

## License

Copyright (c) Fara 2024

## Tests

The pure logic that does not need the game (chat command parser, bomb line spacing, ScoreSaber PP parsing) is tested without game DLLs:

```
dotnet test FaraBombRush.Tests/FaraBombRush.Tests.csproj
```

If the PP shown in the settings screen cannot be loaded, the hint says so, and the reason (HTTP status, exception) is written to the IPA log.
