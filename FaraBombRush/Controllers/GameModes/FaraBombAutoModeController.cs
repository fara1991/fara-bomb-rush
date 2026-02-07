using System;
using System.Collections.Generic;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers.GameModes;

internal class FaraBombAutoModeController : FaraBombGameModeBaseController
{
    private int _playerBombLevel;
    private int _lastBeatIndex = -1;
    private float _beatInterval = 1.0f; // Seconds per beat

    private void Start()
    {
        _playerBombLevel = FaraBombLevelEnumHelper.GetLevelIndex(Config.PlayerBombLevel);
        _beatInterval = 60f / Bpm; // Calculate beat interval from BPM
    }

    private void Update()
    {
        BombPush();
    }

    protected override void BombPush()
    {
        if (!IsSongPlaying()) return;

        float songTime = GetCurrentSongTime();

        // Detect song start
        if (!SongStarted && songTime > 0)
        {
            SongStarted = true;
            SongStartTime = songTime;
        }

        if (!SongStarted) return;

        // Calculate how many beats ahead we want to spawn bombs
        // Use a buffer (e.g., 5 beats) to ensure commands are in queue before FaraBombSystemManager needs them
        // SystemManager spawns at 4 beats ahead, so we need at least that much.
        float lookAheadTime = _beatInterval * 5.0f;

        float adjustedTime = songTime - SongStartTime;
        int maxBeatIndexToSpawn = (int)((adjustedTime + lookAheadTime) / _beatInterval);

        const int maxBeatsPerFrame = 8;
        int beatsProcessed = 0;
        while (_lastBeatIndex < maxBeatIndexToSpawn && beatsProcessed < maxBeatsPerFrame)
        {
            _lastBeatIndex++;
            beatsProcessed++;
            float hitTime = (_lastBeatIndex * _beatInterval) + SongStartTime;

            // Skip beats that have already passed
            if (hitTime < songTime) continue;

            CalcBombPattern(hitTime);
        }
    }

    private void CalcBombPattern(float hitTime)
    {
        // bomb制御用に適用なIDを付与
        int bombId = GetNextBombId();

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
                            BombId = bombId,
                            PositionIndex = notePositionEnum.GetValue(),
                            HitTime = hitTime
                        });
                        selectedPosList.Add(notePositionEnum);
                    }

                    EnqueueBombCommands(bombCommandListModel);
                    break;
                }
            case FaraBombCreatePatternEnum.BombReset:
                {
                    EnqueueBombCommands(CreateResetPatternCommands(bombId, notePositionEnum, hitTime));
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
                            float lineHitTime = hitTime + (BombLineDiffBeat * j * _beatInterval);
                            bombCommandListModel.Add(new BombCommandModel
                            {
                                BombId = bombId,
                                PositionIndex = notePositionEnum.GetValue(),
                                HitTime = lineHitTime
                            });
                        }
                        selectedPosList.Add(notePositionEnum);
                    }

                    EnqueueBombCommands(bombCommandListModel);
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
