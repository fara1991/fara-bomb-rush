using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using UnityEngine;
using Zenject;
using static FaraBombRush.Enums.NoteLineCustomEnum;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers.GameModes;

public class FaraBombAutoModeController : MonoBehaviour
{
    private int _noteCutCount;

    private readonly List<NotePosition> _notePositionEnumList =
        Enum.GetValues(typeof(NotePosition)).Cast<NotePosition>().ToList();
    private const float BombLineDiffBeat = 1.0f;

    private int _bombId;
    private int _playerBombLevel;
    private PluginConfig _config;

    [Inject]
    private void Construct(PluginConfig config)
    {
        _config = config;
    }


    private void Start()
    {        Plugin.Logger.Debug("234");

        _playerBombLevel = FaraBombLevelEnum.GetLevelIndex(_config.PlayerBombLevel);
    }

    private void Update()
    {
        // ノーツをN回切ったらボムを出す
        if (_noteCutCount < 180) return;
        _noteCutCount -= 180;
        BombPush();
    }

    public void BombPush()
    {
        // bomb制御用に適用なIDを付与
        _bombId = _bombId >= int.MaxValue ? 1 : _bombId + 1;

        var randomPercent = Random.Range(0, 100);
        var patternList = FaraBombCreatePatternExternal.GetPattern(_playerBombLevel);
        // ここでどのパターンかを抽出
        var pattern = FaraBombCreatePatternEnum.BombSingle;
        var calcPercent = 0f;
        foreach (var test in patternList)
        {
            calcPercent += test.Value;
            if (calcPercent >= randomPercent)
            {
                pattern = test.Key;
                break;
            }
        }

        var posInt = Random.Range(1, _notePositionEnumList.Count);
        var selectedPosList = new List<int>();

        var bombCommandListModel = new List<BombCommandModel>();
        if (pattern == FaraBombCreatePatternEnum.BombSingle ||
            pattern == FaraBombCreatePatternEnum.BombDouble ||
            pattern == FaraBombCreatePatternEnum.BombTriple)
        {
            var start = 0;
            var end = int.Parse(pattern.ToString()) + 1;
            for (var i = start; i < end; i++)
            {
                while (selectedPosList.Contains(posInt))
                {
                    posInt = Random.Range(1, _notePositionEnumList.Count);
                }
                bombCommandListModel.Add(new BombCommandModel
                {
                    BombId = _bombId,
                    SpawnDelayTime = 0,
                    PositionIndex = posInt - 1
                });
                FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
                selectedPosList.Add(posInt);
            }
        }

        else if (pattern == FaraBombCreatePatternEnum.BombReset)
        {
            SearchStartAndEndPosition(posInt, out var start, out var end);
            for (var i = start; i <= end; i++)
                bombCommandListModel.Add(new BombCommandModel
                {
                    BombId = _bombId,
                    SpawnDelayTime = 0,
                    PositionIndex = i
                });
            FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
        }
        else if (pattern == FaraBombCreatePatternEnum.BombLineSingle || pattern == FaraBombCreatePatternEnum.BombLineDouble || pattern == FaraBombCreatePatternEnum.BombLineTriple)
        {
            var start = int.Parse(pattern.ToString()) - 4;
            var end = int.Parse(pattern.ToString()) - 2;
            for (var i = start; i < end; i++)
            {
                while (selectedPosList.Contains(posInt))
                {
                    posInt = Random.Range(1, _notePositionEnumList.Count);
                }
                for (var j = 0; j < _config.BombLineCount; j++)
                    bombCommandListModel.Add(new BombCommandModel
                    {
                        BombId = _bombId,
                        SpawnDelayTime = BombLineDiffBeat * j,
                        PositionIndex = posInt - j
                    });
                FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
                selectedPosList.Add(posInt);
            }
        }
    }

    private void SearchStartAndEndPosition(int posInt, out int start, out int end)
    {
        var e = (NoteLineCustomEnum)(posInt - 1);
        if (e.IsTopPosition())
        {
            start = TopLeft.GetPositionIndex();
            end = TopRight.GetPositionIndex();
        }
        else if (e.IsBottomPosition())
        {
            start = BottomLeft.GetPositionIndex();
            end = BottomRight.GetPositionIndex();
        }
        else if (e.IsCenterPosition())
        {
            start = CenterLeft.GetPositionIndex();
            end = CenterRight.GetPositionIndex();
        }
        else
        {
            // 万が一変な数値が来たらBottomのボムリセとして扱う
            Plugin.Logger.Debug($"Outbound value. pos: {posInt}.");
            start = BottomLeft.GetPositionIndex();
            end = BottomRight.GetPositionIndex();
        }
    }
}