using System;
using System.Collections.Generic;

namespace FaraBombRush.Enums;

public static class FaraBombCreatePatternEnum
{
    public enum FaraBombCreatePattern
    {
        BombSingle,
        BombDouble,
        BombTriple,
        BombReset,
        BombLineSingle,
        BombLineDouble,
        BombLineTriple
    }

    public static int GetPatternIndex(string patternName)
    {
        if (Enum.TryParse(patternName, out FaraBombCreatePattern pattern))
        {
            return (int)pattern;
        }

        Plugin.Logger.Error($"Unknown patternName: {patternName}");
        return (int)FaraBombCreatePattern.BombSingle;
    }

    public static Dictionary<FaraBombCreatePattern, float> GetPattern(int playerLevel)
    {
        switch (playerLevel)
        {
            case (int)FaraBombLevelEnum.PlayerLevel.Beginner:
                return new Dictionary<FaraBombCreatePattern, float>
                {
                    {FaraBombCreatePattern.BombSingle, 50f},
                    {FaraBombCreatePattern.BombDouble, 30f},
                    {FaraBombCreatePattern.BombTriple, 10f},
                    {FaraBombCreatePattern.BombLineSingle, 10f},
                };
            case (int)FaraBombLevelEnum.PlayerLevel.Easy:
                return new Dictionary<FaraBombCreatePattern, float>
                {
                    {FaraBombCreatePattern.BombSingle, 40f},
                    {FaraBombCreatePattern.BombDouble, 30f},
                    {FaraBombCreatePattern.BombTriple, 10f},
                    {FaraBombCreatePattern.BombLineSingle, 10f},
                    {FaraBombCreatePattern.BombReset, 10f}
                };
            case (int)FaraBombLevelEnum.PlayerLevel.Normal:
                return new Dictionary<FaraBombCreatePattern, float>
                {
                    {FaraBombCreatePattern.BombSingle, 20f},
                    {FaraBombCreatePattern.BombDouble, 20f},
                    {FaraBombCreatePattern.BombTriple, 20f},
                    {FaraBombCreatePattern.BombLineSingle, 20f},
                    {FaraBombCreatePattern.BombReset, 20f}
                };
            case (int)FaraBombLevelEnum.PlayerLevel.Hard:
                return new Dictionary<FaraBombCreatePattern, float>
                {
                    {FaraBombCreatePattern.BombSingle, 10f},
                    {FaraBombCreatePattern.BombDouble, 30f},
                    {FaraBombCreatePattern.BombTriple, 20f},
                    {FaraBombCreatePattern.BombLineSingle, 20f},
                    {FaraBombCreatePattern.BombReset, 20f}
                };
            case (int)FaraBombLevelEnum.PlayerLevel.Expert:
                return new Dictionary<FaraBombCreatePattern, float>
                {
                    {FaraBombCreatePattern.BombDouble, 20f},
                    {FaraBombCreatePattern.BombTriple, 20f},
                    {FaraBombCreatePattern.BombLineSingle, 20f},
                    {FaraBombCreatePattern.BombLineDouble, 20f},
                    {FaraBombCreatePattern.BombLineTriple, 20f}
                };
            case (int)FaraBombLevelEnum.PlayerLevel.ExpertPlus:
                return new Dictionary<FaraBombCreatePattern, float>
                {
                    {FaraBombCreatePattern.BombDouble, 20f},
                    {FaraBombCreatePattern.BombTriple, 20f},
                    {FaraBombCreatePattern.BombLineDouble, 20f},
                    {FaraBombCreatePattern.BombLineTriple, 20f},
                    {FaraBombCreatePattern.BombReset, 20f}
                };
            default:
                return new Dictionary<FaraBombCreatePattern, float>
                {
                    {FaraBombCreatePattern.BombTriple, 20f},
                    {FaraBombCreatePattern.BombLineTriple, 40f},
                    {FaraBombCreatePattern.BombReset, 40f}
                };
        }
    }
}