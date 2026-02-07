using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using FaraBombRush.Exceptions;
using FaraBombRush.Models;
using UnityEngine;

namespace FaraBombRush.Controllers;

internal class FaraBombManagementPoolController
{
    private bool _isInitialized;
    private FaraBombManagementPoolModel _poolModel;

    internal void Initialize(GameObject rootObject, PluginConfig pluginConfig,
        AudioTimeSyncController audioTimeSyncController = null, float njs = 12f, float hitZOffset = 0.5f)
    {
        if (_isInitialized)
        {
            Plugin.Logger.Warn("Pool is already initialized");
            return;
        }

        _poolModel = new FaraBombManagementPoolModel(rootObject, pluginConfig, audioTimeSyncController, njs, hitZOffset);
        _isInitialized = true;
    }

    private Vector3 CalculateSpawnPosition(BombCommandModel commandModel)
    {
        var noteLineEnum = NotePositionEnumHelper.FromIndex(commandModel.PositionIndex);
        return noteLineEnum.GetNotePosition();
    }

    private void InitializeInstance(FaraBombComponentModel instance, BombCommandModel commandModel)
    {
        try
        {
            instance.InitializeWithCommand(commandModel);
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCodeEnum.InitializeInstanceError);
        }
    }

    internal void Spawn(BombCommandModel commandModel)
    {
        try
        {
            var spawnPosition = CalculateSpawnPosition(commandModel);
            var instance = _poolModel.Spawn(spawnPosition);
            if (instance == null)
            {
                Plugin.Logger.Warn("Pool exhausted, cannot spawn bomb");
                return;
            }
            InitializeInstance(instance, commandModel);
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCodeEnum.SpawnError);
        }
    }

    internal void Despawn(FaraBombComponentModel instance)
    {
        if (!_isInitialized)
        {
            Plugin.Logger.Warn("Attempting to despawn when pool is not initialized");
            return;
        }

        try
        {
            _poolModel.Despawn(instance);
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCodeEnum.DespawnError);
        }
    }

    internal List<FaraBombComponentModel> GetActiveItems()
    {
        if (!_isInitialized)
        {
            Plugin.Logger.Warn("Attempting to get active items when pool is not initialized");
            return [];
        }

        return _poolModel.GetActiveItems().ToList();
    }

    internal void Cleanup()
    {
        if (!_isInitialized) return;

        try
        {
            _poolModel.Cleanup();
            _isInitialized = false;
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCodeEnum.CleanupError);
        }
    }

    internal void SetPause(GamePauseStepEnum gamePauseStepEnum)
    {
        foreach (var item in GetActiveItems()) item.SetPause(gamePauseStepEnum);
    }
}
