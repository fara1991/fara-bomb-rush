using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FaraBombRush.Configs;
using FaraBombRush.Controllers;
using FaraBombRush.Models;
using UnityEngine;
using Zenject;

namespace FaraBombRush.Managers;

public class FaraBombPoolManager : MonoBehaviour
{
    private const string FaraBombAssetName = "FaraBomb";
    private const int MaxOnceAddBomb = 3;
    private const int WaitAddBombFrame = 30;
    private static readonly int ShaderColor = Shader.PropertyToID("_Color");

    private readonly string _faraBombAssetPath =
        Path.Combine(Environment.CurrentDirectory, "UserData", "FaraBombRush", "farabomb.particle");

    private AssetBundle _assetBundle;
    private PluginConfig _config;

    // プレハブ参照
    private GameObject _faraBombManagementPrefab;

    // プール管理
    private FaraBombManagementPoolController _poolController;
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

        try
        {
            InitializeComponents();
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to initialize FaraBombPoolManager");
            Plugin.Logger.Error(ex);
            enabled = false;
        }
    }

    private void Update()
    {
        if (!enabled || !_config.IsBombCommandEnable) return;

        try
        {
            ProcessCommandQueue();
            UpdateActiveBombs();
            CleanupInvalidBombs();
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Error in Update");
            Plugin.Logger.Error(ex);
        }
    }

    private void OnDestroy()
    {
        try
        {
            _poolController?.Cleanup();
            if (_faraBombManagementPrefab is not null) Destroy(_faraBombManagementPrefab);
            _assetBundle?.Unload(true);
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Error during cleanup");
            Plugin.Logger.Error(ex);
        }
    }
    
    private void InitializeComponents()
    {
        InitializePrefabs();
        InitializePool();
    }

    private void InitializePrefabs()
    {
        _assetBundle = AssetBundle.LoadFromFile(_faraBombAssetPath);
        if (_assetBundle is null)
            throw new InvalidOperationException($"Failed to load AssetBundle from {_faraBombAssetPath}");

        try
        {
            // マテリアル・テクスチャの読み込み状態をログ出力
            LogAssetBundleContents(_assetBundle);

            var prefab = _assetBundle.LoadAsset<GameObject>(FaraBombAssetName);
            if (prefab is null) throw new InvalidOperationException("FaraBombEffect asset not found in bundle");

            // レンダラーの状態を確認
            var renderers = prefab.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                LogRendererState(renderer);
                if (renderer.gameObject.name != "FaraBombObject") continue;

                var material = renderer.sharedMaterial;
                if (material is null) continue;

                var newMaterial = new Material(material)
                {
                    // FaraBombObject用の設定
                    // シェーダーと基本的な設定のみ変更
                    shader = Shader.Find("Standard")
                };

                if (newMaterial.HasProperty(ShaderColor))
                    newMaterial.SetColor(ShaderColor, new Color(192f / 255f, 64f / 255f, 255f / 255f));

                renderer.material = newMaterial;
            }

            _faraBombManagementPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            _faraBombManagementPrefab.SetActive(false);

            Destroy(prefab);
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to load effect prefab");
            Plugin.Logger.Error(ex);
            throw;
        }
    }

    private void LogAssetBundleContents(AssetBundle assetBundle)
    {
        var materials = assetBundle.LoadAllAssets<Material>();
        foreach (var material in materials) Plugin.Logger.Debug($"Loaded material: {material.name}");

        var textures = assetBundle.LoadAllAssets<Texture>();
        foreach (var texture in textures) Plugin.Logger.Debug($"Loaded texture: {texture.name}");
    }

    private void LogRendererState(Renderer renderer)
    {
        foreach (var material in renderer.sharedMaterials)
        {
            if (material == null) continue;

            Plugin.Logger.Debug($"Found renderer in {renderer.gameObject.name}");
            Plugin.Logger.Debug($"Material: {material.name} on {renderer.gameObject.name}");
            Plugin.Logger.Debug($"Shader: {material.shader.name}");

            var mainTex = material.GetTexture("_MainTex");
            Plugin.Logger.Debug($"Has MainTex: {mainTex != null}");
            if (mainTex != null) Plugin.Logger.Debug($"MainTex size: {mainTex.width}x{mainTex.height}");
        }
    }

    private void InitializePool()
    {
        try
        {
            _poolController = new FaraBombManagementPoolController();
            _poolController.Initialize(_faraBombManagementPrefab);
            Plugin.Logger.Debug("Pool initialized successfully");
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error("Failed to initialize pool");
            Plugin.Logger.Error(ex);
            throw;
        }
    }

    private void ProcessCommandQueue()
    {
        if (!ShouldProcessCommands()) return;

        _waitFrame = 0;

        if (CommandQueue.TryDequeue(out var commands)) ProcessCommands(commands);
    }

    private bool ShouldProcessCommands()
    {
        _waitFrame++;
        return _waitFrame % WaitAddBombFrame == 0;
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
        foreach (var command in commands.Where(cmd => selectedBombIds.Contains(cmd.BombId))) SpawnBomb(command);
    }

    private void SpawnBomb(BombCommandModel command)
    {
        try
        {
            _poolController.Spawn(command);
        }
        catch (Exception ex)
        {
            Plugin.Logger.Error($"Failed to spawn bomb {command.BombId}");
            Plugin.Logger.Error(ex);
        }
    }

    private void UpdateActiveBombs()
    {
        foreach (var component in _poolController.GetActiveItems()) component.UpdateState();
    }

    private void CleanupInvalidBombs()
    {
        var invalidBombs = _poolController.GetActiveItems()
            .Where(bomb => bomb.IsInvalid())
            .ToList();

        foreach (var bomb in invalidBombs) _poolController.Despawn(bomb);
    }
}