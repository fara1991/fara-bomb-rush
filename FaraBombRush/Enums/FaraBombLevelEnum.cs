using System;

namespace FaraBombRush.Enums;

public class FaraBombLevelEnum
{
    public enum PlayerLevel
    {
        Beginner,
        Easy,
        Normal,
        Hard,
        Expert,
        ExpertPlus,
        Lawless
    }

    public static int GetLevelIndex(string levelName)
    {
        if (Enum.TryParse(levelName, out PlayerLevel level))
        {
            return (int)level;
        }

        Plugin.Logger.Error($"Unknown player level: {levelName}");
        return (int)PlayerLevel.Normal;
    }

    public static PlayerLevel GetLevel(float pp)
    {
        return pp switch
        {
            < 2000 => PlayerLevel.Beginner,
            < 4000 => PlayerLevel.Easy,
            < 6000 => PlayerLevel.Normal,
            < 8000 => PlayerLevel.Hard,
            < 10000 => PlayerLevel.Expert,
            < 12000 => PlayerLevel.ExpertPlus,
            _ => PlayerLevel.Lawless
        };
    }
}