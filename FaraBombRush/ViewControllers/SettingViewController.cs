using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using FaraBombRush.Configs;
using FaraBombRush.Controllers.Menus;
using FaraBombRush.Enums;
using Zenject;

namespace FaraBombRush.ViewControllers;

internal class SettingViewController : BSMLResourceViewController
{
    private PluginConfig _config;
    private readonly FaraBombLevelEnum _playerLevelEnum = FaraBombLevelEnumHelper.GetLevelEnum();
    private readonly float _playerPP = LoadSteamController.ScoreSaberPP;

    [Inject]
    private void Construct(PluginConfig config)
    {
        _config = config;
    }

    [UIValue("game-mode-options")]
    private List<object> GameModeOptions = Enum.GetNames(typeof(GameModeEnum)).ToList<object>();

    [UIValue("player-bomb-level-options")]
    private List<object> PlayerBombLevelOptions = Enum.GetNames(typeof(FaraBombLevelEnum)).ToList<object>();

    public override string ResourceName => "FaraBombRush.Views.SettingView.bsml";

    [UIValue("selected-game-mode")]
    private string GameMode
    {
        get => _config.GameMode;
        set => _config.GameMode = value;
    }

    [UIValue("selected-player-bomb-level")]
    private string PlayerBombLevel
    {
        get => _config.PlayerBombLevel;
        set => _config.PlayerBombLevel = value;
    }

    [UIValue("player-recommend-level")]
    private string RecommendLevel =>
        $"Calculate the recommended level using ScoreSaber's PP. Now ScoreSaber PP is {_playerPP}.\nYour recommended level is [{_playerLevelEnum.ToString()}].";

    [UIValue("selected-is-bomb-cut-enable")]
    private bool IsBombCutEnable
    {
        get => _config.IsBombCutEnable;
        set => _config.IsBombCutEnable = value;
    }

    [UIValue("selected-bomb-line-count")]
    private int BombLineCount
    {
        get => _config.BombLineCount;
        set => _config.BombLineCount = value;
    }
}
