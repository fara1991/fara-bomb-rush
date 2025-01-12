using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Controllers;
using FaraBombRush.Controllers.Components;
using FaraBombRush.Enums;
using static FaraBombRush.Enums.ErrorCodeEnum;
using FaraBombRush.Exceptions;
using FaraBombRush.Models;
using UnityEngine;
using Zenject;

namespace FaraBombRush.Managers;

public class FaraBombSystemManager : MonoBehaviour
{
    private const string FaraBombAssetName = "FaraBomb";
    private const int MaxOnceAddBomb = 3;
    private const int WaitAddBombFrame = 30;
    private static readonly int ShaderColor = Shader.PropertyToID("_Color");

    private readonly string _faraBombAssetPath =
        Path.Combine(Environment.CurrentDirectory, "UserData", "FaraBombRush", "farabomb.particle");

    private readonly string _faraBombScorePath =
        Path.Combine(Environment.CurrentDirectory, "UserData", "FaraBombRush", "score.txt");

    private AssetBundle _assetBundle;
    private PluginConfig _config;

    // プレハブ参照
    private GameObject _faraBombManagementPrefab;

    // プール管理
    private FaraBombManagementPoolController _poolController;
    private FaraBombScoreController _scoreController;
    private int _waitFrame;

    // コマンドキュー
    public static ConcurrentQueue<List<BombCommandModel>> CommandQueue { get; } = new();

    [Inject]
    private void Construct(PluginConfig config)
    {
        _config = config;
    }

    private void Awake()
    {
        if (!_config.IsBombCommandEnable)
        {
            enabled = false;
            return;
        }

        InitializeComponents();
    }

    private void Update()
    {
        if (!enabled || !_config.IsBombCommandEnable) return;

        ProcessCommandQueue();
        UpdateActiveBombs();
        CleanupInvalidBombs();
        SaveFaraBombScore();
    }

    private void OnDestroy()
    {
        _poolController?.Cleanup();
        if (_faraBombManagementPrefab is not null) Destroy(_faraBombManagementPrefab);
        _assetBundle?.Unload(true);
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

            // レンダラーの状態を確認
            var renderers = prefab.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (renderer.gameObject.name != "FaraBombObject") continue;

                var material = renderer.sharedMaterial;
                if (material is null) continue;

                var newMaterial = new Material(material)
                {
                    // FaraBombObject用の設定
                    // シェーダーと基本的な設定のみ変更
                    shader = Shader.Find("Standard")
                };

                // 紫以外ありえない
                if (newMaterial.HasProperty(ShaderColor))
                    newMaterial.SetColor(ShaderColor, new Color(192f / 255f, 64f / 255f, 255f / 255f));

                renderer.material = newMaterial;
            }

            _faraBombManagementPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            _faraBombManagementPrefab.SetActive(false);

            Destroy(prefab);
            Plugin.Logger.Debug("FaraBomb prefab initialized successfully");
        }
        catch (Exception ex)
        {
            throw new FaraBombException(ex.Message, ErrorCode.DoesNotExistPrefab);
        }
    }

    private void InitializeScore()
    {
        _scoreController = gameObject.AddComponent<FaraBombScoreController>();
        _scoreController.Enable();
        Plugin.Logger.Debug("ScoreSystem initialized successfully");
    }

    private void InitializePool()
    {
        _poolController = new FaraBombManagementPoolController();
        _poolController.Initialize(_faraBombManagementPrefab, _config);
        Plugin.Logger.Debug("Pool initialized successfully");
    }

    private void ProcessCommandQueue()
    {
        _waitFrame++;
        if (_waitFrame % WaitAddBombFrame != 0) return;

        _waitFrame = 0;
        if (CommandQueue.TryDequeue(out var commands)) ProcessCommands(commands);
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
        {
            switch (component.GetFaraBombScoreMode())
            {
                case FaraBombCalcScoreEnum.ScoreMode.AddScore:
                    _scoreController.BombThrough();
                    break;
                case FaraBombCalcScoreEnum.ScoreMode.SubtractScore:
                    _scoreController.BombCut();
                    break;
            }
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
        var level = _scoreController.MagnificationByLevel;

        using var writer = new StreamWriter(_faraBombScorePath, false);
        writer.WriteLine(
            $"""
             FaraBombRush Debug
             Score: {score}
             ComboRate: {comboRate}
             ThroughCount: {throughCount}
             ThroughComboCount: {throughComboCount}
             Level: {level}
             """
        );
    }
}