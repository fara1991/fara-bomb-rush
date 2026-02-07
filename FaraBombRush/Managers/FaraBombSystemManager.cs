using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Controllers;
using FaraBombRush.Controllers.Components;
using FaraBombRush.Controllers.Menus;
using FaraBombRush.Enums;
using FaraBombRush.Exceptions;
using FaraBombRush.Models;
using FaraBombRush.Patches;
using UnityEngine;
using Zenject;

namespace FaraBombRush.Managers;

internal class FaraBombSystemManager : MonoBehaviour
{
    private const string FaraBombAssetName = "FaraBomb";
    private const int MaxOnceAddBomb = 3;
    private const int WaitAddBombFrame = 30;

    private readonly string _faraBombAssetPath =
        Path.Combine(Environment.CurrentDirectory, "UserData", "FaraBombRush", "farabomb.particle");

    private readonly string _faraBombScorePath =
        Path.Combine(Environment.CurrentDirectory, "UserData", "FaraBombRush", "score.txt");

    private AssetBundle _assetBundle;
    private PluginConfig _config;
    private AudioTimeSyncController _audioTimeSyncController;
    private BeatmapObjectSpawnController.InitData _spawnInitData;
    private float _njs = 12f;
    private float _bpm = 120f;
    private float _hitZOffset = 0.5f;

    // プレハブ参照
    private GameObject _faraBombManagementPrefab;

    // プール管理
    private FaraBombManagementPoolController _poolController;
    private FaraBombScoreController _scoreController;
    private int _waitFrame;

    // コマンドキュー
    internal static ConcurrentQueue<List<BombCommandModel>> CommandQueue { get; } = new();

    private void Awake()
    {
        // Clear stale commands from previous song
        while (CommandQueue.TryDequeue(out _)) { }
        InitializeComponents();
    }

    private void Update()
    {
        if (!enabled) return;

        FaraBombComponentPause(GamePausePatch.GamePauseStepEnum);
        if (GamePausePatch.GamePauseStepEnum != GamePauseStepEnum.Resume) return;
        ProcessCommandQueue();
        UpdateActiveBombs();
        CleanupInvalidBombs();
        SaveFaraBombScore();
    }

    private void OnDestroy()
    {
        // Clear command queue to prevent stale commands leaking to next song
        while (CommandQueue.TryDequeue(out _)) { }
        _poolController?.Cleanup();
        if (_faraBombManagementPrefab is not null) Destroy(_faraBombManagementPrefab);
        _assetBundle?.Unload(true);
    }

    [Inject]
    private void Construct(
        PluginConfig config,
        AudioTimeSyncController audioTimeSyncController,
        [InjectOptional] BeatmapObjectSpawnController.InitData spawnInitData,
        [InjectOptional] IDifficultyBeatmap difficultyBeatmap)
    {
        _config = config;
        _audioTimeSyncController = audioTimeSyncController;
        _spawnInitData = spawnInitData;

        // Get NJS from spawn data
        if (spawnInitData != null)
        {
            _njs = spawnInitData.noteJumpMovementSpeed;
        }

        // Get BPM from beatmap
        if (difficultyBeatmap?.level != null)
        {
            float bpm = difficultyBeatmap.level.beatsPerMinute;
            _bpm = bpm > 0 ? bpm : 120f;
        }
    }

    private void InitializeComponents()
    {
        InitializePrefabs();
        InitializeScore();
        InitializePool();
    }

    private void InitializePrefabs()
    {
        try
        {
            _assetBundle = AssetBundle.LoadFromFile(_faraBombAssetPath);
            var prefab = _assetBundle.LoadAsset<GameObject>(FaraBombAssetName);

            _faraBombManagementPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            _faraBombManagementPrefab.SetActive(false);

            Destroy(prefab);
            // _assetBundle.Unload(false);
            Plugin.Logger.Info("FaraBomb prefab initialized successfully");
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCodeEnum.DoesNotExistPrefab);
        }
    }

    private void InitializeScore()
    {
        _scoreController = gameObject.AddComponent<FaraBombScoreController>();
        _scoreController.Enable();
        Plugin.Logger.Info("ScoreSystem initialized successfully");
    }

    private void InitializePool()
    {
        // マーカーと同様に BeatmapObjectSpawnCenter から HitZOffset を取得する
        _hitZOffset = 0.5f;
        var spawnCenter = GameObject.FindObjectOfType<BeatmapObjectSpawnCenter>();
        if (spawnCenter != null)
        {
            _hitZOffset = spawnCenter.transform.position.z + 0.5f;
            Plugin.Logger.Info($"Found BeatmapObjectSpawnCenter at Z: {spawnCenter.transform.position.z}. Set HitZOffset to: {_hitZOffset}");
        }

        _poolController = new FaraBombManagementPoolController();
        _poolController.Initialize(_faraBombManagementPrefab, _config, _audioTimeSyncController, _njs, _hitZOffset);
        Plugin.Logger.Info($"Pool initialized successfully with NJS: {_njs}, HitZOffset: {_hitZOffset}");
    }

    private void ProcessCommandQueue()
    {
        _waitFrame++;
        if (_waitFrame % WaitAddBombFrame != 0) return;

        _waitFrame = 0;

        if (CommandQueue.TryPeek(out var commands))
        {
            // Check if any command in the next group is ready to be spawned
            // We use the first command's HitTime as a reference for the group
            var firstCommand = commands.FirstOrDefault();
            if (firstCommand != null)
            {
                float songTime = _audioTimeSyncController?.songTime ?? 0f;
                float timeToHit = firstCommand.HitTime - songTime;

                // マーカーと同様に4拍前でSpawnさせる
                // 4拍の時間は (60/BPM)*4
                float lookAheadTime = (60f / _bpm) * 4f;

                if (timeToHit <= lookAheadTime)
                {
                    if (CommandQueue.TryDequeue(out var readyCommands))
                    {
                        ProcessCommands(readyCommands);
                    }
                }
            }
            else
            {
                // Empty list, just dequeue and ignore
                CommandQueue.TryDequeue(out _);
            }
        }
    }

    private void ProcessCommands(List<BombCommandModel> commands)
    {
        // 出現する特定のbombIdを取得
        var selectedBombIds = commands
            .Select(cmd => cmd.BombId)
            .Distinct()
            .Take(MaxOnceAddBomb)
            .ToList();

        // 選択されたbombIdに対応するコマンドを処理
        foreach (var command in commands.Where(cmd => selectedBombIds.Contains(cmd.BombId)))
            _poolController.Spawn(command);
    }

    private void UpdateActiveBombs()
    {
        foreach (var component in _poolController.GetActiveItems()) component.UpdateState();
    }

    private void CleanupInvalidBombs()
    {
        var activeItems = _poolController.GetActiveItems();
        foreach (var component in activeItems)
            switch (component.GetFaraBombScoreMode())
            {
                case FaraBombCalcScoreEnum.AddScore:
                    _scoreController.BombThrough();
                    break;
                case FaraBombCalcScoreEnum.SubtractScore:
                    _scoreController.BombCut();
                    break;
            }

        var invalidBombs = activeItems
            .Where(bomb => bomb.IsInvalid())
            .ToList();

        foreach (var bomb in invalidBombs) _poolController.Despawn(bomb);
    }

    private void SaveFaraBombScore()
    {
        var score = _scoreController.FaraBombScore;
        var comboRate = _scoreController.FaraBombComboRate;
        var throughCount = _scoreController.FaraBombThroughCount;
        var throughComboCount = _scoreController.FaraBombThroughComboCount;

        using var writer = new StreamWriter(_faraBombScorePath, false);
        writer.WriteLine(
            $"""
             Debug
             Score: {score.ToString()}
             ComboRate: {comboRate}
             ThroughCount: {throughCount}
             ThroughComboCount: {throughComboCount}
             Level: {FaraBombLevelEnumHelper.GetLevelEnum().ToString()}
             """
        );
    }

    private void FaraBombComponentPause(GamePauseStepEnum gamePauseStepEnum)
    {
        _poolController.SetPause(gamePauseStepEnum);
    }
}
