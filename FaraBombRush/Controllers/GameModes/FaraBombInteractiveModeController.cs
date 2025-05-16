using System.Collections.Generic;
using System.Linq;
// using ChatCore;
// using ChatCore.Interfaces;
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
    private readonly List<string> _commands = [];

    private void Start()
    {
        var catCoreInstance = CatCoreInstance.Create();
        var chatCoreInstance = ChatCoreInstance.Create();
        catCoreInstance.RunAllServices().GetTwitchPlatformService().OnTextMessageReceived += CatCoreOnTextMessageReceived;
        chatCoreInstance.RunAllServices().GetTwitchService().OnTextMessageReceived += ChatCoreOnTextMessageReceived;
    }

    private void Update()
    {
        BombPush();
    }

    private void CatCoreOnTextMessageReceived(ITwitchService service, TwitchMessage message)
    {
        if (service.DefaultChannel.Name != "" && CheckCommand(message.Message)) _commands.Add(message.Message);
    }

    private void ChatCoreOnTextMessageReceived(IChatService service, IChatMessage message)
    {
    if (service.DisplayName == "Twitch" && CheckCommand(message.Message)) _commands.Add(message.Message);
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
        foreach (var command in _commands.ToList())
        {
            CommandAnalysis(command);
            _commands.Remove(command);
        }
    }

    private void CommandAnalysis(string command)
    {
        // bomb制御用に適用なIDを付与
        BombId = BombId >= int.MaxValue ? 1 : BombId + 1;

        var replaceChatList = command.Split(' ');
        var posIndex = replaceChatList.Length == 2
            ? replaceChatList[1]
            : Random.Range(1, NotePositionEnumList.Count).ToString();

        if (!int.TryParse(posIndex, out var posInt)) return;
        var notePositionEnum = NotePositionEnumHelper.FromValue(posInt);

        var startPos = 0;
        var endPos = Config.BombLineCount;
        var bombCommandListModel = new List<BombCommandModel>();
        if (command.Contains(LineCommand))
        {
            for (var i = startPos; i < endPos; i++)
                bombCommandListModel.Add(new BombCommandModel
                {
                    BombId = BombId,
                    SpawnDelayTime = BombLineDiffBeat * i,
                    PositionIndex = notePositionEnum.GetValue()
                });
        }
        else if (command.Contains(ResetCommand))
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
        }
        else if (command.Contains(BaseCommand))
        {
            bombCommandListModel.Add(new BombCommandModel
            {
                BombId = BombId,
                SpawnDelayTime = 0,
                PositionIndex = notePositionEnum.GetValue()
            });
        }

        FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
    }
}