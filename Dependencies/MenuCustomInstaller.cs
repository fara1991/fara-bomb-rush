using FaraBombRush.UI;
using FaraBombRush.ViewControllers;
using Zenject;

namespace FaraBombRush.Dependencies;

public class MenuCustomInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.Bind<SettingViewController>().FromNewComponentAsViewController().AsSingle();
        Container.BindInterfacesAndSelfTo<SettingsUI>().AsSingle();
        Container.BindInterfacesAndSelfTo<FaraBombRushFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
    }
}