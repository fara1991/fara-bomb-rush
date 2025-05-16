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

    private const int AutoPushCoolCount = 150;

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
        if (_noteCutCount < AutoPushCoolCount)
        {
            _noteCutCount++;
        }
        else
        {
            _noteCutCount -= AutoPushCoolCount;
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

        var notePositionEnum = NotePositionEnumHelper.RandomPositionEnum();
        var selectedPosList = new List<NotePositionEnum>();

        var startPos = 0;
        int endPos;
        var bombCommandListModel = new List<BombCommandModel>();
        switch (pattern)
        {
            case FaraBombCreatePatternEnum.BombSingle:
            case FaraBombCreatePatternEnum.BombDouble:
            case FaraBombCreatePatternEnum.BombTriple:
                {
                    endPos = FaraBombCreatePatternEnumHelper.GetPatternIndex(pattern.ToString()) + 1;
                    for (var i = startPos; i < endPos; i++)
                    {
                        while (selectedPosList.Contains(notePositionEnum))
                        {
                            notePositionEnum =
                                NotePositionEnumHelper.RandomPositionEnum();
                        }

                        bombCommandListModel.Add(new BombCommandModel
                        {
                            BombId = BombId,
                            SpawnDelayTime = 0,
                            PositionIndex = notePositionEnum.GetValue()
                        });
                        selectedPosList.Add(notePositionEnum);
                    }

                    FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
                    break;
                }
            case FaraBombCreatePatternEnum.BombReset:
                {
                    if (notePositionEnum.IsTopPosition() || notePositionEnum == NotePositionEnum.CenterLeft)
                    {
                        startPos = NotePositionEnum.TopLeft.GetValue();
                        endPos = NotePositionEnum.TopRight.GetValue();
                    }
                    else
                    {
                        startPos = NotePositionEnum.BottomLeft.GetValue();
                        endPos = NotePositionEnum.BottomRight.GetValue();
                    }

                    for (var i = startPos; i <= endPos; i++)
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
                    endPos = FaraBombCreatePatternEnumHelper.GetPatternIndex(pattern.ToString()) - 3;
                    for (var i = startPos; i < endPos; i++)
                    {
                        while (selectedPosList.Contains(notePositionEnum))
                            notePositionEnum = NotePositionEnumHelper.RandomPositionEnum();

                        for (var j = 0; j < Config.BombLineCount; j++)
                        {

                            bombCommandListModel.Add(new BombCommandModel
                            {
                                BombId = BombId,
                                SpawnDelayTime = BombLineDiffBeat * j,
                                PositionIndex = notePositionEnum.GetValue()
                            });
                        }
                        selectedPosList.Add(notePositionEnum);
                    }

                    FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}