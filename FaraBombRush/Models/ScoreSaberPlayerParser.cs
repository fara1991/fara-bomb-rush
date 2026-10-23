using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace FaraBombRush.Models;

public enum ScoreSaberLoadState
{
    /// <summary>まだ取得を始めていない（Steam の初期化待ちを含む）。</summary>
    NotStarted,
    Loading,
    Loaded,
    /// <summary>ScoreSaber にこのプレイヤー ID のアカウントがない（HTTP 404）。</summary>
    NotRegistered,
    /// <summary>通信・応答の解析などに失敗した。</summary>
    Failed
}

/// <summary>
/// ScoreSaber の /api/player/{id}/basic の応答から PP を取り出す純粋関数。
/// 必要なのは "pp" だけなので、他の項目（scoreStats など）が増減・改名されても失敗しないように、型を決め打ちせず "pp" だけを読む。
/// （以前は全項目を必須にしていて、応答に無い "replayedWatched"（実際は "replaysWatched"）のせいで毎回例外になり、PP が 0 のままだった。）
/// </summary>
public static class ScoreSaberPlayerParser
{
    public static bool TryParsePp(string json, out float pp, out string error)
    {
        pp = 0f;
        error = null;

        if (string.IsNullOrWhiteSpace(json))
        {
            error = "empty response";
            return false;
        }

        JObject root;
        try
        {
            root = JObject.Parse(json);
        }
        catch (Exception ex)
        {
            error = $"invalid JSON: {ex.Message}";
            return false;
        }

        var token = root["pp"];
        if (token == null || token.Type == JTokenType.Null)
        {
            error = "\"pp\" is missing in the response";
            return false;
        }

        if (token.Type is not (JTokenType.Float or JTokenType.Integer or JTokenType.String))
        {
            error = $"\"pp\" has an unexpected type: {token.Type}";
            return false;
        }

        if (!float.TryParse(Convert.ToString(token, CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out pp)
            || float.IsNaN(pp) || float.IsInfinity(pp) || pp < 0f)
        {
            pp = 0f;
            error = $"\"pp\" is not a valid number: {token}";
            return false;
        }

        return true;
    }
}
