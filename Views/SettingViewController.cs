using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;


namespace FaraBombRush.Views;

[HotReload(RelativePathToLayout = @"SettingViewController.bsml")]
[ViewDefinition("FaraBombRush.Views.SettingViewController.bsml")]
internal class SettingViewController : BSMLAutomaticViewController {
	private string yourTextField = "Hello World";
	public string YourTextProperty {
		get { return yourTextField; }
		set {
			if (yourTextField == value) return;
			yourTextField = value;
			NotifyPropertyChanged();
		}
	}

	[UIAction("#post-parse")]
	internal void PostParse() {
		// Code to run after BSML finishes
	}

	public void Initialize() {
		BSMLSettings.instance.AddSettingsMenu("<size=80%>FaraBombRush</size>", string.Join(".", GetType().Namespace, GetType().Name), this);
	}
}