using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FaraBombRush.Models;
using Steamworks;
using UnityEngine;

namespace FaraBombRush.Controllers.Menus;

internal class LoadSteamController : MonoBehaviour
{
    private const int MaxSteamRetryCount = 5;
    private const float SteamRetryInterval = 3f;
    private const int MaxFetchAttempts = 3;
    private const float FetchRetryInterval = 5f;
    private const int RequestTimeoutSeconds = 10;
    private const int LogBodyMaxLength = 200;

    internal static float ScoreSaberPP { get; private set; }

    internal static ScoreSaberLoadState State { get; private set; } = ScoreSaberLoadState.NotStarted;

    /// <summary>取得に失敗したときの原因（設定画面のヒントには出さず、ログ用）。</summary>
    internal static string FailureReason { get; private set; }

    /// <summary>State / ScoreSaberPP が変わったときに呼ばれる（Unity のメインスレッドから）。</summary>
    internal static event Action Changed;

    private void Start()
    {
        StartCoroutine(LoadSteamDataCoroutine());
    }

    private static bool IsSteamInitialized()
    {
#if GAME_1_40_8
        // 1.40 の SteamManager は静的な Initialized を持たない（インスタンスの isInitialised だけ）ので、Steamworks.NET で確認する。
        // Steam の初期化前に呼ぶと InvalidOperationException になる
        try
        {
            return SteamUser.GetSteamID().IsValid();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
#else
        return SteamManager.Initialized;
#endif
    }

    private static void SetState(ScoreSaberLoadState state, string failureReason = null)
    {
        State = state;
        FailureReason = failureReason;
        Changed?.Invoke();
    }

    private IEnumerator LoadSteamDataCoroutine()
    {
        SetState(ScoreSaberLoadState.Loading);

        var retryCount = 0;
        while (!IsSteamInitialized() && retryCount < MaxSteamRetryCount)
        {
            Plugin.Logger.Info($"Waiting for Steam initialization... Attempt {retryCount + 1}");
            yield return new WaitForSeconds(SteamRetryInterval);
            retryCount++;
        }

        if (!IsSteamInitialized())
        {
            Plugin.Logger.Error("ScoreSaber PP was not loaded: Steam initialization failed");
            SetState(ScoreSaberLoadState.Failed, "Steam initialization failed");
            yield break;
        }

        string steamUserId;
        try
        {
            steamUserId = SteamUser.GetSteamID().m_SteamID.ToString();
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error($"ScoreSaber PP was not loaded: could not get the Steam ID: {ex}");
            SetState(ScoreSaberLoadState.Failed, "could not get the Steam ID");
            yield break;
        }

        var url = $"https://scoresaber.com/api/player/{steamUserId}/basic";
        Plugin.Logger.Info($"Loading ScoreSaber PP: {url}");

        for (var attempt = 1; attempt <= MaxFetchAttempts; attempt++)
        {
            // 通信は別スレッドで行い、メインスレッドを止めない（以前は .Result で待っていた）
            var task = Task.Run(() => FetchAsync(url));
            while (!task.IsCompleted) yield return null;

            var result = task.IsFaulted
                ? FetchResult.Error(task.Exception?.GetBaseException().Message ?? "unknown error")
                : task.Result;

            if (result.Succeeded)
            {
                ScoreSaberPP = result.Pp;
                Plugin.Logger.Info($"ScoreSaber PP: {result.Pp}");
                SetState(ScoreSaberLoadState.Loaded);
                yield break;
            }

            if (result.NotRegistered)
            {
                Plugin.Logger.Warn($"ScoreSaber PP was not loaded: no ScoreSaber account for Steam ID {steamUserId} (HTTP 404). {result.Message}");
                SetState(ScoreSaberLoadState.NotRegistered, result.Message);
                yield break;
            }

            Plugin.Logger.Warn($"ScoreSaber PP load failed (attempt {attempt}/{MaxFetchAttempts}): {result.Message}");
            if (attempt < MaxFetchAttempts)
            {
                yield return new WaitForSeconds(FetchRetryInterval);
                continue;
            }

            Plugin.Logger.Error($"ScoreSaber PP was not loaded: {result.Message}");
            SetState(ScoreSaberLoadState.Failed, result.Message);
        }
    }

    private static async Task<FetchResult> FetchAsync(string url)
    {
        try
        {
            // .NET Framework 4.8.1 / Mono では、既定のままだと古い TLS で接続しようとして失敗することがあるので TLS 1.2 を許可する
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(RequestTimeoutSeconds) };
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "FaraBombRush (+https://github.com/fara1991/fara-bomb-rush)");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");

            using var response = await httpClient.GetAsync(url).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var status = (int)response.StatusCode;
            var snippet = body.Length > LogBodyMaxLength ? body.Substring(0, LogBodyMaxLength) : body;

            if (status == 404) return FetchResult.NotFound($"HTTP 404: {snippet}");
            if (!response.IsSuccessStatusCode) return FetchResult.Error($"HTTP {status}: {snippet}");

            if (!ScoreSaberPlayerParser.TryParsePp(body, out var pp, out var parseError))
                return FetchResult.Error($"HTTP {status} but could not read the PP: {parseError}. Body: {snippet}");

            return FetchResult.Success(pp);
        }
        catch (TaskCanceledException)
        {
            return FetchResult.Error($"request timed out ({RequestTimeoutSeconds}s)");
        }
        catch (Exception ex)
        {
            return FetchResult.Error($"{ex.GetType().Name}: {ex.Message}");
        }
    }

    private readonly struct FetchResult
    {
        private FetchResult(bool succeeded, bool notRegistered, float pp, string message)
        {
            Succeeded = succeeded;
            NotRegistered = notRegistered;
            Pp = pp;
            Message = message;
        }

        internal bool Succeeded { get; }
        internal bool NotRegistered { get; }
        internal float Pp { get; }
        internal string Message { get; }

        internal static FetchResult Success(float pp) => new(true, false, pp, null);
        internal static FetchResult NotFound(string message) => new(false, true, 0f, message);
        internal static FetchResult Error(string message) => new(false, false, 0f, message);
    }
}
