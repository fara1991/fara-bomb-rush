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
    internal static ConcurrentQueue<List<BombCommandModel>> CommandQueue { get; } = new();

    private void Awake()
    {
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
        _poolController?.Cleanup();
        if (_faraBombManagementPrefab is not null) Destroy(_faraBombManagementPrefab);
        _assetBundle?.Unload(true);
    }

    [Inject]
    private void Construct(PluginConfig config)
    {
        _config = config;
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
        _poolController = new FaraBombManagementPoolController();
        _poolController.Initialize(_faraBombManagementPrefab, _config);
        Plugin.Logger.Info("Pool initialized successfully");
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
             FaraBombRush Debug
             Score: {score}
             ComboRate: {comboRate}
             ThroughCount: {throughCount}
             ThroughComboCount: {throughComboCount}
             Level: {FaraBombLevelEnumHelper.GetLevelEnum(LoadSteamController.ScoreSaberPP).ToString()}
             """
        );
    }

    private void FaraBombComponentPause(GamePauseStepEnum gamePauseStepEnum)
    {
        _poolController.SetPause(gamePauseStepEnum);
    }
}