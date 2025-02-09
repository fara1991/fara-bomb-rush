using System;
using UnityEngine;

namespace FaraBombRush.Enums;

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

public static class NotePositionHelper
{
    private const float SpawnZ = 30.0f;
    private const float DefaultPlayerHeight = 140f;

    private static readonly Vector3[] PositionCoordinates =
    [
        new(-0.9f, 1.7f, SpawnZ), // TopLeft
        new(-0.3f, 1.7f, SpawnZ), // TopMiddleLeft
        new(0.3f, 1.7f, SpawnZ), // TopMiddleRight
        new(0.9f, 1.7f, SpawnZ), // TopRight
        new(-0.9f, 1.2f, SpawnZ), // CenterLeft
        new(0.9f, 1.2f, SpawnZ), // CenterRight
        new(-0.9f, 0.7f, SpawnZ), // BottomLeft
        new(-0.3f, 0.7f, SpawnZ), // BottomMiddleLeft
        new(0.3f, 0.7f, SpawnZ), // BottomMiddleRight
        new(0.9f, 0.7f, SpawnZ) // BottomRight
    ];

    public static Vector3 GetNotePosition(this NotePositionEnum position, float spawnDelayTime,
        float playerHeight = DefaultPlayerHeight)
    {
        var pos = PositionCoordinates[(int)position];
        Plugin.Logger.Debug($"Get note position: {pos}");
        pos.z += spawnDelayTime;
        return pos;
    }

    public static int GetPositionIndex(this NotePositionEnum position)
    {
        return (int)position;
    }

    public static bool IsTopPosition(this NotePositionEnum position)
    {
        return position.ToString().Contains("Top");
    }

    public static bool IsCenterPosition(this NotePositionEnum position)
    {
        return position.ToString().Contains("Center");
    }

    public static bool IsBottomPosition(this NotePositionEnum position)
    {
        return position.ToString().Contains("Bottom");
    }

    public static NotePositionEnum ToNotePosition(int index)
    {
        if (!Enum.IsDefined(typeof(NotePositionEnum), index))
        {
            Plugin.Logger.Debug("Enum Outbound index");
            throw new NotImplementedException();
        }

        return (NotePositionEnum)index;
    }
}