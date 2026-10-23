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
        // None と未実装の Battle は何も起動しない（システム管理もチャット読み取りも作らない）
        if (!_config.IsEnabled || !GameModeEnumHelper.RunsMod(_config.GameMode)) return;

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
        if (GameModeEnumHelper.Normalize(_config.GameMode) == nameof(GameModeEnum.Interactive))
        {
            Container.BindInterfacesAndSelfTo<FaraBombInteractiveModeController>().FromNewComponentOnNewGameObject()
                .AsCached().NonLazy();
        }
        else
        {
            Container.BindInterfacesAndSelfTo<FaraBombAutoModeController>().FromNewComponentOnNewGameObject().AsCached()
                .NonLazy();
        }
    }
}
