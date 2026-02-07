using System.Collections.Concurrent;
using System.Collections.Generic;
using CatCore;
using CatCore.Models.Twitch.IRC;
using CatCore.Services.Twitch.Interfaces;
using ChatCore;
using ChatCore.Interfaces;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers.GameModes;

internal class FaraBombInteractiveModeController : FaraBombGameModeBaseController
{
    private const string BaseCommand = "!bomb";
    private const string LineCommand = "!bombline";
    private const string ResetCommand = "!bombreset";
    private readonly ConcurrentQueue<string> _commands = new();
    private ITwitchService _twitchPlatformService;
    private IChatService _chatCoreService;

    private void Start()
    {
        var catCoreInstance = CatCoreInstance.Create();
        var chatCoreInstance = ChatCoreInstance.Create();
        _twitchPlatformService = catCoreInstance.RunAllServices().GetTwitchPlatformService();
        _twitchPlatformService.OnTextMessageReceived += CatCoreOnTextMessageReceived;
        _chatCoreService = chatCoreInstance.RunAllServices().GetTwitchService();
        _chatCoreService.OnTextMessageReceived += ChatCoreOnTextMessageReceived;
    }

    private void OnDestroy()
    {
        if (_twitchPlatformService != null)
            _twitchPlatformService.OnTextMessageReceived -= CatCoreOnTextMessageReceived;
        if (_chatCoreService != null)
            _chatCoreService.OnTextMessageReceived -= ChatCoreOnTextMessageReceived;
    }

    private void Update()
    {
        BombPush();
    }

    private void CatCoreOnTextMessageReceived(ITwitchService service, TwitchMessage message)
    {
        if (service.DefaultChannel.Name != "" && CheckCommand(message.Message)) _commands.Enqueue(message.Message);
    }

    private void ChatCoreOnTextMessageReceived(IChatService service, IChatMessage message)
    {
        if (service.DisplayName == "Twitch" && CheckCommand(message.Message)) _commands.Enqueue(message.Message);
    }

    private bool CheckCommand(string chat)
    {
        var chatSplit = chat.Split(' ');
        if (chatSplit.Length is 0 or > 2) return false;
        if (!chat.Contains(BaseCommand) && !chat.Contains(LineCommand) && !chat.Contains(ResetCommand))
            return false;
        if (chatSplit.Length != 2 || !int.TryParse(chatSplit[1], out var pos)) return true;
        return 1 <= pos && pos <= NotePositionEnum.BottomRight.GetValue() + 1;
    }

    protected override void BombPush()
    {
        while (_commands.TryDequeue(out var command))
        {
            CommandAnalysis(command);
        }
    }

    private void CommandAnalysis(string command)
    {
        // bomb制御用に適用なIDを付与
        int bombId = GetNextBombId();

        var replaceChatList = command.Split(' ');
        var posIndex = replaceChatList.Length == 2
            ? replaceChatList[1]
            : Random.Range(1, NotePositionEnumList.Count).ToString();

        if (!int.TryParse(posIndex, out var posInt)) return;
        var notePositionEnum = NotePositionEnumHelper.FromValue(posInt);

        var hitTime = GetCurrentSongTime() + (60f / Bpm) * 4.5f; // Spawn slightly ahead of the spawn timing (4 beats)

        if (command.Contains(LineCommand))
        {
            var bombCommandListModel = new List<BombCommandModel>();
            for (var i = 0; i < Config.BombLineCount; i++)
            {
                bombCommandListModel.Add(new BombCommandModel
                {
                    BombId = bombId,
                    PositionIndex = notePositionEnum.GetValue(),
                    HitTime = hitTime + (BombLineDiffBeat * i * (60f / Bpm))
                });
            }
            EnqueueBombCommands(bombCommandListModel);
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