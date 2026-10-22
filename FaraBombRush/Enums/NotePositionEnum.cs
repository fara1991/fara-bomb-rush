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
    private const float DefaultPlayerHeight = 140f;

    /// <summary>チャットコマンドで指定できる位置番号の最小値。</summary>
    public const int MinPositionNumber = 1;

    /// <summary>チャットコマンドで指定できる位置番号の最大値（ノーツの 12 か所から中央 2 か所を除いた 10 か所）。</summary>
    public const int MaxPositionNumber = 10;

    /// <summary>位置番号 1〜10 に対応する位置（インデックス = 番号 - 1）。上段 1〜4、中段 5（左）と 6（右）、下段 7〜10。</summary>
    private static readonly NotePositionEnum[] PositionByNumber =
    [
        NotePositionEnum.TopLeft,
        NotePositionEnum.TopMiddleLeft,
        NotePositionEnum.TopMiddleRight,
        NotePositionEnum.TopRight,
        NotePositionEnum.CenterLeft,
        NotePositionEnum.CenterRight,
        NotePositionEnum.BottomLeft,
        NotePositionEnum.BottomMiddleLeft,
        NotePositionEnum.BottomMiddleRight,
        NotePositionEnum.BottomRight
    ];

    private static readonly Vector2[] PositionCoordinates =
    [
        new(-0.9f, 1.7f), // TopLeft
        new(-0.3f, 1.7f), // TopMiddleLeft
        new(0.3f, 1.7f), // TopMiddleRight
        new(0.9f, 1.7f), // TopRight
        new(-0.9f, 1.2f), // CenterLeft
        new(0.9f, 1.2f), // CenterRight
        new(-0.9f, 0.7f), // BottomLeft
        new(-0.3f, 0.7f), // BottomMiddleLeft
        new(0.3f, 0.7f), // BottomMiddleRight
        new(0.9f, 0.7f) // BottomRight
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

    public static Vector3 GetNotePosition(this NotePositionEnum position, float playerHeight = DefaultPlayerHeight)
    {
        var pos = PositionCoordinates[position.GetIndex()];
        return new Vector3(pos.x, pos.y, 0); // Z will be set by MoveController
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

        // 一致しないIndexはデフォルト値を返すか、エラーログを出してランダムに
        Plugin.Logger.Warn($"Invalid position value: {value}. Using random position.");
        return RandomPositionEnum();
    }

    /// <summary>位置番号（1〜10）が有効かどうか。</summary>
    public static bool IsValidPositionNumber(int number) => number >= MinPositionNumber && number <= MaxPositionNumber;

    /// <summary>位置番号（1〜10）を位置に変換する。範囲外は false を返し、position は既定値になる。</summary>
    public static bool TryFromNumber(int number, out NotePositionEnum position)
    {
        if (IsValidPositionNumber(number))
        {
            position = PositionByNumber[number - MinPositionNumber];
            return true;
        }

        position = default;
        return false;
    }

    public static NotePositionEnum FromIndex(int index)
    {
        if (Enum.IsDefined(typeof(NotePositionEnum), index))
        {
            return (NotePositionEnum)index;
        }

        Plugin.Logger.Warn($"Invalid position index: {index}. Using random position.");
        return RandomPositionEnum();
    }
}
