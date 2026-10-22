using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using CP_SDK.Chat.Interfaces;
using FaraBombRush.Enums;
using FaraBombRush.Models;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers.GameModes;

internal class FaraBombInteractiveModeController : FaraBombGameModeBaseController
{
    private const string BaseCommand = "!bomb";
    private const string LineCommand = "!bombline";
    private const string ResetCommand = "!bombreset";
    private const float SpawnAheadBeats = 4.5f;

    private readonly ConcurrentQueue<string> _commands = new();

    private void Start()
    {
        try
        {
            CP_SDK.Chat.Service.Acquire();
            CP_SDK.Chat.Service.Multiplexer.OnTextMessageReceived += OnTextMessageReceived;
            Plugin.Logger.Info("ChatPlexSDK chat service acquired");
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error($"Chat service initialization failed: {ex.Message}");
            enabled = false;
        }
    }

    private void OnDestroy()
    {
        try
        {
            CP_SDK.Chat.Service.Multiplexer.OnTextMessageReceived -= OnTextMessageReceived;
            CP_SDK.Chat.Service.Release();
        }
        catch (Exception ex)
        {
            Plugin.Logger.Warn($"Chat service cleanup failed: {ex.Message}");
        }
    }

    private void OnTextMessageReceived(IChatService service, IChatMessage message)
    {
        if (service.DisplayName == "Twitch" && CheckCommand(message.Message))
            _commands.Enqueue(message.Message);
    }

    private void Update()
    {
        BombPush();
    }

    private bool CheckCommand(string chat)
    {
        var chatSplit = chat.Split(' ');
        if (chatSplit.Length > 2) return false;
        if (!chat.Contains(BaseCommand) && !chat.Contains(LineCommand) && !chat.Contains(ResetCommand))
            return false;
        if (chatSplit.Length != 2 || !int.TryParse(chatSplit[1], out var pos)) return true;
        return 1 <= pos && pos <= NotePositionEnum.BottomRight.GetValue() + 1;
    }

    protected override void BombPush()
    {
        while (_commands.TryDequeue(out var command))
            CommandAnalysis(command);
    }

    private void CommandAnalysis(string command)
    {
        int bombId = GetNextBombId();

        var parts = command.Split(' ');
        var posIndex = parts.Length == 2
            ? parts[1]
            : Random.Range(1, NotePositionEnumList.Count).ToString();

        if (!int.TryParse(posIndex, out var posInt)) return;

        var notePositionEnum = NotePositionEnumHelper.FromValue(posInt);
        var hitTime = GetCurrentSongTime() + (60f / Bpm) * SpawnAheadBeats;

        if (command.Contains(LineCommand))
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
        }
        else if (command.Contains(ResetCommand))
        {
            EnqueueBombCommands(CreateResetPatternCommands(bombId, notePositionEnum, hitTime));
        }
        else if (command.Contains(BaseCommand))
        {
            EnqueueBombCommand(bombId, notePositionEnum.GetValue(), hitTime);
        }
    }
}
