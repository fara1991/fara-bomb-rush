using System;
using FaraBombRush.Controllers.Menus;
using FaraBombRush.Models;

namespace FaraBombRush.Enums;

public static class FaraBombLevelEnumHelper
{
    public static int GetLevelIndex(string levelName)
    {
        if (Enum.TryParse(levelName, out FaraBombLevelEnum levelEnum)) return (int)levelEnum;

        Plugin.Logger.Error($"Unknown player level: {levelName}");
        return (int)FaraBombLevelEnum.Normal;
    }

    public static FaraBombLevelEnum GetLevelEnum()
    {
        return BombLevelCalculator.FromPp(LoadSteamController.ScoreSaberPP);
    }
}
