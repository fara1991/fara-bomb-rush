using System;
using System.Collections;
using System.Net.Http;
using FaraBombRush.Enums;
using FaraBombRush.Exceptions;
using FaraBombRush.Models;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

namespace FaraBombRush.Controllers.Menus;

internal class LoadSteamController : MonoBehaviour
{
    private static float _scoreSaberPP;
    private const int MaxRetryCount = 5;
    private const float RetryInterval = 3f;
    
    internal static float ScoreSaberPP
    {
        get => _scoreSaberPP;
        set
        {
            _scoreSaberPP = value;
            OnChanged?.Invoke(null, value); // イベント発火
            Plugin.Logger.Debug($"ScoreSaber PP: {value}");
        }
    }

    private void Start()
    {
        Plugin.Logger.Info("Loading Steam");
        StartCoroutine(LoadSteamDataCoroutine());
    }

    private IEnumerator LoadSteamDataCoroutine()
    {
        Plugin.Logger.Info("Start LoadSteamDataCoroutine");
        var retryCount = 0;
        
        // Steam初期化を待つ
        while (!SteamManager.Initialized && retryCount < MaxRetryCount)
        {
            Plugin.Logger.Debug($"Waiting for Steam initialization... Attempt {retryCount + 1}");
            yield return new WaitForSeconds(RetryInterval);
            retryCount++;
        }
        
        try
        {
            if (!SteamManager.Initialized)
            {
                throw new FaraBombException("Steam initialization failed", ErrorCodeEnum.LoadSteamError);
            }

            var steamUserId = SteamUser.GetSteamID().m_SteamID.ToString();
            Plugin.Logger.Debug($"SteamUserId: {steamUserId}");
            var scoreSaberUrl = $"https://scoresaber.com/api/player/{steamUserId}/basic";

            using var httpClient = new HttpClient();
            var response = httpClient.GetAsync(scoreSaberUrl).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;

            var scoreSaberData = JsonConvert.DeserializeObject<ScoreSaberModel>(jsonString);
            ScoreSaberPP = scoreSaberData.PP;
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCodeEnum.LoadSteamError);
        }
    }
    
    public static event EventHandler<float> OnChanged;
}