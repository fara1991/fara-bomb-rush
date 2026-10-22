using System;

namespace FaraBombRush.Enums;

public enum GameModeEnum
{
    Interactive,
    Auto,
    Battle
}

public static class GameModeEnumHelper
{
    /// <summary>
    /// 保存された GameMode を、有効なモード名に直す。古い設定の "None" や不正な値は Interactive として扱う
    /// （どのモードも起動しない状態を作らないため）。大文字小文字と前後の空白は無視する。
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
}
