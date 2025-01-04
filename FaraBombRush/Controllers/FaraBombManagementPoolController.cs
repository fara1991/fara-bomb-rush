using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using FaraBombRush.Models;
using UnityEngine;

namespace FaraBombRush.Controllers;

public class FaraBombManagementPoolController
{
    private bool _isInitialized;
    private FaraBombManagementPoolModel _poolModel;

    public void Initialize(GameObject rootObject, PluginConfig pluginConfig)
    {
        if (rootObject is null) throw new ArgumentNullException(nameof(rootObject), "Root object cannot be null");

        if (_isInitialized)
        {
            Plugin.Logger.Warn("Pool is already initialized");
            return;
        }

        try
        {
            _poolModel = new FaraBombManagementPoolModel(rootObject, pluginConfig);
            _isInitialized = true;
            Plugin.Logger.Debug("Pool initialized successfully");
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to initialize pool");
            Plugin.Logger.Error(ex);
            throw;
        }
    }

    private void ValidateInitializationAndInput(BombCommandModel commandModel)
    {
        if (!_isInitialized) throw new InvalidOperationException("Pool is not initialized");

        if (commandModel == null) throw new ArgumentNullException(nameof(commandModel));
    }

    private Vector3 CalculateSpawnPosition(BombCommandModel commandModel)
    {
        try
        {
            var noteLineEnum = (NoteLineCustomEnum)commandModel.PositionIndex;
            return noteLineEnum.GetNotePosition(commandModel.SpawnDelayTime);
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error($"Failed to calculate spawn position for index: {commandModel.PositionIndex}");
            Plugin.Logger.Error(ex);
            throw;
        }
    }

    private void InitializeInstance(FaraBombComponentModel instance, BombCommandModel commandModel)
    {
        try
        {
            instance.InitializeWithCommand(commandModel);
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to initialize instance components");
            Plugin.Logger.Error(ex);
            throw;
        }
    }

    public void Spawn(BombCommandModel commandModel)
    {
        ValidateInitializationAndInput(commandModel);

        try
        {
            var spawnPosition = CalculateSpawnPosition(commandModel);
            var instance = _poolModel.Spawn(spawnPosition);

            if (instance is null) throw new InvalidOperationException("Failed to spawn instance from pool");

            InitializeInstance(instance, commandModel);
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error($"Failed to spawn bomb with ID: {commandModel.BombId}");
            Plugin.Logger.Error(ex);
            throw;
        }
    }

    public void Despawn(FaraBombComponentModel instance)
    {
        if (!_isInitialized)
        {
            Plugin.Logger.Warn("Attempting to despawn when pool is not initialized");
            return;
        }

        if (instance is null)
        {
            Plugin.Logger.Warn("Attempting to despawn null instance");
            return;
        }

        try
        {
            _poolModel.Despawn(instance);
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to despawn instance");
            Plugin.Logger.Error(ex);
            throw;
        }
    }

    public List<FaraBombComponentModel> GetActiveItems()
    {
        if (!_isInitialized)
        {
            Plugin.Logger.Warn("Attempting to get active items when pool is not initialized");
            return [];
        }

        try
        {
            return _poolModel.GetActiveItems().ToList();
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to get active items");
            Plugin.Logger.Error(ex);
            return [];
        }
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
            Plugin.Logger.Error("Error during cleanup");
            Plugin.Logger.Error(ex);
        }
    }
}