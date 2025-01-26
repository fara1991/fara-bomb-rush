using System;
using System.Collections.Generic;
using System.Linq;
using ChatCore;
using ChatCore.Interfaces;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using static FaraBombRush.Enums.NoteLineCustomEnum;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers.GameModes;

public class FaraBombInteractiveModeController : FaraBombGameModeBase
{
    private const string BaseCommand = "!bomb";
    private const string LineCommand = "!bombline";
    private const string ResetCommand = "!bombreset";
    private readonly List<string> _commands = [];

    private void Start()
    {
        var chatCoreInstance = ChatCoreInstance.Create();
        chatCoreInstance.RunAllServices().GetTwitchService().OnTextMessageReceived += ChatCoreOnTextMessageReceived;
    }

    private void Update()
    {
        BombPush();
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
        return pos >= 1 && NotePositionEnumList.Count >= pos;
    }

    public void SendCommand(string command)
    {
        _commands.Add(command);
    }

    protected override void BombPush()
    {
        foreach (var command in _commands.ToList())
        {
            CommandAnalysis(command);
            _commands.Remove(command);
        }
    }

    private void CommandAnalysis(string chat)
    {
        // bomb制御用に適用なIDを付与
        BombId = BombId >= int.MaxValue ? 1 : BombId + 1;

        var replaceChatList = chat.Split(' ');
        var pos = replaceChatList.Length == 2
            ? replaceChatList[1]
            : Random.Range(1, NotePositionEnumList.Count).ToString();

        if (!int.TryParse(pos, out var posInt)) return;

        var bombCommandListModel = new List<BombCommandModel>();
        Plugin.Logger.Debug(Config.ToString());
        if (chat.Contains(LineCommand))
        {
            for (var i = 0; i < Config.BombLineCount; i++)
                bombCommandListModel.Add(new BombCommandModel
                {
                    BombId = BombId,
                    SpawnDelayTime = BombLineDiffBeat * i,
                    PositionIndex = posInt - 1
                });
        }
        else if (chat.Contains(ResetCommand))
        {
            SearchStartAndEndPosition(posInt, out var start, out var end);
            for (var i = start; i <= end; i++)
                bombCommandListModel.Add(new BombCommandModel
                {
                    BombId = BombId,
                    SpawnDelayTime = 0,
                    PositionIndex = i
                });
        }
        else if (chat.Contains(BaseCommand))
        {
            bombCommandListModel.Add(new BombCommandModel
            {
                BombId = BombId,
                SpawnDelayTime = 0,
                PositionIndex = posInt - 1
            });
        }

        FaraBombSystemManager.CommandQueue.Enqueue(bombCommandListModel);
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