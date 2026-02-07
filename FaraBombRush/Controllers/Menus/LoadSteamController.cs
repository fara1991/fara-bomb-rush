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
    private const int MaxRetryCount = 5;
    private const float RetryInterval = 3f;

    internal static float ScoreSaberPP { get; private set; }

    private void Start()
    {
        StartCoroutine(LoadSteamDataCoroutine());
    }

    private IEnumerator LoadSteamDataCoroutine()
    {
        var retryCount = 0;

        // Steam初期化を待つ
        while (!SteamManager.Initialized && retryCount < MaxRetryCount)
        {
            Plugin.Logger.Info($"Waiting for Steam initialization... Attempt {retryCount + 1}");
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
}
