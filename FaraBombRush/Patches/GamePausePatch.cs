using HarmonyLib;
using static FaraBombRush.Enums.GamePauseStepEnum;

namespace FaraBombRush.Patches;

[HarmonyPatch(typeof(GamePause))]
internal class GamePausePatch
{
    internal static GamePauseStep GamePauseStep = GamePauseStep.Resume;

    [HarmonyPatch(nameof(GamePause.Pause))]
    [HarmonyPostfix]
    public static void AfterPause()
    {
        GamePauseStep = GamePauseStep.Pause;
        Plugin.Logger.Debug("GamePause.Pause called");
    }

    [HarmonyPatch(nameof(GamePause.WillResume))]
    [HarmonyPrefix]
    public static void WillResume()
    {
        GamePauseStep = GamePauseStep.WillResume;
        Plugin.Logger.Debug("GamePause.WillResume called");
    }

    [HarmonyPatch(nameof(GamePause.Resume))]
    [HarmonyPostfix]
    public static void AfterResume()
    {
        GamePauseStep = GamePauseStep.Resume;
        Plugin.Logger.Debug("GamePause.Resume called");
    }
}