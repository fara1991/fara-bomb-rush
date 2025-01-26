using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using static FaraBombRush.Enums.NoteLineCustomEnum;
using UnityEngine;
using Zenject;

namespace FaraBombRush.Controllers.GameModes;

public class FaraBombGameModeBase : MonoBehaviour
{
    protected readonly List<NotePosition> NotePositionEnumList =
        Enum.GetValues(typeof(NotePosition)).Cast<NotePosition>().ToList();
    protected const float BombLineDiffBeat = 1.0f;
    protected int BombId;
    protected PluginConfig Config;

    [Inject]
    private void Construct(PluginConfig config)
    {
        Config = config;
    }

    protected virtual void BombPush()
    {

    }
}