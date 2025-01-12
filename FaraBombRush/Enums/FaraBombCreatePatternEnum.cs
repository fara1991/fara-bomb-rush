using System.Collections.Generic;

namespace FaraBombRush.Enums;

public enum FaraBombCreatePatternEnum
{
    BombSingle,
    BombDouble,
    BombTriple,
    BombReset,
    BombLineSingle,
    BombLineDouble,
    BombLineTriple
}

public static class FaraBombCreatePatternExternal
{
    public static Dictionary<FaraBombCreatePatternEnum, float> GetPattern(int playerLevel)
    {
        switch (playerLevel)
        {
            case (int) FaraBombLevelEnum.PlayerLevel.Beginner:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 50f},
                    {FaraBombCreatePatternEnum.BombDouble, 30f},
                    {FaraBombCreatePatternEnum.BombTriple, 10f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 10f},
                };
            case (int) FaraBombLevelEnum.PlayerLevel.Easy:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 40f},
                    {FaraBombCreatePatternEnum.BombDouble, 30f},
                    {FaraBombCreatePatternEnum.BombTriple, 10f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 10f},
                    {FaraBombCreatePatternEnum.BombReset, 10f}
                };
            case (int) FaraBombLevelEnum.PlayerLevel.Normal:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 20f},
                    {FaraBombCreatePatternEnum.BombDouble, 20f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 20f},
                    {FaraBombCreatePatternEnum.BombReset, 20f}
                };
            case (int) FaraBombLevelEnum.PlayerLevel.Hard:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 10f},
                    {FaraBombCreatePatternEnum.BombDouble, 30f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 20f},
                    {FaraBombCreatePatternEnum.BombReset, 20f}
                };
            case (int) FaraBombLevelEnum.PlayerLevel.Expert:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombDouble, 20f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 20f},
                    {FaraBombCreatePatternEnum.BombLineDouble, 20f},
                    {FaraBombCreatePatternEnum.BombLineTriple, 20f}
                };
            case (int) FaraBombLevelEnum.PlayerLevel.ExpertPlus:
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