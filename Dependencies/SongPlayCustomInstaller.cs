using FaraBombRush.Controllers;
using FaraBombRush.Enums;
using FaraBombRush.Managers;
using FaraBombRush.Models;
using UnityEngine;
using Zenject;

namespace FaraBombRush.Dependencies;

public class SongPlayCustomInstaller : Installer
{
    public override void InstallBindings()
    {
        // Pool Manager
        Container.BindInterfacesAndSelfTo<FaraBombPoolManager>()
            .FromNewComponentOnNewGameObject()
            .AsCached()
            .NonLazy();

        // Pool操作
        Container.BindInterfacesAndSelfTo<FaraBombManagementPoolController>().AsCached();
        Container.Bind<FaraBombManagementPoolModel>().AsCached();

        // FaraBomb Controller
        Container.Bind<FaraBombCoordinatorController>().FromNewComponentOnNewGameObject().AsTransient();
        Container.Bind<FaraBombColliderController>().FromNewComponentOnNewGameObject().AsTransient();
        Container.Bind<FaraBombMoveController>().FromNewComponentOnNewGameObject().AsTransient();
        Container.Bind<FaraBombEffectController>().FromNewComponentOnNewGameObject().AsTransient();
    }
}