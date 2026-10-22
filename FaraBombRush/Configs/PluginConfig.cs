using System.Runtime.CompilerServices;
using FaraBombRush.Enums;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace FaraBombRush.Configs;

public class PluginConfig
{
    // 通常設定
    public bool IsEnabled { get; set; } = true;
    public string Language { get; set; } = "English";
    public string GameMode { get; set; } = GameModeEnum.Interactive.ToString();
    public bool IsBombCutEnable { get; set; } = true;
    public string PlayerBombLevel { get; set; } = FaraBombLevelEnum.Normal.ToString();

    // 特殊設定
    public bool IsAprilFoolMode { get; set; } = false;
}
