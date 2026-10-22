using System;
using CP_SDK.Chat.Interfaces;

namespace FaraBombRush.Services;

/// <summary>
/// ChatPlexSDK への参照をこのクラスに閉じ込める。
/// ChatPlexSDK（BeatSaberPlus）が無い環境では、このクラスのメソッド呼び出しが例外になるだけで、
/// 呼び出し側（try/catch 内）がそれを捕まえられる。ChatPlexSDK の型を含むクラスを直接 Bind すると、
/// プラグインの読み込み時点で失敗する恐れがあるため、呼び出し側（コントローラ）からは型を参照しない。
/// </summary>
internal static class ChatPlexBridge
{
    private static Action<string, string> _onMessage;

    /// <summary>チャット受信を開始する。onMessage には（送信元の表示名, メッセージ本文）が渡る。</summary>
    public static void Acquire(Action<string, string> onMessage)
    {
        _onMessage = onMessage;
        CP_SDK.Chat.Service.Acquire();
        try
        {
            CP_SDK.Chat.Service.Multiplexer.OnTextMessageReceived += OnTextMessageReceived;
        }
        catch
        {
            // 購読に失敗したら、取得したサービスを戻してから例外を渡す
            CP_SDK.Chat.Service.Release();
            _onMessage = null;
            throw;
        }
    }

    /// <summary>チャット受信を止めて、サービスを返す。Acquire が成功したあとにだけ呼ぶこと。</summary>
    public static void Release()
    {
        try
        {
            CP_SDK.Chat.Service.Multiplexer.OnTextMessageReceived -= OnTextMessageReceived;
        }
        finally
        {
            _onMessage = null;
            CP_SDK.Chat.Service.Release();
        }
    }

    private static void OnTextMessageReceived(IChatService service, IChatMessage message)
    {
        _onMessage?.Invoke(service.DisplayName, message.Message);
    }
}
