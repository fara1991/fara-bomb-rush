using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using Zenject;

namespace FaraBombRush.ViewControllers;

public class SettingViewController : BSMLResourceViewController
{
    private PluginConfig _config;

    [Inject]
    private void Construct(PluginConfig config)
    {
        _config = config;
    }
    
    [UIValue("game-mode-options")]
    private List<object> GameModeOptions = Enum.GetNames(typeof(GameModeEnum)).ToList<object>();

    public override string ResourceName => "FaraBombRush.Views.SettingView.bsml";

    [UIValue("selected-game-mode")]
    public string GameMode
    {
        get => _config.GameMode;
        set => _config.GameMode = value;
    }

    [UIValue("selected-is-bomb-command-enable")]
    public bool IsBombCommandEnable
    {
        get => _config.IsBombCommandEnable;
        set => _config.IsBombCommandEnable = value;
    }

    [UIValue("selected-is-bomb-cut-enable")]
    public bool IsBombCutEnable
    {
        get => _config.IsBombCutEnable;
        set => _config.IsBombCutEnable = value;
    }

    [UIValue("selected-bomb-line-count")]
    public int BombLineCount
    {
        get => _config.BombLineCount;
        set => _config.BombLineCount = value;
    }

    [UIValue("selected-bomb-spawn-distance")]
    public float BombSpawnDistance
    {
        get => _config.BombSpawnDistance;
        set => _config.BombSpawnDistance = value;
    }
}