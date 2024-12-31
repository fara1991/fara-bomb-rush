using FaraBombRush.Controllers;
using Zenject;

namespace FaraBombRush.Dependencies;

public class GameCustomInstaller : Installer {
	public override void InstallBindings() {
		Container.BindInterfacesAndSelfTo<ChatCoreWrapperController>().AsCached().NonLazy();
	}
}