using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FaraBombRush.Models;

public class FaraBombManagementPoolModel
{
    private const int InitialSize = 50;
    private readonly HashSet<FaraBombComponentModel> _activeItems;
    private readonly Queue<FaraBombComponentModel> _pool;
    private readonly GameObject _rootPrefab;
    private PluginConfig _pluginConfig;

    public FaraBombManagementPoolModel(GameObject rootObject, PluginConfig pluginConfig)
    {
        _pluginConfig = pluginConfig;
        try
        {
            _pool = new Queue<FaraBombComponentModel>();
            _activeItems = [];
            _rootPrefab = rootObject;

            PrewarmPool();
            Plugin.Logger.Debug($"Pool initialized with {InitialSize} instances");
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to initialize FaraBombManagementPoolModel");
            Plugin.Logger.Error(ex);
            throw;
        }
    }

    public int ActiveCount => _activeItems.Count;
    public int PoolCount => _pool.Count;

    private void PrewarmPool()
    {
        for (var i = 0; i < InitialSize; i++) CreateNewInstance();
    }

    private void CreateNewInstance()
    {
        var rootInstance = Object.Instantiate(_rootPrefab);
        var components = rootInstance.GetComponent<FaraBombComponentModel>() ??
                         rootInstance.AddComponent<FaraBombComponentModel>();

        var bombInstance = rootInstance.transform.GetChild(0).gameObject;
        var effectInstance = rootInstance.transform.GetChild(1).gameObject;

        rootInstance.name = FaraBombNameEnum.Management.ToString();
        bombInstance.name = FaraBombNameEnum.BombObject.ToString();
        effectInstance.name = FaraBombNameEnum.ParticleEffect.ToString();

        // コンポーネントの初期化
        components.Initialize(rootInstance, bombInstance, effectInstance, _pluginConfig);
        rootInstance.SetActive(false);
        _pool.Enqueue(components);
    }

    public FaraBombComponentModel Spawn(Vector3 position)
    {
        try
        {
            if (_pool.Count == 0)
            {
                Plugin.Logger.Warn("Pool is empty, cannot spawn new instance");
                return null;
            }

            var instance = _pool.Dequeue();
            instance.transform.position = position;
            instance.OnSpawned();

            _activeItems.Add(instance);
            return instance;
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error($"Failed to spawn at position {position}");
            Plugin.Logger.Error(ex);
            return null;
        }
    }

    public void Despawn(FaraBombComponentModel instance)
    {
        if (instance is null)
        {
            Plugin.Logger.Warn("Attempting to despawn null instance");
            return;
        }

        try
        {
            if (!_activeItems.Remove(instance))
            {
                Plugin.Logger.Warn($"Attempted to despawn inactive instance: {instance.gameObject.name}");
                return;
            }

            _pool.Enqueue(instance);
            instance.OnDespawned();
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to despawn instance");
            Plugin.Logger.Error(ex);
        }
    }

    public IReadOnlyCollection<FaraBombComponentModel> GetActiveItems()
    {
        return _activeItems;
    }

    public void Cleanup()
    {
        try
        {
            foreach (var item in _activeItems) item?.gameObject.SetActive(false);

            _activeItems.Clear();

            foreach (var item in _pool.Where(item => item is not null)) Object.Destroy(item.gameObject);

            _pool.Clear();

            Plugin.Logger.Debug("Pool cleanup completed");
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to cleanup pool");
            Plugin.Logger.Error(ex);
        }
    }
}