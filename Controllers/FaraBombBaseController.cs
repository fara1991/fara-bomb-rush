using FaraBombRush.Configs;
using FaraBombRush.Enums;
using UnityEngine;
using Zenject;
using static FaraBombRush.Enums.NoteLineCustomEnum;

namespace FaraBombRush.Controllers;

public class FaraBombBaseController: MonoBehaviour
{
    internal PluginConfig _config;
    internal const float BombLineDiffBeat = 1.0f;
    internal int _bombId;
    

    [Inject]
    private void Construct(PluginConfig config)
    {
        _config = config;
    }

    internal void SearchStartAndEndPosition(int posInt, out int start, out int end)
    {
        var e = (NoteLineCustomEnum) (posInt - 1);
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