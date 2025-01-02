using System;
using UnityEngine;

namespace FaraBombRush.Enums;

public class NoteLineCustomEnum
{
    public enum NotePositionEnum
    {
        TopLeft,
        TopMiddleLeft,
        TopMiddleRight,
        TopRight,
        CenterLeft,
        CenterRight,
        BottomLeft,
        BottomMiddleLeft,
        BottomMiddleRight,
        BottomRight
    }

    private const float SpawnZ = 30.0f;

    // Enum
    public static readonly NoteLineCustomEnum TopLeft = new(-0.9f, 1.7f, SpawnZ, NotePositionEnum.TopLeft);
    public static readonly NoteLineCustomEnum TopMiddleLeft = new(-0.3f, 1.7f, SpawnZ, NotePositionEnum.TopMiddleLeft);
    public static readonly NoteLineCustomEnum TopMiddleRight = new(0.3f, 1.7f, SpawnZ, NotePositionEnum.TopMiddleRight);
    public static readonly NoteLineCustomEnum TopRight = new(0.9f, 1.7f, SpawnZ, NotePositionEnum.TopRight);
    public static readonly NoteLineCustomEnum CenterLeft = new(-0.9f, 1.2f, SpawnZ, NotePositionEnum.CenterLeft);
    public static readonly NoteLineCustomEnum CenterRight = new(0.9f, 1.2f, SpawnZ, NotePositionEnum.CenterRight);
    public static readonly NoteLineCustomEnum BottomLeft = new(-0.9f, 0.7f, SpawnZ, NotePositionEnum.BottomLeft);

    public static readonly NoteLineCustomEnum BottomMiddleLeft =
        new(-0.3f, 0.7f, SpawnZ, NotePositionEnum.BottomMiddleLeft);

    public static readonly NoteLineCustomEnum BottomMiddleRight =
        new(0.3f, 0.7f, SpawnZ, NotePositionEnum.BottomMiddleRight);

    public static readonly NoteLineCustomEnum BottomRight = new(0.9f, 0.7f, SpawnZ, NotePositionEnum.BottomRight);
    private readonly NotePositionEnum _notePositionEnum;
    private readonly int _playerHeight;
    private readonly Vector3 _position;

    private NoteLineCustomEnum(float x, float y, float z, NotePositionEnum notePositionEnum, int playerHeight = 140)
    {
        _position = new Vector3(x, y, z);
        _notePositionEnum = notePositionEnum;
        _playerHeight = playerHeight;
    }

    public int GetPositionIndex()
    {
        return (int) _notePositionEnum;
    }

    private string GetPositionString()
    {
        return _notePositionEnum.ToString();
    }

    public Vector3 GetNotePosition(float spawnDelayTime)
    {
        var baseY = _playerHeight / 100.0f;
        var pos = _position;
        pos.z += spawnDelayTime;
        return pos;
    }

    public bool IsTopPosition()
    {
        return GetPositionString().Contains("Top");
    }

    public bool IsCenterPosition()
    {
        return GetPositionString().Contains("Center");
    }

    public bool IsBottomPosition()
    {
        return GetPositionString().Contains("Bottom");
    }

    public static explicit operator NoteLineCustomEnum(int index)
    {
        switch (index)
        {
            case (int) NotePositionEnum.TopLeft: return TopLeft;
            case (int) NotePositionEnum.TopMiddleLeft: return TopMiddleLeft;
            case (int) NotePositionEnum.TopMiddleRight: return TopMiddleRight;
            case (int) NotePositionEnum.TopRight: return TopRight;
            case (int) NotePositionEnum.CenterLeft: return CenterLeft;
            case (int) NotePositionEnum.CenterRight: return CenterRight;
            case (int) NotePositionEnum.BottomLeft: return BottomLeft;
            case (int) NotePositionEnum.BottomMiddleLeft: return BottomMiddleLeft;
            case (int) NotePositionEnum.BottomMiddleRight: return BottomMiddleRight;
            case (int) NotePositionEnum.BottomRight: return BottomRight;
            default:
            {
                Plugin.Logger.Debug("Enum Outbound index");
                throw new NotImplementedException();
            }
        }
    }
}