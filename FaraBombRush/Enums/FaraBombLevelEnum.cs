using System;

namespace FaraBombRush.Enums;

public enum FaraBombLevelEnum
{
    Beginner,
    Easy,
    Normal,
    Hard,
    Expert,
    ExpertPlus,
    Lawless
}

public static class FaraBombLevelEnumHelper
{
    public static int GetLevelIndex(string levelName)
    {
        if (Enum.TryParse(levelName, out FaraBombLevelEnum levelEnum)) return (int) levelEnum;

        Plugin.Logger.Error($"Unknown player level: {levelName}");
        return (int) FaraBombLevelEnum.Normal;
    }

    public static FaraBombLevelEnum GetLevelEnum(float pp)
    {
        return pp switch
        {
            < 2000 => FaraBombLevelEnum.Beginner,
            < 4000 => FaraBombLevelEnum.Easy,
            < 6000 => FaraBombLevelEnum.Normal,
            < 8000 => FaraBombLevelEnum.Hard,
            < 10000 => FaraBombLevelEnum.Expert,
            < 12000 => FaraBombLevelEnum.ExpertPlus,
            _ => FaraBombLevelEnum.Lawless
        };
    }
}