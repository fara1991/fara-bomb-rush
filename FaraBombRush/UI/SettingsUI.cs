using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using Zenject;

namespace FaraBombRush.UI;

internal class SettingsUI : IInitializable, IDisposable
{
    private readonly FaraBombRushFlowCoordinator _faraBombRushFlowCoordinator;
    private readonly MenuButton _menuButton;

    private SettingsUI(FaraBombRushFlowCoordinator faraBombRushFlowCoordinator)
    {
        _faraBombRushFlowCoordinator = faraBombRushFlowCoordinator;
        _menuButton = new MenuButton("Fara Bomb Rush", "Fara Bomb Rush Settings", ShowFlow);
    }

    public void Dispose()
    {
        if (MenuButtons.IsSingletonAvailable && BSMLParser.IsSingletonAvailable)
            MenuButtons.instance.UnregisterButton(_menuButton);
    }

    public void Initialize()
    {
        MenuButtons.instance.RegisterButton(_menuButton);
    }

    private void ShowFlow()
    {
        BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_faraBombRushFlowCoordinator);
    }
}
