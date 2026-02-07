using FaraBombRush.Enums;
using HarmonyLib;

namespace FaraBombRush.Patches;

[HarmonyPatch(typeof(GamePause))]
internal class GamePausePatch
{
    internal static GamePauseStepEnum GamePauseStepEnum = GamePauseStepEnum.Resume;

    [HarmonyPatch(nameof(GamePause.Pause))]
    [HarmonyPostfix]
    public static void AfterPause()
    {
        GamePauseStepEnum = GamePauseStepEnum.Pause;
        Plugin.Logger.Info("GamePause.Pause called");
    }

    [HarmonyPatch(nameof(GamePause.WillResume))]
    [HarmonyPrefix]
    public static void WillResume()
    {
        GamePauseStepEnum = GamePauseStepEnum.WillResume;
        Plugin.Logger.Info("GamePause.WillResume called");
    }

    [HarmonyPatch(nameof(GamePause.Resume))]
    [HarmonyPostfix]
    public static void AfterResume()
    {
        GamePauseStepEnum = GamePauseStepEnum.Resume;
        Plugin.Logger.Info("GamePause.Resume called");
    }
}
