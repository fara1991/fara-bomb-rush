using System;
using System.Linq;

namespace FaraBombRush.Models;

public enum BombChatCommandKind
{
    Bomb,
    Line,
    Reset
}

public readonly struct BombChatCommand
{
    public BombChatCommand(BombChatCommandKind kind, int? position)
    {
        Kind = kind;
        Position = position;
    }

    public BombChatCommandKind Kind { get; }

    /// <summary>位置番号（1〜12）。省略時は null（ランダム）。</summary>
    public int? Position { get; }
}

/// <summary>
/// Twitch チャットの !bomb / !bombline / !bombreset を判定する純粋関数。Unity や Beat Saber に依存しない。
/// 受理するのは、先頭トークンが完全一致（大文字小文字を無視）するコマンドで、
/// 引数は省略するか 2 トークン目の整数 1〜12 だけ。
/// </summary>
public static class BombChatCommandParser
{
    public const int MinPosition = 1;
    public const int MaxPosition = 12;

    public static bool TryParse(string chat, out BombChatCommand command)
    {
        command = default;
        if (string.IsNullOrWhiteSpace(chat)) return false;

        var tokens = chat.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length is < 1 or > 2) return false;

        BombChatCommandKind kind;
        switch (tokens[0].ToLowerInvariant())
        {
            case "!bomb":
                kind = BombChatCommandKind.Bomb;
                break;
            case "!bombline":
                kind = BombChatCommandKind.Line;
                break;
            case "!bombreset":
                kind = BombChatCommandKind.Reset;
                break;
            default:
                return false;
        }

        int? position = null;
        if (tokens.Length == 2)
        {
            if (!TryParsePosition(tokens[1], out var value)) return false;
            position = value;
        }

        command = new BombChatCommand(kind, position);
        return true;
    }

    private static bool TryParsePosition(string token, out int value)
    {
        value = 0;
        // 数字だけを受け付ける（"+5" や全角数字などは拒否）。長さの上限で int のオーバーフローも防ぐ
        if (token.Length == 0 || token.Length > 2 || !token.All(c => c >= '0' && c <= '9')) return false;

        value = int.Parse(token, System.Globalization.CultureInfo.InvariantCulture);
        return value >= MinPosition && value <= MaxPosition;
    }
}
