using BeatSaberMarkupLanguage;
using FaraBombRush.ViewControllers;
using HMUI;
using Zenject;

namespace FaraBombRush.UI;

internal class FaraBombRushFlowCoordinator : FlowCoordinator
{
    private MainFlowCoordinator _mainFlowCoordinator;
    private SettingViewController _settingViewController;

    [Inject]
    public void Construct(
        MainFlowCoordinator mainFlowCoordinator,
        SettingViewController settingViewController)
    {
        _mainFlowCoordinator = mainFlowCoordinator;
        _settingViewController = settingViewController;
    }

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        if (!firstActivation) return;

        showBackButton = true;
        SetTitle("Fara Bomb Rush");
        ProvideInitialViewControllers(_settingViewController);
    }

    protected override void BackButtonWasPressed(ViewController viewController)
    {
        base.BackButtonWasPressed(viewController);
        _mainFlowCoordinator.DismissFlowCoordinator(this);
    }
}