using System;
using System.Collections.Generic;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using static FaraBombRush.Enums.NoteLineCustomEnum;
using static FaraBombRush.Enums.FaraBombCreatePatternEnum;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers.GameModes;

public class FaraBombAutoModeController : FaraBombGameModeBase
{
    private int _noteCutCount;
    private int _playerBombLevel;

    private void Start()
    {
        _playerBombLevel = FaraBombLevelEnum.GetLevelIndex(Config.PlayerBombLevel);
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
        var patternList = GetPattern(_playerBombLevel);
        // ここでどのパターンかを抽出
        var pattern = FaraBombCreatePattern.BombSingle;
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
            case FaraBombCreatePattern.BombSingle:
            case FaraBombCreatePattern.BombDouble:
            case FaraBombCreatePattern.BombTriple:
                {
                    var start = 0;
                    var end = GetPatternIndex(pattern.ToString()) + 1;
                    Plugin.Logger.Debug("Single");
                    Plugin.Logger.Debug(start.ToString() + ", " + end.ToString());
                    for (var i = start; i < end; i++)
                    {
                        while (selectedPosList.Contains(posInt))
                        {
                            posInt = Random.Range(1, NotePositionEnumList.Count);
                        }

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
            case FaraBombCreatePattern.BombReset:
                {
                    SearchStartAndEndPosition(posInt, out var start, out var end);
                    Plugin.Logger.Debug("Reset");
                    Plugin.Logger.Debug(start.ToString() + ", " + end.ToString());
                    for (var i = start; i <= end; i++)
                    {
                        bombCommandListModel.Add(new BombCommandModel
                        {
                            BombId = BombId,
                            SpawnDelayTime = 0,
                            PositionIndex = i
                        });
                    }
                    FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
                    break;
                }
            case FaraBombCreatePattern.BombLineSingle:
            case FaraBombCreatePattern.BombLineDouble:
            case FaraBombCreatePattern.BombLineTriple:
                {
                    var start = 0;
                    var end = GetPatternIndex(pattern.ToString()) - 3;
                    Plugin.Logger.Debug("Double, Triple");
                    Plugin.Logger.Debug(start.ToString() + ", " + end.ToString());
                    for (var i = start; i < end; i++)
                    {
                        while (selectedPosList.Contains(posInt))
                        {
                            posInt = Random.Range(1, NotePositionEnumList.Count);
                        }

                        for (var j = 0; j < Config.BombLineCount; j++)
                        {
                            bombCommandListModel.Add(new BombCommandModel
                            {
                                BombId = BombId,
                                SpawnDelayTime = BombLineDiffBeat * j,
                                PositionIndex = posInt - 1
                            });
                        }
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