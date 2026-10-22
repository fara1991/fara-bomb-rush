using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using Zenject;

namespace FaraBombRush.UI;

internal class SettingsUI : IInitializable, IDisposable
{
    private readonly FaraBombRushFlowCoordinator _faraBombRushFlowCoordinator;
    private readonly MenuButton _menuButton;
#if GAME_1_40_8
    private readonly MenuButtons _menuButtons;
    private readonly MainFlowCoordinator _mainFlowCoordinator;

    private SettingsUI(FaraBombRushFlowCoordinator faraBombRushFlowCoordinator, MenuButtons menuButtons, MainFlowCoordinator mainFlowCoordinator)
    {
        _faraBombRushFlowCoordinator = faraBombRushFlowCoordinator;
        _menuButtons = menuButtons;
        _mainFlowCoordinator = mainFlowCoordinator;
        _menuButton = new MenuButton("Fara Bomb Rush", "Fara Bomb Rush Settings", ShowFlow);
    }

    public void Dispose()
    {
        _menuButtons.UnregisterButton(_menuButton);
    }

    public void Initialize()
    {
        _menuButtons.RegisterButton(_menuButton);
    }

    private void ShowFlow()
    {
        _mainFlowCoordinator.PresentFlowCoordinator(_faraBombRushFlowCoordinator);
    }
#else
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
#endif
}
