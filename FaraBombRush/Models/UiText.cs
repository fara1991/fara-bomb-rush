using System;

namespace FaraBombRush.Models;

/// <summary>
/// 表示言語の判定と文言の選択。FaraAccField と同じ方式（設定値 "English" / "Japanese" で英語と日本語を切り替える）。
/// Unity や Beat Saber に依存しないので、ゲーム DLL なしで検証できる。
/// 設定値が無い・不正・大文字小文字違いのときは英語にフォールバックする。
/// </summary>
public static class UiText
{
    public const string English = "English";
    public const string Japanese = "Japanese";

    /// <summary>設定画面の言語の選択肢（保存される値そのもの）。</summary>
    public static readonly string[] LanguageOptions = { English, Japanese };

    /// <summary>言語が変わったことを通知する。設定画面とメニューボタンの文言を更新するために使う。</summary>
    public static event Action LanguageChanged;

    public static void RaiseLanguageChanged() => LanguageChanged?.Invoke();

    public static bool IsJapanese(string language) =>
        string.Equals(language?.Trim(), Japanese, StringComparison.OrdinalIgnoreCase);

    /// <summary>保存値を、正規の言語名（"English" か "Japanese"）に直す。</summary>
    public static string Normalize(string language) => IsJapanese(language) ? Japanese : English;

    /// <summary>言語に応じて英語か日本語の文言を返す。</summary>
    public static string Pick(string language, string english, string japanese) =>
        IsJapanese(language) ? japanese : english;
}
