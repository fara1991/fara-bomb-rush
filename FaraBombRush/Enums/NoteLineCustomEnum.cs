using System;
using UnityEngine;

namespace FaraBombRush.Enums;

public class NoteLineCustomEnum
{
    public enum NotePosition
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
    public static readonly NoteLineCustomEnum TopLeft = new(-0.9f, 1.7f, SpawnZ, NotePosition.TopLeft);
    public static readonly NoteLineCustomEnum TopMiddleLeft = new(-0.3f, 1.7f, SpawnZ, NotePosition.TopMiddleLeft);
    public static readonly NoteLineCustomEnum TopMiddleRight = new(0.3f, 1.7f, SpawnZ, NotePosition.TopMiddleRight);
    public static readonly NoteLineCustomEnum TopRight = new(0.9f, 1.7f, SpawnZ, NotePosition.TopRight);
    public static readonly NoteLineCustomEnum CenterLeft = new(-0.9f, 1.2f, SpawnZ, NotePosition.CenterLeft);
    public static readonly NoteLineCustomEnum CenterRight = new(0.9f, 1.2f, SpawnZ, NotePosition.CenterRight);
    public static readonly NoteLineCustomEnum BottomLeft = new(-0.9f, 0.7f, SpawnZ, NotePosition.BottomLeft);

    public static readonly NoteLineCustomEnum BottomMiddleLeft =
        new(-0.3f, 0.7f, SpawnZ, NotePosition.BottomMiddleLeft);

    public static readonly NoteLineCustomEnum BottomMiddleRight =
        new(0.3f, 0.7f, SpawnZ, NotePosition.BottomMiddleRight);

    public static readonly NoteLineCustomEnum BottomRight = new(0.9f, 0.7f, SpawnZ, NotePosition.BottomRight);
    private readonly NotePosition _notePosition;
    private readonly int _playerHeight;
    private readonly Vector3 _position;

    private NoteLineCustomEnum(float x, float y, float z, NotePosition notePosition, int playerHeight = 140)
    {
        _position = new Vector3(x, y, z);
        _notePosition = notePosition;
        _playerHeight = playerHeight;
    }

    public int GetPositionIndex()
    {
        return (int)_notePosition;
    }

    private string GetPositionString()
    {
        return _notePosition.ToString();
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
            case (int)NotePosition.TopLeft: return TopLeft;
            case (int)NotePosition.TopMiddleLeft: return TopMiddleLeft;
            case (int)NotePosition.TopMiddleRight: return TopMiddleRight;
            case (int)NotePosition.TopRight: return TopRight;
            case (int)NotePosition.CenterLeft: return CenterLeft;
            case (int)NotePosition.CenterRight: return CenterRight;
            case (int)NotePosition.BottomLeft: return BottomLeft;
            case (int)NotePosition.BottomMiddleLeft: return BottomMiddleLeft;
            case (int)NotePosition.BottomMiddleRight: return BottomMiddleRight;
            case (int)NotePosition.BottomRight: return BottomRight;
            default:
                {
                    Plugin.Logger.Debug("Enum Outbound index");
                    throw new NotImplementedException();
                }
        }
    }
}