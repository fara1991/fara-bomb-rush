using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Controllers.Menus;
using FaraBombRush.Enums;
using UnityEngine;

namespace FaraBombRush.Controllers.Components;

internal class FaraBombScoreController : FaraBombComponentBaseController
{
    // スコア調整用の定数
    private const float BaseFaraBombCutPenalty = 300f;
    private const float BaseAvoidBonus = 100f;
    private const float MaxComboRate = 8f;
    private readonly List<float> _faraBombComboRatesByThroughCount = [0f, 2f, 6f, 14f];
    internal float FaraBombScore { get; private set; }
    internal int FaraBombCutCount { get; private set; }
    internal int FaraBombThroughCount { get; private set; }
    internal int FaraBombThroughComboCount { get; private set; }
    internal float FaraBombComboRate { get; private set; }

    protected override void InitializeComponent()
    {
        FaraBombScore = 0f;
        FaraBombCutCount = 0;
        FaraBombThroughCount = 0;
        FaraBombComboRate = 1f;
        Plugin.Logger.Debug("Initializing ScoreController");
    }

    // ぶつかった時の処理
    internal void BombCut()
    {
        FaraBombCutCount++;
        FaraBombThroughComboCount = 0;
        FaraBombComboRate = 1f;
        FaraBombScore -= BaseFaraBombCutPenalty * FaraBombCutCount;
        Plugin.Logger.Debug(
            $"Bomb hit! Score: {FaraBombScore}, Cuts: {FaraBombCutCount}");
    }

    // すり抜けた時の処理
    internal void BombThrough()
    {
        if (!Mathf.Approximately(FaraBombComboRate, MaxComboRate))
            foreach (var item in _faraBombComboRatesByThroughCount.Select((value, index) => new {value, index}))
                if (FaraBombThroughCount >= item.value)
                    FaraBombComboRate = Mathf.Pow(2, item.index);
                else break;

        FaraBombThroughCount++;
        FaraBombThroughComboCount++;
        FaraBombScore += BaseAvoidBonus * FaraBombComboRate;
        Plugin.Logger.Debug(
            $"Bomb avoided! Score: {FaraBombScore}, Avoids: {FaraBombThroughCount}, ComboRate: {FaraBombComboRate}");
    }
}