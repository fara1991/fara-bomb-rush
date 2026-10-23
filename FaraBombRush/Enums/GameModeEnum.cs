using System;
using System.Linq;

namespace FaraBombRush.Enums;

public enum GameModeEnum
{
    None,
    Interactive,
    Auto,
    Battle
}

public static class GameModeEnumHelper
{
    /// <summary>設定画面の選択肢。未実装の Battle は含めない。</summary>
    public static readonly string[] SelectableNames =
    {
        nameof(GameModeEnum.None),
        nameof(GameModeEnum.Interactive),
        nameof(GameModeEnum.Auto)
    };

    /// <summary>
    /// 保存された GameMode を、モード名に直す。大文字小文字と前後の空白は無視する。
    /// None は None のまま返す（1.0.0 の既定値。選んだ人の設定を勝手に有効にしない）。
    /// Battle も名前としては Battle のまま返す（何もしない扱いは <see cref="RunsMod"/> で決める）。
    /// 空・null・不正な値は Interactive として扱う。
    /// </summary>
    public static string Normalize(string value)
    {
        var trimmed = value?.Trim();
        foreach (var mode in Enum.GetNames(typeof(GameModeEnum)))
        {
            if (string.Equals(trimmed, mode, StringComparison.OrdinalIgnoreCase)) return mode;
        }
        return nameof(GameModeEnum.Interactive);
    }

    /// <summary>Mod（とチャット読み取り）を起動するモードか。None と未実装の Battle は何も起動しない。</summary>
    public static bool RunsMod(string value)
    {
        var mode = Normalize(value);
        return mode == nameof(GameModeEnum.Interactive) || mode == nameof(GameModeEnum.Auto);
    }

    /// <summary>設定画面に表示する値。選択肢にない Battle は None（何もしない）として見せる。</summary>
    public static string ToSelectable(string value)
    {
        var mode = Normalize(value);
        return SelectableNames.Contains(mode) ? mode : nameof(GameModeEnum.None);
    }
}
