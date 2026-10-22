using System.Runtime.CompilerServices;
using FaraBombRush.Enums;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace FaraBombRush.Configs;

// IPA の自動保存は virtual なプロパティの変更だけを検出するため、すべて virtual にする
// （非 virtual だと設定画面で変えた値が再起動後に残らない）
public class PluginConfig
{
    // 通常設定
    public virtual bool IsEnabled { get; set; } = true;

    /// <summary>表示言語。"English" か "Japanese"。古い設定に無い・不正な値は英語として扱う（UiText を参照）。</summary>
    public virtual string Language { get; set; } = "English";

    public virtual string GameMode { get; set; } = GameModeEnum.Interactive.ToString();
    public virtual bool IsBombCutEnable { get; set; } = true;
    public virtual string PlayerBombLevel { get; set; } = FaraBombLevelEnum.Normal.ToString();

    // 特殊設定
    public virtual bool IsAprilFoolMode { get; set; } = false;

    /// <summary>設定値が変わったときに呼ぶ（保存は IPA が行う）。</summary>
    public virtual void Changed()
    {
    }
}
