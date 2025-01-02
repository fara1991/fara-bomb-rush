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
    public static Dictionary<FaraBombCreatePatternEnum, float> GetPattern(FaraBombLevel level)
    {
        switch (level)
        {
            case FaraBombLevel.Beginner:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 50f},
                    {FaraBombCreatePatternEnum.BombDouble, 30f},
                    {FaraBombCreatePatternEnum.BombTriple, 10f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 10f},
                };
            case FaraBombLevel.Easy:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 40f},
                    {FaraBombCreatePatternEnum.BombDouble, 30f},
                    {FaraBombCreatePatternEnum.BombTriple, 10f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 10f},
                    {FaraBombCreatePatternEnum.BombReset, 10f}
                };
            case FaraBombLevel.Normal:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 20f},
                    {FaraBombCreatePatternEnum.BombDouble, 20f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 20f},
                    {FaraBombCreatePatternEnum.BombReset, 20f}
                };
            case FaraBombLevel.Hard:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombSingle, 10f},
                    {FaraBombCreatePatternEnum.BombDouble, 30f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 20f},
                    {FaraBombCreatePatternEnum.BombReset, 20f}
                };
            case FaraBombLevel.Expert:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombDouble, 20f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineSingle, 20f},
                    {FaraBombCreatePatternEnum.BombLineDouble, 20f},
                    {FaraBombCreatePatternEnum.BombLineTriple, 20f}
                };
            case FaraBombLevel.ExpertPlus:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombDouble, 20f},
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineDouble, 20f},
                    {FaraBombCreatePatternEnum.BombLineTriple, 20f},
                    {FaraBombCreatePatternEnum.BombReset, 20f}
                };
            case FaraBombLevel.Ultimate:
                return new Dictionary<FaraBombCreatePatternEnum, float>
                {
                    {FaraBombCreatePatternEnum.BombTriple, 20f},
                    {FaraBombCreatePatternEnum.BombLineDouble, 40f},
                    {FaraBombCreatePatternEnum.BombLineTriple, 30f},
                    {FaraBombCreatePatternEnum.BombReset, 10f}
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