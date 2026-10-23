using FaraBombRush.Enums;

namespace FaraBombRush.Models;

/// <summary>ScoreSaber の PP から推奨ボムレベルを決める純粋関数。</summary>
public static class BombLevelCalculator
{
    public static FaraBombLevelEnum FromPp(float pp)
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
