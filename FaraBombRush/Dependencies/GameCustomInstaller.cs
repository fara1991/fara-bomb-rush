using FaraBombRush.Configs;
using FaraBombRush.Controllers.Menus;
using Zenject;

namespace FaraBombRush.Dependencies;

public class GameCustomInstaller : Installer
{
    private readonly PluginConfig _config;

    private GameCustomInstaller(PluginConfig config)
    {
        _config = config;
    }

    public override void InstallBindings()
    {
        Container.BindInstance(_config).AsSingle();
        Container.BindInterfacesAndSelfTo<LoadSteamController>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
    }
}
