<#
.SYNOPSIS
  Builds FaraBombRush for Beat Saber 1.29.1 and 1.40.8 in one go and makes one release zip per game version.

.DESCRIPTION
  Default is a dry run: build, check the manifests, make the zips under dist\, list their contents.
  Nothing is tagged, pushed or published unless -Publish is given.

  Needs the reference DLLs of each game version in %USERPROFILE%\BSManager\BSInstances\<GameVersion>
  (so this cannot run on GitHub Actions).

.PARAMETER Version
  Release version without "v", e.g. 1.1.0. The git tag is v<Version>.

.PARAMETER Publish
  After a successful build: create tag v<Version>, push it, and run "gh release create"
  with both zips attached. Requires a clean working tree on master (see -AllowNonMaster).

.PARAMETER AllowNonMaster
  Allow -Publish from a branch other than master (not recommended).

.EXAMPLE
  powershell -ExecutionPolicy Bypass -File .\tools\release.ps1 1.1.0            # dry run
  powershell -ExecutionPolicy Bypass -File .\tools\release.ps1 1.1.0 -Publish   # tag v1.1.0 + GitHub release
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version,
    [switch]$Publish,
    [switch]$AllowNonMaster
)

$ErrorActionPreference = 'Stop'

# git prints UTF-8 (commit subjects are Japanese); make PowerShell decode it as UTF-8.
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch { }

$GameVersions = @('1.29.1', '1.40.8')
$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$ProjDir = Join-Path $RepoRoot 'FaraBombRush'
$Csproj = Join-Path $ProjDir 'FaraBombRush.csproj'
$DistDir = Join-Path $RepoRoot 'dist'
$Tag = "v$Version"
$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)

function Write-Step([string]$Text) { Write-Host ''; Write-Host "== $Text" -ForegroundColor Cyan }

function Invoke-Native([string]$Exe, [string[]]$Arguments) {
    & $Exe @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "$Exe $($Arguments -join ' ') failed (exit code $LASTEXITCODE)"
    }
}

function Get-GitOutput([string[]]$Arguments) {
    $out = & git -C $RepoRoot -c i18n.logOutputEncoding=UTF-8 @Arguments
    if ($LASTEXITCODE -ne 0) { throw "git $($Arguments -join ' ') failed (exit code $LASTEXITCODE)" }
    return $out
}

function Get-ManifestPath([string]$GameVersion) {
    if ($GameVersion -eq '1.40.8') { return Join-Path $ProjDir 'manifest.1.40.8.json' }
    return Join-Path $ProjDir 'manifest.json'
}

# ---------------------------------------------------------------- (b) check manifests
Write-Step "Check versions (expected $Version)"
$errors = @()
foreach ($gv in $GameVersions) {
    $path = Get-ManifestPath $gv
    $m = (Get-Content -LiteralPath $path -Raw -Encoding UTF8) | ConvertFrom-Json
    $name = Split-Path $path -Leaf
    Write-Host ("  {0,-22} version={1} gameVersion={2}" -f $name, $m.version, $m.gameVersion)
    if ($m.version -ne $Version) { $errors += "$name version is '$($m.version)' (expected '$Version')" }
    if ($m.gameVersion -ne $gv) { $errors += "$name gameVersion is '$($m.gameVersion)' (expected '$gv')" }
}
$asmInfo = Get-Content -LiteralPath (Join-Path $ProjDir 'Properties\AssemblyInfo.cs') -Raw
foreach ($attr in @('AssemblyVersion', 'AssemblyFileVersion')) {
    $mt = [regex]::Match($asmInfo, "(?m)^\s*\[assembly:\s*$attr\(""([^""]+)""\)\]")
    $val = if ($mt.Success) { $mt.Groups[1].Value } else { '(not found)' }
    Write-Host ("  {0,-22} {1}" -f $attr, $val)
    if ($val -ne $Version) { $errors += "AssemblyInfo.cs $attr is '$val' (expected '$Version')" }
}
if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Host "  NG: $_" -ForegroundColor Red }
    throw 'Version mismatch. Fix the files above, then run again.'
}

# ---------------------------------------------------------------- prerequisites
Write-Step 'Check prerequisites'
foreach ($gv in $GameVersions) {
    $gameDir = Join-Path $env:USERPROFILE "BSManager\BSInstances\$gv"
    $probe = Join-Path $gameDir 'Beat Saber_Data\Managed\Main.dll'
    if (-not (Test-Path -LiteralPath $probe)) {
        throw "Reference DLLs for $gv not found: $probe (install Beat Saber $gv with BSManager)"
    }
    Write-Host "  OK  $gameDir"
}
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'dotnet is not on PATH.' }

if ($Publish) {
    if (-not (Get-Command gh -ErrorAction SilentlyContinue)) { throw 'gh (GitHub CLI) is not on PATH.' }
    Invoke-Native 'gh' @('auth', 'status')
    $dirty = Get-GitOutput @('status', '--porcelain')
    if ($dirty) { throw "Working tree is not clean (needed for -Publish):`n$($dirty -join "`n")" }
    $branch = (Get-GitOutput @('rev-parse', '--abbrev-ref', 'HEAD')) | Select-Object -First 1
    if ($branch -ne 'master' -and -not $AllowNonMaster) {
        throw "Current branch is '$branch'. Release from master (merge develop into master first), or pass -AllowNonMaster."
    }
    Invoke-Native 'git' @('-C', $RepoRoot, 'fetch', 'origin', '--tags', '--quiet')
    if (Get-GitOutput @('tag', '--list', $Tag)) { throw "Tag $Tag already exists locally." }
    if (Get-GitOutput @('ls-remote', '--tags', 'origin', "refs/tags/$Tag")) { throw "Tag $Tag already exists on origin." }
    if ($branch -eq 'master') {
        $local = (Get-GitOutput @('rev-parse', 'HEAD')) | Select-Object -First 1
        $remote = (Get-GitOutput @('rev-parse', 'origin/master')) | Select-Object -First 1
        if ($local -ne $remote) { throw 'Local master differs from origin/master. Pull or push first.' }
    }
}

# ---------------------------------------------------------------- (a) build both game versions
$outputs = @{}
foreach ($gv in $GameVersions) {
    Write-Step "Build Release for Beat Saber $gv"
    # DisableCopyToGame: do not touch the Plugins / UserData folders of the installed games.
    Invoke-Native 'dotnet' @('build', $Csproj, '-c', 'Release', "-p:GameVersion=$gv", '-p:DisableCopyToGame=True', '-restore', '-v:minimal', '-nologo')
    $dll = Join-Path $ProjDir "bin\Release\$gv\FaraBombRush.dll"
    if (-not (Test-Path -LiteralPath $dll)) { throw "Build output not found: $dll" }
    $outputs[$gv] = $dll
}

# ---------------------------------------------------------------- embedded manifest check + (c) zips
if (Test-Path -LiteralPath $DistDir) { Remove-Item -LiteralPath $DistDir -Recurse -Force }
New-Item -ItemType Directory -Path $DistDir | Out-Null
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$particle = Join-Path $ProjDir 'Assets\farabomb.particle'
if (-not (Test-Path -LiteralPath $particle)) { throw "Missing asset: $particle" }

function Add-ZipEntry($Zip, [string]$SourceFile, [string]$EntryName) {
    # Entry names always use "/" (Compress-Archive in Windows PowerShell 5.1 would write "\").
    [void][System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($Zip, $SourceFile, $EntryName, [System.IO.Compression.CompressionLevel]::Optimal)
}

$zips = @()
foreach ($gv in $GameVersions) {
    Write-Step "Make zip for Beat Saber $gv"
    $dll = $outputs[$gv]

    # The manifest is embedded in the DLL; make sure the DLL really carries the expected version.
    $asm = [System.Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($dll))
    $stream = $asm.GetManifestResourceStream('FaraBombRush.manifest.json')
    if ($null -eq $stream) { throw "Embedded manifest not found in $dll" }
    $reader = New-Object System.IO.StreamReader($stream, [System.Text.Encoding]::UTF8)
    $embedded = $reader.ReadToEnd() | ConvertFrom-Json
    $reader.Dispose()
    if ($embedded.version -ne $Version -or $embedded.gameVersion -ne $gv) {
        throw "Embedded manifest in $dll is $($embedded.version) / $($embedded.gameVersion) (expected $Version / $gv)"
    }
    Write-Host "  embedded manifest OK ($($embedded.version) / $($embedded.gameVersion))"

    $zipPath = Join-Path $DistDir "FaraBombRush-$Tag-BS$gv.zip"
    $zip = [System.IO.Compression.ZipFile]::Open($zipPath, [System.IO.Compression.ZipArchiveMode]::Create)
    try {
        # Same layout as the 1.0.0 release zip: unzip into the Beat Saber folder.
        Add-ZipEntry $zip $dll 'Plugins/FaraBombRush.dll'
        $pdb = [System.IO.Path]::ChangeExtension($dll, '.pdb')
        if (Test-Path -LiteralPath $pdb) { Add-ZipEntry $zip $pdb 'Plugins/FaraBombRush.pdb' }
        Add-ZipEntry $zip $particle 'UserData/FaraBombRush/farabomb.particle'
    }
    finally { $zip.Dispose() }
    $zips += $zipPath

    $zr = [System.IO.Compression.ZipFile]::OpenRead($zipPath)
    try {
        Write-Host "  $zipPath"
        foreach ($e in $zr.Entries) { Write-Host ("    {0,10:N0}  {1}" -f $e.Length, $e.FullName) }
    }
    finally { $zr.Dispose() }
}

# ---------------------------------------------------------------- release notes
Write-Step 'Release notes'
$prevTag = $null
try {
    $described = & git -C $RepoRoot describe --tags --abbrev=0 2>$null
    if ($LASTEXITCODE -eq 0 -and $described) { $prevTag = ($described | Select-Object -First 1) }
}
catch { $prevTag = $null }
if ($prevTag) {
    $subjects = Get-GitOutput @('log', "$prevTag..HEAD", '--no-merges', '--pretty=format:- %s')
    $range = "Changes since $prevTag"
}
else {
    $subjects = Get-GitOutput @('log', '--no-merges', '--max-count=30', '--pretty=format:- %s')
    $range = 'Recent changes'
}
$changes = ($subjects -join "`n")

$notes = @"
## FaraBombRush $Tag

Beat Saber mod that spawns bombs during gameplay (Twitch chat commands and an auto mode synced to the song's BPM). See the [README](https://github.com/fara1991/fara-bomb-rush#readme) for commands and settings.

### Downloads (pick the one for your game version)

| Beat Saber | File |
|------------|------|
| 1.29.1 | ``FaraBombRush-$Tag-BS1.29.1.zip`` |
| 1.40.8 | ``FaraBombRush-$Tag-BS1.40.8.zip`` |

### Install

1. Install [BSIPA](https://github.com/bsmg/BeatSaber-IPA-Reloaded) 4.2.0 or later.
2. Extract the zip into the Beat Saber folder (it contains ``Plugins/`` and ``UserData/FaraBombRush/``).

### Requirements / Notes

- **Beat Saber 1.40.8**: SiraUtil (3.1.0+) and BeatSaberMarkupLanguage (1.12.0+) are required (the mod will not load without them). Install **BeatSaberPlus (ChatPlexSDK)** too: Twitch chat commands need it, and chat is disabled without it.
- **Beat Saber 1.29.1**: BeatSaberPlus (ChatPlexSDK) is needed for the Twitch chat commands as well.
- The zip for each game version only works with that version. Do not mix them.
- Updating from 1.0.0: the chat backend changed to ChatPlexSDK (BeatSaberPlus), and the BombLineCount setting was removed.

### $range

$changes
"@
$notesPath = Join-Path $DistDir "release-notes-$Tag.md"
[System.IO.File]::WriteAllText($notesPath, $notes, $Utf8NoBom)
Write-Host "  $notesPath"

# ---------------------------------------------------------------- dry run ends here
if (-not $Publish) {
    Write-Host ''
    Write-Host 'Dry run finished. Nothing was tagged or published.' -ForegroundColor Green
    Write-Host 'Zips:' -ForegroundColor Green
    $zips | ForEach-Object { Write-Host "  $_" }
    Write-Host "To publish: powershell -ExecutionPolicy Bypass -File .\tools\release.ps1 $Version -Publish"
    return
}

# ---------------------------------------------------------------- (d) tag + GitHub release
Write-Step "Publish $Tag"
$head = (Get-GitOutput @('rev-parse', '--short', 'HEAD')) | Select-Object -First 1
Write-Host "  tagging $head as $Tag"
Invoke-Native 'git' @('-C', $RepoRoot, 'tag', '-a', $Tag, '-m', "FaraBombRush $Tag")
Invoke-Native 'git' @('-C', $RepoRoot, 'push', 'origin', $Tag)
$ghArgs = @('release', 'create', $Tag) + $zips + @('--verify-tag', '--title', "FaraBombRush $Tag", '--notes-file', $notesPath)
Invoke-Native 'gh' $ghArgs
Write-Host "Published $Tag." -ForegroundColor Green
