using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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
/// 判定の前に、制御文字・書式文字（UnicodeCategory.Format）・未割り当て文字（U+E0000 など。7TV が付ける不可視文字）を取り除き、
/// 先頭の "@名前"（Twitch の返信）は 1 つだけ読み飛ばす。
/// 数字は ASCII の 0〜9 だけ（全角数字は拒否）。コマンド側の "!" も ASCII だけなので、全角を片方だけ許すと一貫しなくなるため。
/// </summary>
public static class BombChatCommandParser
{
    public const int MinPosition = 1;
    public const int MaxPosition = 12;

    public static bool TryParse(string chat, out BombChatCommand command)
    {
        command = default;
        if (string.IsNullOrWhiteSpace(chat)) return false;

        var tokens = Tokenize(chat);

        // Twitch の返信は "@名前 !bomb" の形で届く。先頭の "@名前" を 1 つだけ読み飛ばす
        var start = tokens.Count > 1 && tokens[0].Length > 1 && tokens[0][0] == '@' ? 1 : 0;
        var count = tokens.Count - start;
        if (count is < 1 or > 2) return false;

        BombChatCommandKind kind;
        switch (tokens[start].ToLowerInvariant())
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
        if (count == 2)
        {
            if (!TryParsePosition(tokens[start + 1], out var value)) return false;
            position = value;
        }

        command = new BombChatCommand(kind, position);
        return true;
    }

    /// <summary>空白で区切り、各トークンから制御文字と書式文字を取り除く。取り除いて空になったトークンは捨てる。</summary>
    private static List<string> Tokenize(string chat)
    {
        var tokens = new List<string>();
        var current = new StringBuilder();

        void Flush()
        {
            if (current.Length == 0) return;
            tokens.Add(current.ToString());
            current.Clear();
        }

        for (var i = 0; i < chat.Length; i++)
        {
            var c = chat[i];
            var pair = char.IsHighSurrogate(c) && i + 1 < chat.Length && char.IsLowSurrogate(chat[i + 1]);

            if (char.IsWhiteSpace(c))
            {
                Flush();
                continue;
            }

            // サロゲートペアは 2 文字ぶんを 1 つの文字として分類する（7TV の不可視文字は U+E0000 台）
            var category = pair
                ? CharUnicodeInfo.GetUnicodeCategory(chat, i)
                : CharUnicodeInfo.GetUnicodeCategory(c);
            var drop = category == UnicodeCategory.Format || category == UnicodeCategory.Control
                       || category == UnicodeCategory.OtherNotAssigned || category == UnicodeCategory.Surrogate;

            if (!drop)
            {
                current.Append(c);
                if (pair) current.Append(chat[i + 1]);
            }
            if (pair) i++;
        }
        Flush();
        return tokens;
    }

    private static bool TryParsePosition(string token, out int value)
    {
        value = 0;
        // 数字だけを受け付ける（"+5" や全角数字などは拒否）。長さの上限で int のオーバーフローも防ぐ
        if (token.Length == 0 || token.Length > 2 || !token.All(c => c >= '0' && c <= '9')) return false;

        value = int.Parse(token, CultureInfo.InvariantCulture);
        return value >= MinPosition && value <= MaxPosition;
    }
}
