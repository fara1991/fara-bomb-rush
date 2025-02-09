using System;
using System.Collections.Generic;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers.GameModes;

internal class FaraBombAutoModeController : FaraBombGameModeBaseController
{
    private int _noteCutCount;
    private int _playerBombLevel;
    

    private void Start()
    {
        _playerBombLevel = FaraBombLevelEnumHelper.GetLevelIndex(Config.PlayerBombLevel);
    }

    private void Update()
    {
        BombPush();
    }

    protected override void BombPush()
    {
        if (_noteCutCount < 180)
        {
            _noteCutCount++;
        }
        else
        {
            _noteCutCount -= 180;
            CalcBombPattern();
        }
    }

    private void CalcBombPattern()
    {
        // bomb制御用に適用なIDを付与
        BombId = BombId >= int.MaxValue ? 1 : BombId + 1;

        var randomPercent = Random.Range(0f, 100f);
        var patternList = FaraBombCreatePatternEnumHelper.GetPattern(_playerBombLevel);
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

        var posInt = Random.Range(1, NotePositionEnumList.Count);
        var selectedPosList = new List<int>();

        var bombCommandListModel = new List<BombCommandModel>();
        switch (pattern)
        {
            case FaraBombCreatePatternEnum.BombSingle:
            case FaraBombCreatePatternEnum.BombDouble:
            case FaraBombCreatePatternEnum.BombTriple:
            {
                var start = 0;
                var end = FaraBombCreatePatternEnumHelper.GetPatternIndex(pattern.ToString()) + 1;
                Plugin.Logger.Debug("Single");
                for (var i = start; i < end; i++)
                {
                    // 1～10のPosition計算が必要
                    while (selectedPosList.Contains(posInt)) posInt = Random.Range(1, NotePositionEnumList.Count);

                    bombCommandListModel.Add(new BombCommandModel
                    {
                        BombId = BombId,
                        SpawnDelayTime = 0,
                        PositionIndex = posInt - 1
                    });
                    selectedPosList.Add(posInt);
                }

                FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
                break;
            }
            case FaraBombCreatePatternEnum.BombReset:
            {
                SearchStartAndEndPosition(posInt, out var start, out var end);
                Plugin.Logger.Debug("Reset");
                for (var i = start; i <= end; i++)
                    bombCommandListModel.Add(new BombCommandModel
                    {
                        BombId = BombId,
                        SpawnDelayTime = 0,
                        PositionIndex = i
                    });
                FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
                break;
            }
            case FaraBombCreatePatternEnum.BombLineSingle:
            case FaraBombCreatePatternEnum.BombLineDouble:
            case FaraBombCreatePatternEnum.BombLineTriple:
            {
                var start = 0;
                var end = FaraBombCreatePatternEnumHelper.GetPatternIndex(pattern.ToString()) - 3;
                Plugin.Logger.Debug("Double, Triple");
                for (var i = start; i < end; i++)
                {
                    while (selectedPosList.Contains(posInt)) posInt = Random.Range(1, NotePositionEnumList.Count);

                    for (var j = 0; j < Config.BombLineCount; j++)
                        bombCommandListModel.Add(new BombCommandModel
                        {
                            BombId = BombId,
                            SpawnDelayTime = BombLineDiffBeat * j,
                            PositionIndex = posInt - 1
                        });
                    FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
                    selectedPosList.Add(posInt);
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SearchStartAndEndPosition(int posInt, out int start, out int end)
    {
        var e = (NotePositionEnum) (posInt - 1);
        if (e.IsTopPosition())
        {
            start = NotePositionEnum.TopLeft.GetPositionIndex();
            end = NotePositionEnum.TopRight.GetPositionIndex();
        }
        else if (e.IsBottomPosition())
        {
            start = NotePositionEnum.BottomLeft.GetPositionIndex();
            end = NotePositionEnum.BottomRight.GetPositionIndex();
        }
        else if (e.IsCenterPosition())
        {
            start = NotePositionEnum.CenterLeft.GetPositionIndex();
            end = NotePositionEnum.CenterRight.GetPositionIndex();
        }
        else
        {
            // 万が一変な数値が来たらBottomのボムリセとして扱う
            Plugin.Logger.Debug($"Outbound value. pos: {posInt}.");
            start = NotePositionEnum.BottomLeft.GetPositionIndex();
            end = NotePositionEnum.BottomRight.GetPositionIndex();
        }
    }
}