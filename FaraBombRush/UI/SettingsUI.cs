using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using FaraBombRush.Configs;
using FaraBombRush.Models;
using Zenject;

namespace FaraBombRush.UI;

internal class SettingsUI : IInitializable, IDisposable
{
    private readonly FaraBombRushFlowCoordinator _faraBombRushFlowCoordinator;
    private readonly PluginConfig _config;
    private MenuButton _menuButton;
#if GAME_1_40_8
    private readonly MenuButtons _menuButtons;
    private readonly MainFlowCoordinator _mainFlowCoordinator;

    private SettingsUI(FaraBombRushFlowCoordinator faraBombRushFlowCoordinator, PluginConfig config, MenuButtons menuButtons, MainFlowCoordinator mainFlowCoordinator)
    {
        _faraBombRushFlowCoordinator = faraBombRushFlowCoordinator;
        _config = config;
        _menuButtons = menuButtons;
        _mainFlowCoordinator = mainFlowCoordinator;
        _menuButton = CreateButton();
    }

    public void Dispose()
    {
        UiText.LanguageChanged -= ApplyText;
        _menuButtons.UnregisterButton(_menuButton);
    }

    public void Initialize()
    {
        UiText.LanguageChanged += ApplyText;
        _menuButtons.RegisterButton(_menuButton);
    }

    private void ShowFlow()
    {
        _mainFlowCoordinator.PresentFlowCoordinator(_faraBombRushFlowCoordinator);
    }

    private void ApplyText()
    {
        // MenuButton には説明文を後から変える手段がないため、作り直して差し替える
        _menuButtons.UnregisterButton(_menuButton);
        _menuButton = CreateButton();
        _menuButtons.RegisterButton(_menuButton);
    }
#else
    private SettingsUI(FaraBombRushFlowCoordinator faraBombRushFlowCoordinator, PluginConfig config)
    {
        _faraBombRushFlowCoordinator = faraBombRushFlowCoordinator;
        _config = config;
        _menuButton = CreateButton();
    }

    public void Dispose()
    {
        UiText.LanguageChanged -= ApplyText;
        if (MenuButtons.IsSingletonAvailable && BSMLParser.IsSingletonAvailable)
            MenuButtons.instance.UnregisterButton(_menuButton);
    }

    public void Initialize()
    {
        UiText.LanguageChanged += ApplyText;
        MenuButtons.instance.RegisterButton(_menuButton);
    }

    private void ShowFlow()
    {
        BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_faraBombRushFlowCoordinator);
    }

    private void ApplyText()
    {
        // メニューのシングルトンが無い（まだ生成されていない、または破棄後）なら何もしない
        if (!MenuButtons.IsSingletonAvailable) return;

        // MenuButton には説明文を後から変える手段がないため、作り直して差し替える
        MenuButtons.instance.UnregisterButton(_menuButton);
        _menuButton = CreateButton();
        MenuButtons.instance.RegisterButton(_menuButton);
    }
#endif

    private MenuButton CreateButton() =>
        new MenuButton("Fara Bomb Rush", UiText.Pick(_config.Language, "Fara Bomb Rush Settings", "Fara Bomb Rush の設定"), ShowFlow);
}
