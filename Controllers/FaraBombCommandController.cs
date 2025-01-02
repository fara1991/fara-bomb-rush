using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using Zenject;
using static FaraBombRush.Enums.NoteLineCustomEnum;
using Random = UnityEngine.Random;

namespace FaraBombRush.Controllers;

public class FaraBombCommandController: FaraBombBaseController
{
    private const string BaseCommand = "!bomb";
    private const string LineCommand = "!bombline";
    private const string ResetCommand = "!bombreset";

    private readonly List<NotePositionEnum> _notePositionEnumList =
        Enum.GetValues(typeof(NotePositionEnum)).Cast<NotePositionEnum>().ToList();

    public bool CheckCommand(string chat)
    {
        var chatSplit = chat.Split(' ');
        if (0 == chatSplit.Length || chatSplit.Length > 2) return false;
        if (!chat.Contains(BaseCommand) && !chat.Contains(LineCommand) && !chat.Contains(ResetCommand))
            return false;
        if (chatSplit.Length != 2 || !int.TryParse(chatSplit[1], out var pos)) return true;
        return pos >= 1 && _notePositionEnumList.Count >= pos;
    }

    public void BombPush(string chat)
    {
        // bomb制御用に適用なIDを付与
        _bombId = _bombId >= int.MaxValue ? 1 : _bombId + 1;

        var replaceChatList = chat.Split(' ');
        var pos = replaceChatList.Length == 2
            ? replaceChatList[1]
            : Random.Range(1, _notePositionEnumList.Count).ToString();

        if (!int.TryParse(pos, out var posInt)) return;

        var bombCommandListModel = new List<BombCommandModel>();
        if (chat.Contains(LineCommand))
        {
            for (var i = 0; i < _config.BombLineCount; i++)
                bombCommandListModel.Add(new BombCommandModel
                {
                    BombId = _bombId,
                    SpawnDelayTime = BombLineDiffBeat * i,
                    PositionIndex = posInt - 1
                });
            FaraBombPoolManager.CommandQueue.Enqueue(bombCommandListModel);
        }
        else if (chat.Contains(ResetCommand))
        {
            SearchStartAndEndPosition(posInt, out var start, out var end);
            for (var i = start; i <= end; i++)
                bombCommandListModel.Add(new BombCommandModel
                {
                    BombId = _bombId,
                    SpawnDelayTime = 0,
                    PositionIndex = i
                });
            FaraBombPoolManager.CommandQueue.Enqueue(bombCommandListModel);
        }
        else if (chat.Contains(BaseCommand))
        {
            bombCommandListModel.Add(new BombCommandModel
            {
                BombId = _bombId,
                SpawnDelayTime = 0,
                PositionIndex = posInt - 1
            });
            FaraBombPoolManager.CommandQueue.Enqueue(bombCommandListModel);
        }
    }
}