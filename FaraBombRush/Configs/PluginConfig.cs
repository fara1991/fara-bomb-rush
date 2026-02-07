using System.Runtime.CompilerServices;
using FaraBombRush.Enums;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace FaraBombRush.Configs;

public class PluginConfig
{
    // 通常設定
    public string GameMode { get; set; } = GameModeEnum.None.ToString();
    public bool IsBombCutEnable { get; set; } = true;
    public int BombLineCount { get; set; } = 5;
    public string PlayerBombLevel { get; set; } = FaraBombLevelEnum.Normal.ToString();

    // 特殊設定
    public bool IsAprilFoolMode { get; set; } = false;
}