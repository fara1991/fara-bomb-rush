using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using UnityEngine;
using Zenject;

namespace FaraBombRush.Controllers.GameModes;

internal class FaraBombGameModeBaseController : MonoBehaviour
{
    protected const float BombLineDiffBeat = 1.0f;

    protected readonly List<NotePositionEnum> NotePositionEnumList =
        Enum.GetValues(typeof(NotePositionEnum)).Cast<NotePositionEnum>().ToList();

    protected int BombId;
    protected PluginConfig Config;

    // Beat Saber timing and spawn system
    protected AudioTimeSyncController AudioTimeSyncController;
    protected BeatmapObjectSpawnController.InitData SpawnInitData;
    protected float Njs = 12f; // Note Jump Speed
    protected float Bpm = 120f;
    protected bool SongStarted = false;
    protected float SongStartTime = 0f;

    [Inject]
    private void Construct(
        PluginConfig config,
        AudioTimeSyncController audioTimeSyncController,
        [InjectOptional] BeatmapObjectSpawnController.InitData spawnInitData,
        [InjectOptional] IDifficultyBeatmap difficultyBeatmap)
    {
        Config = config;
        AudioTimeSyncController = audioTimeSyncController;
        SpawnInitData = spawnInitData;

        // Get NJS from spawn data
        if (spawnInitData != null)
        {
            Njs = spawnInitData.noteJumpMovementSpeed;
        }

        // Get BPM from beatmap
        if (difficultyBeatmap?.level != null)
        {
            float bpm = difficultyBeatmap.level.beatsPerMinute;
            Bpm = bpm > 0 ? bpm : 120f;
        }
    }

    protected virtual void BombPush()
    {
    }

    protected int GetNextBombId()
    {
        BombId = BombId >= int.MaxValue ? 1 : BombId + 1;
        return BombId;
    }

    protected void EnqueueBombCommand(int bombId, int positionIndex, float hitTime)
    {
        var command = new BombCommandModel
        {
            BombId = bombId,
            PositionIndex = positionIndex,
            HitTime = hitTime
        };
        FaraBombSystemManager.CommandQueue.Enqueue(new List<BombCommandModel> { command });
    }

    protected void EnqueueBombCommands(List<BombCommandModel> commands)
    {
        if (commands != null && commands.Count > 0)
        {
            FaraBombSystemManager.CommandQueue.Enqueue(commands);
        }
    }

    protected List<BombCommandModel> CreateResetPatternCommands(int bombId, NotePositionEnum centerPos, float hitTime)
    {
        var commands = new List<BombCommandModel>();
        int startPos, endPos;

        if (centerPos.IsTopPosition() || centerPos == NotePositionEnum.CenterLeft)
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
        {
            commands.Add(new BombCommandModel
            {
                BombId = bombId,
                PositionIndex = i,
                HitTime = hitTime
            });
        }
        return commands;
    }

    protected float GetCurrentSongTime()
    {
        return AudioTimeSyncController?.songTime ?? 0f;
    }

    protected bool IsSongPlaying()
    {
        return AudioTimeSyncController != null && AudioTimeSyncController.songTime > 0;
    }
}
