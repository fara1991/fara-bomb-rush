using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Enums;

namespace FaraBombRush.Models;

public class FaraBombManagementPoolModel
{
    private readonly Queue<FaraBombComponents> _pool;
    private readonly HashSet<FaraBombComponents> _activeItems;
    private readonly GameObject _rootPrefab;
    private const int InitialSize = 50;

    public FaraBombManagementPoolModel(GameObject rootObject)
    {
        try
        {
            _pool = new Queue<FaraBombComponents>();
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

    private void PrewarmPool()
    {
        for (var i = 0; i < InitialSize; i++)
        {
            CreateNewInstance();
        }
    }

    private void CreateNewInstance()
    {
        var rootInstance = UnityEngine.Object.Instantiate(_rootPrefab);
        var components = rootInstance.GetComponent<FaraBombComponents>() ??
                         rootInstance.AddComponent<FaraBombComponents>();

        var bombInstance = rootInstance.transform.GetChild(0).gameObject;
        var effectInstance = rootInstance.transform.GetChild(1).gameObject;
        
        rootInstance.name = FaraBombNameEnum.Management.ToString();
        bombInstance.name = FaraBombNameEnum.BombObject.ToString();
        effectInstance.name = FaraBombNameEnum.ParticleEffect.ToString();
        
        // コンポーネントの初期化
        components.Initialize(rootInstance, bombInstance, effectInstance);
        rootInstance.SetActive(false);
        _pool.Enqueue(components);
    }
    
    public FaraBombComponents Spawn(Vector3 position)
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

    public void Despawn(FaraBombComponents instance)
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

    public IReadOnlyCollection<FaraBombComponents> GetActiveItems() => _activeItems;

    public void Cleanup()
    {
        try
        {
            foreach (var item in _activeItems)
            {
                item?.gameObject.SetActive(false);
            }

            _activeItems.Clear();

            foreach (var item in _pool.Where(item => item is not null))
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }

            _pool.Clear();

            Plugin.Logger.Debug("Pool cleanup completed");
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to cleanup pool");
            Plugin.Logger.Error(ex);
        }
    }

    public int ActiveCount => _activeItems.Count;
    public int PoolCount => _pool.Count;
}