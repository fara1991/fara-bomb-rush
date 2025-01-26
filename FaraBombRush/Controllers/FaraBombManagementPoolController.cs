using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using static FaraBombRush.Enums.ErrorCodeEnum;
using static FaraBombRush.Enums.GamePauseStepEnum;
using FaraBombRush.Exceptions;
using FaraBombRush.Models;
using UnityEngine;

namespace FaraBombRush.Controllers;

public class FaraBombManagementPoolController
{
    private bool _isInitialized;
    private FaraBombManagementPoolModel _poolModel;

    public void Initialize(GameObject rootObject, PluginConfig pluginConfig)
    {
        if (_isInitialized)
        {
            Plugin.Logger.Warn("Pool is already initialized");
            return;
        }

        _poolModel = new FaraBombManagementPoolModel(rootObject, pluginConfig);
        _isInitialized = true;
        Plugin.Logger.Debug("Pool initialized successfully");
    }

    private Vector3 CalculateSpawnPosition(BombCommandModel commandModel)
    {
        var noteLineEnum = (NoteLineCustomEnum)commandModel.PositionIndex;
        return noteLineEnum.GetNotePosition(commandModel.SpawnDelayTime);
    }

    private void InitializeInstance(FaraBombComponentModel instance, BombCommandModel commandModel)
    {
        try
        {
            instance.InitializeWithCommand(commandModel);
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCode.InitializeInstanceError);
        }
    }

    public void Spawn(BombCommandModel commandModel)
    {
        try
        {
            var spawnPosition = CalculateSpawnPosition(commandModel);
            var instance = _poolModel.Spawn(spawnPosition);
            InitializeInstance(instance, commandModel);
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCode.SpawnError);
        }
    }

    public void Despawn(FaraBombComponentModel instance)
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
            throw new FaraBombException(ex.Message, ErrorCode.DespawnError);
        }
    }

    public List<FaraBombComponentModel> GetActiveItems()
    {
        if (!_isInitialized)
        {
            Plugin.Logger.Warn("Attempting to get active items when pool is not initialized");
            return [];
        }

        return _poolModel.GetActiveItems().ToList();
    }

    public void Cleanup()
    {
        if (!_isInitialized) return;

        try
        {
            _poolModel.Cleanup();
            _isInitialized = false;
            Plugin.Logger.Debug("Pool cleanup completed");
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCode.CleanupError);
        }
    }

    public void SetPause(GamePauseStep pauseStep)
    {
        foreach (var item in GetActiveItems())
        {
            item.SetPause(pauseStep);
        }
    }
}