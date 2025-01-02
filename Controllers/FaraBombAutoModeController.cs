using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using static FaraBombRush.Enums.NoteLineCustomEnum;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers;

public class FaraBombAutoModeController : FaraBombBaseController
{
    private int _noteCutCount;

    private readonly List<NotePositionEnum> _notePositionEnumList =
        Enum.GetValues(typeof(NotePositionEnum)).Cast<NotePositionEnum>().ToList();

    private int _bombId;
    private FaraBombLevel _level;

    private void Start()
    {
        _level = FaraBombLevelExternal.GetLevel(_config.PlayerBombLevel);
    }

    private void Update()
    {
        // ノーツをN回切ったらボムを出す
        if (_noteCutCount < 5) return;
        _noteCutCount -= 5;
        BombPush();
    }

    public void BombPush()
    {
        // bomb制御用に適用なIDを付与
        _bombId = _bombId >= int.MaxValue ? 1 : _bombId + 1;

        var randomPercent = Random.Range(0, 100);
        var patternList = FaraBombCreatePatternExternal.GetPattern(_level);
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
                FaraBombPoolManager.CommandQueue.Enqueue(bombCommandListModel);
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
            FaraBombPoolManager.CommandQueue.Enqueue(bombCommandListModel);
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
                FaraBombPoolManager.CommandQueue.Enqueue(bombCommandListModel);
                selectedPosList.Add(posInt);
            }
        }
    }
}