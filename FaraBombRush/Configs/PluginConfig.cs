using System.Runtime.CompilerServices;
using FaraBombRush.Enums;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace FaraBombRush.Configs;

public class PluginConfig
{
    // 通常設定
    public string GameMode { get; set; } = GameModeEnum.Interactive.ToString();
    public bool IsBombCommandEnable { get; set; } = true;
    public bool IsBombCutEnable { get; set; } = true;
    public float BombSpawnDistance { get; set; } = 30.0f;
    public int BombLineCount { get; set; } = 5;
    public string PlayerBombLevel { get; set; } = FaraBombLevelEnum.PlayerLevel.Normal.ToString();
    
    // 特殊設定
    public bool IsAprilFoolMode { get; set; } = false;
}