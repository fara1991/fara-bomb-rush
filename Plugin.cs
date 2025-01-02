using System.Reflection;
using FaraBombRush.Configs;
using FaraBombRush.Dependencies;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace FaraBombRush;

[Plugin(RuntimeOptions.DynamicInit)]
public class Plugin
{
    private const string HarmonyId = "com.github.fara1991.FaraBombRush";
    private static readonly Harmony Harmony = new(HarmonyId);

    [Init]
    public Plugin(IPALogger logger, Config config, Zenjector zenjector)
    {
        var pluginConfig = config.Generated<PluginConfig>();
        Logger = logger;
        Logger?.Debug("Logger initialized.");

        // Zenjector
        zenjector.Install<GameCustomInstaller>(Location.App, pluginConfig);
        zenjector.Install<MenuCustomInstaller>(Location.Menu);
        zenjector.Install<SongPlayCustomInstaller>(Location.Player);
    }

    internal static IPALogger Logger { get; private set; }

    #region Disableable

    /// <summary>
    ///     Called when the plugin is enabled (including when the game starts if the plugin is enabled).
    /// </summary>
    [OnEnable]
    public void OnEnable()
    {
        ApplyHarmonyPatches();
    }

    /// <summary>
    ///     Called when the plugin is disabled and on Beat Saber quit. It is important to clean up any Harmony patches,
    ///     GameObjects, and Monobehaviours here.
    ///     The game should be left in a state as if the plugin was never started.
    ///     Methods marked [OnDisable] must return void or Task.
    /// </summary>
    [OnDisable]
    public void OnDisable()
    {
        RemoveHarmonyPatches();
    }

    #endregion

    // Uncomment the methods in this section if using Harmony

    #region Harmony

    /// <summary>
    ///     Attempts to apply all the Harmony patches in this assembly.
    /// </summary>
    private static void ApplyHarmonyPatches()
    {
        Logger?.Debug("Applying Harmony patches.");
        Harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    ///     Attempts to remove all the Harmony patches that used our HarmonyId.
    /// </summary>
    private static void RemoveHarmonyPatches()
    {
        Harmony.UnpatchSelf();
    }

    #endregion
}