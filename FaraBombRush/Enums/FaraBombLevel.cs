using System;

namespace FaraBombRush.Enums;

public enum FaraBombLevel
{
    Beginner,
    Easy,
    Normal,
    Hard,
    Expert,
    ExpertPlus,
    Ultimate,
    Chaos
}

public static class FaraBombLevelExternal
{
    public static FaraBombLevel GetLevel(float pp)
    {
        return pp switch
        {
            < 2000 => FaraBombLevel.Beginner,
            < 4000 => FaraBombLevel.Easy,
            < 6000 => FaraBombLevel.Normal,
            < 8000 => FaraBombLevel.Hard,
            < 10000 => FaraBombLevel.Expert,
            < 12000 => FaraBombLevel.ExpertPlus,
            < 14000 => FaraBombLevel.Ultimate,
            _ => FaraBombLevel.Chaos
        };
    }

    public static FaraBombLevel GetLevel(string level)
    {
        return Enum.TryParse<FaraBombLevel>(level, out var result) ? result : throw new ArgumentException($"Invalid level: {level}");
    }
}