using System;
using System.Collections.Generic;
using System.IO;
using FaraBombRush.Configs;
using FaraBombRush.Enums;
using UnityEngine;
using Zenject;

namespace FaraBombRush.Services;

// Trigger detector attached per bomb — fires on any collider enter (VR controller)
internal class MenuBombHitDetector : MonoBehaviour
{
    internal bool WasHit { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        WasHit = true;
    }
}

internal class MenuBombPreviewService : MonoBehaviour
{
    private const string FaraBombAssetName = "FaraBomb";
    private const float BombSpeed = 4f;
    private const float SpawnZ = 6f;
    private const float DeleteZ = -2f;
    private const float SpawnInterval = 0.7f;
    private const float ExplosionDuration = 2f;

    private readonly string _assetPath =
        Path.Combine(Environment.CurrentDirectory, "UserData", "FaraBombRush", "farabomb.particle");

    private static readonly NotePositionEnum[] PreviewPositions =
    [
        NotePositionEnum.TopLeft,
        NotePositionEnum.TopRight,
        NotePositionEnum.CenterLeft,
        NotePositionEnum.CenterRight,
        NotePositionEnum.BottomLeft,
        NotePositionEnum.BottomRight,
    ];

    private class BombEntry
    {
        public GameObject Root;
        public GameObject BombMesh;
        public GameObject Effect;
        public MenuBombHitDetector HitDetector;
        public bool IsExploding;
        public float ExplosionTimer;
    }

    private PluginConfig _config;
    private GameObject _prefab;
    private GameObject _container;
    private readonly List<BombEntry> _activeBombs = new List<BombEntry>();
    private readonly List<BombEntry> _toRemove = new List<BombEntry>();
    private float _spawnTimer;
    private int _positionIndex;
    private bool _isActive;

    [Inject]
    private void Construct(PluginConfig config)
    {
        _config = config;
    }

    internal void UpdateEnabled(bool enabled)
    {
        if (enabled) Show();
        else Hide();
    }

    internal void Show()
    {
        _isActive = true;
        if (_container != null)
            _container.SetActive(true);
    }

    internal void Hide()
    {
        _isActive = false;
        if (_container != null)
            _container.SetActive(false);
        ClearAllBombs();
    }

    private void Awake()
    {
        _container = new GameObject("FaraBombRush_MenuPreviewContainer");
        _container.SetActive(false);
        TryLoadAsset();
    }

    private void TryLoadAsset()
    {
        try
        {
            if (!File.Exists(_assetPath))
            {
                Plugin.Logger.Warn("MenuPreview: farabomb.particle not found");
                return;
            }

            var bundle = AssetBundle.LoadFromFile(_assetPath);
            if (bundle == null) return;
            _prefab = bundle.LoadAsset<GameObject>(FaraBombAssetName);
            bundle.Unload(false);
            Plugin.Logger.Info("MenuPreview: Asset loaded successfully");
        }
        catch (Exception ex)
        {
            Plugin.Logger.Warn($"MenuPreview: Failed to load asset - {ex.Message}");
        }
    }

    private void Update()
    {
        if (!_isActive || _prefab == null) return;

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= SpawnInterval)
        {
            _spawnTimer = 0f;
            SpawnBomb();
        }

        UpdateBombs();
    }

    private void SpawnBomb()
    {
        var notePos = PreviewPositions[_positionIndex % PreviewPositions.Length].GetNotePosition();
        _positionIndex++;

        var root = Instantiate(_prefab, new Vector3(notePos.x, notePos.y, SpawnZ), Quaternion.identity,
            _container.transform);

        if (root.transform.childCount < 2)
        {
            Destroy(root);
            return;
        }

        var bombMesh = root.transform.GetChild(0).gameObject;
        var effect = root.transform.GetChild(1).gameObject;

        // Effect is hidden until hit
        effect.SetActive(false);

        // Setup collider as trigger for saber detection
        MenuBombHitDetector hitDetector = null;
        if (_config.IsBombCutEnable)
        {
            var col = bombMesh.GetComponent<SphereCollider>();
            if (col != null)
            {
                col.isTrigger = true;
                hitDetector = bombMesh.AddComponent<MenuBombHitDetector>();
            }
        }

        root.SetActive(true);
        bombMesh.SetActive(true);

        _activeBombs.Add(new BombEntry
        {
            Root = root,
            BombMesh = bombMesh,
            Effect = effect,
            HitDetector = hitDetector,
        });
    }

    private void UpdateBombs()
    {
        _toRemove.Clear();

        foreach (var entry in _activeBombs)
        {
            if (entry.Root == null)
            {
                _toRemove.Add(entry);
                continue;
            }

            if (entry.IsExploding)
            {
                entry.ExplosionTimer += Time.deltaTime;
                if (entry.ExplosionTimer >= ExplosionDuration)
                {
                    Destroy(entry.Root);
                    _toRemove.Add(entry);
                }

                continue;
            }

            // Check saber hit (ValidBombCollision ON only)
            if (_config.IsBombCutEnable && entry.HitDetector != null && entry.HitDetector.WasHit)
            {
                entry.BombMesh.SetActive(false);
                entry.Effect.SetActive(true);
                entry.IsExploding = true;
                continue;
            }

            // Move bomb forward
            entry.Root.transform.position += Vector3.back * (BombSpeed * Time.deltaTime);

            if (entry.Root.transform.position.z < DeleteZ)
            {
                Destroy(entry.Root);
                _toRemove.Add(entry);
            }
        }

        foreach (var entry in _toRemove)
            _activeBombs.Remove(entry);
    }

    private void ClearAllBombs()
    {
        foreach (var entry in _activeBombs)
            if (entry.Root != null) Destroy(entry.Root);
        _activeBombs.Clear();
        _spawnTimer = 0f;
    }

    private void OnDestroy()
    {
        ClearAllBombs();
        if (_container != null) Destroy(_container);
    }
}
