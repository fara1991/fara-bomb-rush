using BeatSaberMarkupLanguage;
using FaraBombRush.Configs;
using FaraBombRush.Models;
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
            ProvideInitialViewControllers(_settingViewController);
        }

        // 言語を変えたあとに開き直しても、タイトルが新しい言語になるように毎回設定する
        SetTitle(UiText.Pick(_config.Language, "FaraBombRush Settings", "FaraBombRush の設定"));

        _previewService.UpdateEnabled(_config.IsEnabled);
    }

    protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
    {
        // 戻るボタン以外（別経路で閉じる場合など）でも、プレビューの爆弾を片付ける
        _previewService.Hide();
    }

    protected override void BackButtonWasPressed(ViewController viewController)
    {
        base.BackButtonWasPressed(viewController);
        _mainFlowCoordinator.DismissFlowCoordinator(this);
    }
}
