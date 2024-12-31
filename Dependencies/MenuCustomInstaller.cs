using FaraBombRush.Views;
using Zenject;

namespace FaraBombRush.Dependencies;

public class MenuCustomInstaller : MonoInstaller {
	public override void InstallBindings() {
		Container.BindInterfacesAndSelfTo<SettingViewController>().FromNewComponentAsViewController().AsCached()
			.NonLazy();
	}
}