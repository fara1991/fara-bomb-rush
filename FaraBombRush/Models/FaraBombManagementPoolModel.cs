using System.Collections.Generic;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FaraBombRush.Models;

internal class FaraBombManagementPoolModel
{
    private const int InitialSize = 100;
    private readonly HashSet<FaraBombComponentModel> _activeItems;
    private readonly PluginConfig _pluginConfig;
    private readonly Queue<FaraBombComponentModel> _pool;
    private readonly GameObject _rootPrefab;
    private readonly AudioTimeSyncController _audioTimeSyncController;
    private readonly float _njs;
    private readonly float _hitZOffset;

    internal FaraBombManagementPoolModel(GameObject rootObject, PluginConfig pluginConfig,
        AudioTimeSyncController audioTimeSyncController = null, float njs = 12f, float hitZOffset = 0.5f)
    {
        _pluginConfig = pluginConfig;
        _audioTimeSyncController = audioTimeSyncController;
        _njs = njs;
        _hitZOffset = hitZOffset;
        _pool = new Queue<FaraBombComponentModel>();
        _activeItems = [];
        _rootPrefab = rootObject;

        PrewarmPool();
        Plugin.Logger.Info($"Pool initialized with {InitialSize} instances");
    }

    internal int ActiveCount => _activeItems.Count;
    internal int PoolCount => _pool.Count;

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

        components.Initialize(rootInstance, bombInstance, effectInstance, _pluginConfig, _audioTimeSyncController, _njs, _hitZOffset);
        rootInstance.SetActive(false);
        _pool.Enqueue(components);
    }

    internal FaraBombComponentModel Spawn(Vector3 position)
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

    internal void Despawn(FaraBombComponentModel instance)
    {
        if (!_activeItems.Remove(instance))
        {
            Plugin.Logger.Warn($"Attempted to despawn inactive instance: {instance.gameObject.name}");
            return;
        }

        _pool.Enqueue(instance);
        instance.OnDespawned();
    }

    internal IReadOnlyCollection<FaraBombComponentModel> GetActiveItems()
    {
        return _activeItems;
    }

    internal void Cleanup()
    {
        foreach (var item in _activeItems) item?.gameObject.SetActive(false);
        _activeItems.Clear();

        foreach (var item in _pool.Where(item => item is not null)) Object.Destroy(item.gameObject);
        _pool.Clear();
    }
}