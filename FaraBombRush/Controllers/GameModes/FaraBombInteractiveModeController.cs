using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using FaraBombRush.Enums;
using FaraBombRush.Models;
using FaraBombRush.Services;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers.GameModes;

internal class FaraBombInteractiveModeController : FaraBombGameModeBaseController
{
    private const float SpawnAheadBeats = 4.5f;

    private readonly ConcurrentQueue<BombChatCommand> _commands = new();
    private bool _chatAcquired;

    private void Start()
    {
        try
        {
            ChatPlexBridge.Acquire(OnChatMessage);
            _chatAcquired = true;
            Plugin.Logger.Info("ChatPlexSDK chat service acquired");
        }
        catch (Exception ex)
        {
            // ChatPlexSDK（BeatSaberPlus）が無い場合もここに来る
            Plugin.Logger.Error($"Chat service initialization failed: {ex.Message}");
            enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (!_chatAcquired) return;
        _chatAcquired = false;

        try
        {
            ChatPlexBridge.Release();
        }
        catch (Exception ex)
        {
            Plugin.Logger.Warn($"Chat service cleanup failed: {ex.Message}");
        }
    }

    private void OnChatMessage(string displayName, string message)
    {
        if (displayName != "Twitch") return;
        if (BombChatCommandParser.TryParse(message, out var command))
            _commands.Enqueue(command);
    }

    private void Update()
    {
        BombPush();
    }

    protected override void BombPush()
    {
        while (_commands.TryDequeue(out var command))
            CommandAnalysis(command);
    }

    private void CommandAnalysis(BombChatCommand command)
    {
        int bombId = GetNextBombId();

        // 位置の省略時は、実在する 10 か所から一様に選ぶ（1〜12 の番号を経由すると偏るため）
        var notePositionEnum = command.Position.HasValue
            ? NotePositionEnumHelper.FromValue(command.Position.Value)
            : NotePositionEnumList[Random.Range(0, NotePositionEnumList.Count)];

        var hitTime = GetCurrentSongTime() + (60f / Bpm) * SpawnAheadBeats;

        switch (command.Kind)
        {
            case BombChatCommandKind.Line:
            {
                var commands = new List<BombCommandModel>();
                for (var i = 0; i < 5; i++)
                {
                    commands.Add(new BombCommandModel
                    {
                        BombId = bombId,
                        PositionIndex = notePositionEnum.GetValue(),
                        HitTime = hitTime + (BombLineDiffBeat * i * (60f / Bpm))
                    });
                }
                EnqueueBombCommands(commands);
                break;
            }
            case BombChatCommandKind.Reset:
                EnqueueBombCommands(CreateResetPatternCommands(bombId, notePositionEnum, hitTime));
                break;
            case BombChatCommandKind.Bomb:
                EnqueueBombCommand(bombId, notePositionEnum.GetValue(), hitTime);
                break;
        }
    }
}
