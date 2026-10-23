using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using FaraBombRush.Configs;
using FaraBombRush.Controllers.Menus;
using FaraBombRush.Enums;
using FaraBombRush.Models;
using FaraBombRush.Services;
using Zenject;

namespace FaraBombRush.ViewControllers;

// 文言は BSML の ~key と UIValue で結ぶ。言語を変えたら NotifyPropertyChanged で全部描き直す（FaraAccField と同じ方式）
[ViewDefinition("FaraBombRush.Views.SettingView.bsml")]
internal class SettingViewController : BSMLAutomaticViewController
{
    private PluginConfig _config;
    private MenuBombPreviewService _previewService;

    [Inject]
    private void Construct(PluginConfig config, MenuBombPreviewService previewService)
    {
        _config = config;
        _previewService = previewService;

        // PP は設定画面を作った後に取得が終わることがあるので、取得が終わったら表示を描き直す
        LoadSteamController.Changed -= OnScoreSaberChanged;
        LoadSteamController.Changed += OnScoreSaberChanged;
    }

    protected override void OnDestroy()
    {
        LoadSteamController.Changed -= OnScoreSaberChanged;
        base.OnDestroy();
    }

    private void OnScoreSaberChanged()
    {
        NotifyPropertyChanged(nameof(RecommendLevel));
    }

    private string L(string en, string ja) => UiText.Pick(_config?.Language, en, ja);

    #region Localized text

    [UIValue("header-common")]
    public string HeaderCommon => L("Common", "共通");

    [UIValue("header-bomb")]
    public string HeaderBomb => L("Bomb Settings", "ボムの設定");

    [UIValue("language-text")]
    public string LanguageText => L("Language", "言語");

    [UIValue("language-hint")]
    public string LanguageHint => L("Choose the display language of the settings and menu", "設定とメニューの表示言語を選びます");

    [UIValue("enabled-text")]
    public string EnabledText => L("Enabled", "有効");

    [UIValue("enabled-hint")]
    public string EnabledHint => L("Enable or disable Fara Bomb Rush", "Fara Bomb Rush を有効または無効にします");

    [UIValue("game-mode-text")]
    public string GameModeText => L("Play Mode", "プレイモード");

    [UIValue("game-mode-hint")]
    public string GameModeHint => L("Choose a game mode to play", "遊ぶモードを選びます");

    [UIValue("player-level-text")]
    public string PlayerLevelText => L("Player Bomb Level", "プレイヤーのボムレベル");

    [UIValue("bomb-cut-text")]
    public string BombCutText => L("Valid Bomb Collision", "ボムの当たり判定");

    [UIValue("bomb-cut-hint")]
    public string BombCutHint => L("Choose whether bombs can be hit", "ボムに当たり判定を付けるかを選びます");

    [UIValue("player-recommend-level")]
    public string RecommendLevel
    {
        get
        {
            var pp = LoadSteamController.ScoreSaberPP;
            var level = FaraBombLevelEnumHelper.GetLevelEnum();
            switch (LoadSteamController.State)
            {
                case ScoreSaberLoadState.Loaded:
                    return L(
                        $"Calculate the recommended level using ScoreSaber's PP. Now ScoreSaber PP is {pp:0.##}.\nYour recommended level is [{level}].",
                        $"ScoreSaber の PP から推奨レベルを計算します。現在の PP は {pp:0.##} です。\n推奨レベルは [{level}] です。");
                case ScoreSaberLoadState.NotRegistered:
                    return L(
                        "Could not get the PP: no ScoreSaber account was found for this Steam account.\nThe recommended level is unknown.",
                        "PP を取得できませんでした（この Steam アカウントの ScoreSaber 登録が見つかりません）。\n推奨レベルは不明です。");
                case ScoreSaberLoadState.Failed:
                    return L(
                        "Could not get the PP from ScoreSaber (see the log for details).\nThe recommended level is unknown.",
                        "ScoreSaber から PP を取得できませんでした（詳細はログを参照）。\n推奨レベルは不明です。");
                default:
                    return L(
                        "Loading the PP from ScoreSaber...\nThe recommended level is unknown yet.",
                        "ScoreSaber から PP を取得しています…\n推奨レベルはまだ不明です。");
            }
        }
    }

    private void NotifyAllTextChanged()
    {
        NotifyPropertyChanged(nameof(HeaderCommon));
        NotifyPropertyChanged(nameof(HeaderBomb));
        NotifyPropertyChanged(nameof(LanguageText));
        NotifyPropertyChanged(nameof(LanguageHint));
        NotifyPropertyChanged(nameof(EnabledText));
        NotifyPropertyChanged(nameof(EnabledHint));
        NotifyPropertyChanged(nameof(GameModeText));
        NotifyPropertyChanged(nameof(GameModeHint));
        NotifyPropertyChanged(nameof(PlayerLevelText));
        NotifyPropertyChanged(nameof(BombCutText));
        NotifyPropertyChanged(nameof(BombCutHint));
        NotifyPropertyChanged(nameof(RecommendLevel));
    }

    #endregion

    #region Settings values

    [UIValue("language-options")]
    public List<object> LanguageOptions => UiText.LanguageOptions.Cast<object>().ToList();

    [UIValue("selected-language")]
    public string Language
    {
        get => UiText.Normalize(_config.Language);
        set
        {
            _config.Language = UiText.Normalize(value);
            _config.Changed();
            NotifyAllTextChanged();
            UiText.RaiseLanguageChanged();
        }
    }

    [UIValue("selected-is-enabled")]
    public bool IsEnabled
    {
        get => _config.IsEnabled;
        set
        {
            _config.IsEnabled = value;
            _config.Changed();
            _previewService.UpdateEnabled(value);
        }
    }

    [UIValue("game-mode-options")]
    public List<object> GameModeOptions => Enum.GetNames(typeof(GameModeEnum)).ToList<object>();

    [UIValue("selected-game-mode")]
    public string GameMode
    {
        get => GameModeEnumHelper.Normalize(_config.GameMode);
        set
        {
            _config.GameMode = GameModeEnumHelper.Normalize(value);
            _config.Changed();
        }
    }

    [UIValue("player-bomb-level-options")]
    public List<object> PlayerBombLevelOptions => Enum.GetNames(typeof(FaraBombLevelEnum)).ToList<object>();

    [UIValue("selected-player-bomb-level")]
    public string PlayerBombLevel
    {
        get => _config.PlayerBombLevel;
        set
        {
            _config.PlayerBombLevel = value;
            _config.Changed();
        }
    }

    [UIValue("selected-is-bomb-cut-enable")]
    public bool IsBombCutEnable
    {
        get => _config.IsBombCutEnable;
        set
        {
            _config.IsBombCutEnable = value;
            _config.Changed();
        }
    }

    #endregion
}
