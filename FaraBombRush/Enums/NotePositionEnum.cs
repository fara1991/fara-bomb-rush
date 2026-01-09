using System;
using System.Linq;
using UnityEngine;

namespace FaraBombRush.Enums;

public enum NotePositionEnum
{
    TopLeft = 0,
    TopMiddleLeft = 1,
    TopMiddleRight = 2,
    TopRight = 3,
    CenterLeft = 4,
    CenterRight = 7,
    BottomLeft = 8,
    BottomMiddleLeft = 9,
    BottomMiddleRight = 10,
    BottomRight = 11
}

public static class NotePositionEnumHelper
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

    public static int GetValue(this NotePositionEnum notePosition) => (int)notePosition;

    public static int GetIndex(this NotePositionEnum notePosition)
    {
        var values = Enum.GetValues(typeof(NotePositionEnum)).Cast<NotePositionEnum>().ToList();
        return values.IndexOf(notePosition);
    }

    public static NotePositionEnum RandomPositionEnum()
    {
        var values = Enum.GetValues(typeof(NotePositionEnum)).Cast<NotePositionEnum>().ToList();
        return values[UnityEngine.Random.Range(0, values.Count)];
    }

    public static Vector3 GetNotePosition(this NotePositionEnum position, float spawnDelayTime,
        float playerHeight = DefaultPlayerHeight)
    {
        var pos = PositionCoordinates[position.GetIndex()];
        pos.z += spawnDelayTime;
        return pos;
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

    public static NotePositionEnum FromValue(int value)
    {
        var values = Enum.GetValues(typeof(NotePositionEnum)).Cast<NotePositionEnum>().ToList();
        foreach (var notePositionValue in values.Where(notePositionValue => value == notePositionValue.GetValue() + 1))
        {
            return notePositionValue;
        }

        // 一致しないIndexはランダムに再計算
        return RandomPositionEnum();
    }
}