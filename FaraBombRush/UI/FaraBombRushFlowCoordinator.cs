using BeatSaberMarkupLanguage;
using FaraBombRush.Configs;
using FaraBombRush.Services;
using FaraBombRush.ViewControllers;
using HMUI;
using Zenject;

namespace FaraBombRush.UI;

internal class FaraBombRushFlowCoordinator : FlowCoordinator
{
    private MainFlowCoordinator _mainFlowCoordinator;
    private SettingViewController _settingViewController;
    private MenuBombPreviewService _previewService;
    private PluginConfig _config;

    [Inject]
    private void Construct(
        MainFlowCoordinator mainFlowCoordinator,
        SettingViewController settingViewController,
        MenuBombPreviewService previewService,
        PluginConfig config)
    {
        _mainFlowCoordinator = mainFlowCoordinator;
        _settingViewController = settingViewController;
        _previewService = previewService;
        _config = config;
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (firstActivation)
        {
            showBackButton = true;
            SetTitle("FaraBombRush Settings");
            ProvideInitialViewControllers(_settingViewController);
        }

        _previewService.UpdateEnabled(_config.IsEnabled);
    }

    protected override void BackButtonWasPressed(ViewController viewController)
    {
        _previewService.Hide();
        base.BackButtonWasPressed(viewController);
        _mainFlowCoordinator.DismissFlowCoordinator(this);
    }
}
