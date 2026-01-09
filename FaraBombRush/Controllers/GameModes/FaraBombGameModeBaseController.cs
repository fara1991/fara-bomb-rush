using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using UnityEngine;
using Zenject;

namespace FaraBombRush.Controllers.GameModes;

internal class FaraBombGameModeBaseController : MonoBehaviour
{
    protected const float BombLineDiffBeat = 1.0f;

    protected readonly List<NotePositionEnum> NotePositionEnumList =
        Enum.GetValues(typeof(NotePositionEnum)).Cast<NotePositionEnum>().ToList();

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