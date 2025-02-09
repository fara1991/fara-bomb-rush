using System;
using System.Collections.Generic;

namespace FaraBombRush.Enums;

internal enum FaraBombCreatePatternEnum
{
    BombSingle,
    BombDouble,
    BombTriple,
    BombReset,
    BombLineSingle,
    BombLineDouble,
    BombLineTriple
}

internal static class FaraBombCreatePatternEnumHelper
{
    internal static int GetPatternIndex(string patternName)
    {
        if (Enum.TryParse(patternName, out FaraBombCreatePatternEnum pattern)) return (int) pattern;

        Plugin.Logger.Error($"Unknown patternName: {patternName}");
        return (int) FaraBombCreatePatternEnum.BombSingle;
    }

    internal static Dictionary<FaraBombCreatePatternEnum, float> GetPattern(int playerLevel)
    {
        switch (playerLevel)
        {
            case (int) FaraBombLevelEnum.Beginner:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 50f},
                    {FaraBombCreatePatternEnum.BombDouble, 30f},
                    {FaraBombCreatePatternEnum.BombTriple, 10f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 10f}
                };
            case (int) FaraBombLevelEnum.Easy:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 40f},
                    {FaraBombCreatePatternEnum.BombDouble, 30f},
                    {FaraBombCreatePatternEnum.BombTriple, 10f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 10f},
                    {FaraBombCreatePatternEnum.BombReset, 10f}
                };
            case (int) FaraBombLevelEnum.Normal:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 20f},
                    {FaraBombCreatePatternEnum.BombDouble, 20f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 20f},
                    {FaraBombCreatePatternEnum.BombReset, 20f}
                };
            case (int) FaraBombLevelEnum.Hard:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 10f},
                    {FaraBombCreatePatternEnum.BombDouble, 30f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 20f},
                    {FaraBombCreatePatternEnum.BombReset, 20f}
                };
            case (int) FaraBombLevelEnum.Expert:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombDouble, 20f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 20f},
                    {FaraBombCreatePatternEnum.BombLineDouble, 20f},
                    {FaraBombCreatePatternEnum.BombLineTriple, 20f}
                };
            case (int) FaraBombLevelEnum.ExpertPlus:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombDouble, 20f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineDouble, 20f},
                    {FaraBombCreatePatternEnum.BombLineTriple, 20f},
                    {FaraBombCreatePatternEnum.BombReset, 20f}
                };
            default:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineTriple, 40f},
                    {FaraBombCreatePatternEnum.BombReset, 40f}
                };
        }
    }
}