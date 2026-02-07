using FaraBombRush.Configs;
using FaraBombRush.Controllers;
using FaraBombRush.Controllers.Components;
using FaraBombRush.Controllers.GameModes;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using Zenject;

namespace FaraBombRush.Dependencies;

public class SongPlayCustomInstaller : Installer
{
    private readonly PluginConfig _config;

    private SongPlayCustomInstaller(PluginConfig config)
    {
        _config = config;
    }

    public override void InstallBindings()
    {
        if (_config.GameMode == GameModeEnum.None.ToString()) return;

        // Beat Saber's audio and spawn system
        Container.Bind<AudioTimeSyncController>().FromResolve();
        Container.Bind<BeatmapObjectSpawnController.InitData>().FromResolve().AsCached();
        Container.Bind<IDifficultyBeatmap>().FromResolve().AsCached();

        Container.BindInterfacesAndSelfTo<FaraBombSystemManager>()
            .FromNewComponentOnNewGameObject()
            .AsCached()
            .NonLazy();

        // Pool操作
        Container.Bind<FaraBombManagementPoolController>().AsCached();
        Container.Bind<FaraBombManagementPoolModel>().AsCached();

        // FaraBomb Model, Component
        Container.Bind<FaraBombComponentModel>().FromNewComponentOnNewGameObject().AsTransient();
        Container.Bind<FaraBombCoordinatorController>().FromNewComponentOnNewGameObject().AsTransient();
        Container.Bind<FaraBombColliderController>().FromNewComponentOnNewGameObject().AsTransient();
        Container.Bind<FaraBombMoveController>().FromNewComponentOnNewGameObject().AsTransient();
        Container.Bind<FaraBombEffectController>().FromNewComponentOnNewGameObject().AsTransient();
        Container.Bind<FaraBombScoreController>().FromNewComponentOnNewGameObject().AsTransient();

        // Play Mode
        Container.BindInterfacesAndSelfTo<FaraBombGameModeBaseController>().FromNewComponentOnNewGameObject().AsCached()
            .NonLazy();
        if (_config.GameMode == GameModeEnum.Interactive.ToString())
        {
            Container.BindInterfacesAndSelfTo<FaraBombInteractiveModeController>().FromNewComponentOnNewGameObject()
                .AsCached().NonLazy();
        }
        else if (_config.GameMode == GameModeEnum.Auto.ToString())
        {
            Container.BindInterfacesAndSelfTo<FaraBombAutoModeController>().FromNewComponentOnNewGameObject().AsCached()
                .NonLazy();
        }
        else if (_config.GameMode == GameModeEnum.Battle.ToString())
        {
            // 後で実装
        }
    }
}
