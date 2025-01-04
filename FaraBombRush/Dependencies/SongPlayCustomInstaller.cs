using FaraBombRush.Configs;
using FaraBombRush.Controllers;
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
        // Pool Manager
        Container.BindInterfacesAndSelfTo<FaraBombPoolManager>()
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

        // Play Mode
        if (_config.GameMode == GameModeEnum.Interactive.ToString())
        {
            Container.BindInterfacesAndSelfTo<ChatCoreWrapperController>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
            Container.BindInterfacesAndSelfTo<FaraBombCommandController>().AsCached().NonLazy();
        }
        else if (_config.GameMode == GameModeEnum.Auto.ToString())
        {
            Container.BindInterfacesAndSelfTo<FaraBombAutoModeController>().AsCached().NonLazy();
        }
        else if (_config.GameMode == GameModeEnum.Battle.ToString())
        {
            // 後で実装
        }
    }
}