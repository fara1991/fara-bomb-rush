using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Enums;
using FaraBombRush.Interfaces;
using UnityEngine;

namespace FaraBombRush.Controllers.Components;

public class FaraBombScoreController : FaraBombComponentBase
{
    public float FaraBombScore { get; private set; }
    public int FaraBombCutCount { get; private set; }
    public int FaraBombThroughCount { get; private set; }
    public int FaraBombThroughComboCount { get; private set; }
    public float FaraBombComboRate { get; private set; }
    public float MagnificationByLevel { get; private set; }
    private readonly List<float> _faraBombComboRatesByThroughCount = [0f, 2f, 6f, 14f];

    // スコア調整用の定数
    private const float BaseFaraBombCutPenalty = 300f;
    private const float BaseAvoidBonus = 100f;
    private const float MaxComboRate = 8f;

    public override void InitializeComponent()
    {
        FaraBombScore = 0f;
        FaraBombCutCount = 0;
        FaraBombThroughCount = 0;
        FaraBombComboRate = 1f;
        MagnificationByLevel = FaraBombLevelEnum.GetLevelIndex(_pluginConfig.PlayerBombLevel) + 1f;

        Plugin.Logger.Debug("Initializing ScoreController");
    }

    // ぶつかった時の処理
    public void BombCut()
    {
        FaraBombScore -= BaseFaraBombCutPenalty * MagnificationByLevel;
        FaraBombCutCount++;
        FaraBombThroughComboCount = 0;
        FaraBombComboRate = 1f;
        Plugin.Logger.Debug($"Bomb hit! Score: {FaraBombScore}, Cuts: {FaraBombCutCount}, PlayerLevel: {MagnificationByLevel}");
    }

    // すり抜けた時の処理
    public void BombThrough()
    {
        if (!Mathf.Approximately(FaraBombComboRate, MaxComboRate))
        {
            foreach (var item in _faraBombComboRatesByThroughCount.Select((value, index) => new { value, index }))
            {
                if (FaraBombThroughCount >= item.value)
                {
                    FaraBombComboRate = Mathf.Pow(2, item.index);
                }
                else break;
            }
        }

        FaraBombScore += BaseAvoidBonus * FaraBombComboRate;
        FaraBombThroughCount++;
        FaraBombThroughComboCount++;
        Plugin.Logger.Debug($"Bomb avoided! Score: {FaraBombScore}, Avoids: {FaraBombThroughCount}, ComboRate: {FaraBombComboRate}, PlayerLevel: {MagnificationByLevel}");
    }
}