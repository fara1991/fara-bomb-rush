using FaraBombRush.Configs;
using FaraBombRush.Controllers;
using FaraBombRush.Controllers.Components;
using FaraBombRush.Enums;
using UnityEngine;

namespace FaraBombRush.Models;

/// <summary>
///     FaraBombの全コンポーネントを管理するクラス
/// </summary>
internal class FaraBombComponentModel : MonoBehaviour
{
    private GamePauseStepEnum _gamePauseStepEnum = GamePauseStepEnum.Resume;
    private AudioTimeSyncController _audioTimeSyncController;
    private float _njs = 12f;
    private float _hitZOffset;

    // 各コンポーネントへの参照
    private FaraBombCoordinatorController Coordinator { get; set; }
    private FaraBombColliderController Collider { get; set; }
    private FaraBombMoveController Move { get; set; }
    private FaraBombEffectController Effect { get; set; }

    // 各GameObject参照
    private GameObject RootObject { get; set; }
    private GameObject BombObject { get; set; }
    private GameObject EffectObject { get; set; }

    private void OnDisable()
    {
        DisableComponents();
    }

    internal void Initialize(GameObject rootObject, GameObject bombObject, GameObject effectObject,
        PluginConfig pluginConfig, AudioTimeSyncController audioTimeSyncController = null, float njs = 12f, float hitZOffset = 0.5f)
    {
        RootObject = rootObject;
        BombObject = bombObject;
        EffectObject = effectObject;
        _audioTimeSyncController = audioTimeSyncController;
        _njs = njs;
        _hitZOffset = hitZOffset;

        RootObject.transform.position = Vector3.zero;
        BombObject.transform.position = Vector3.zero;
        EffectObject.transform.position = Vector3.zero;

        Coordinator = RootObject.AddComponent<FaraBombCoordinatorController>();
        Move = RootObject.AddComponent<FaraBombMoveController>();
        Effect = EffectObject.AddComponent<FaraBombEffectController>();
        if (pluginConfig.IsBombCutEnable) Collider = BombObject.AddComponent<FaraBombColliderController>();

        Collider?.Initialize(pluginConfig);
        Move?.Initialize(pluginConfig);
        Effect?.Initialize(pluginConfig);
        Coordinator.Setup(Collider, Move, Effect);
    }

    internal void OnSpawned()
    {
        EnableComponents();
        RootObject?.SetActive(true);
        BombObject?.SetActive(true);
        EffectObject?.SetActive(true);
    }

    internal void OnDespawned()
    {
        DisableComponents();
        RootObject?.SetActive(false);
        BombObject?.SetActive(false);
        EffectObject?.SetActive(false);
    }

    private void EnableComponents()
    {
        if (Coordinator is not null) Coordinator.enabled = true;
        if (Collider is not null) Collider.enabled = true;
        if (Move is not null) Move.enabled = true;
        if (Effect is not null) Effect.enabled = true;
    }

    private void DisableComponents()
    {
        if (Coordinator is not null) Coordinator.enabled = false;
        if (Collider is not null) Collider.enabled = false;
        if (Move is not null) Move.enabled = false;
        if (Effect is not null) Effect.enabled = false;
    }

    internal void InitializeWithCommand(BombCommandModel command)
    {
        // Setup movement with timing information
        if (Move != null && _audioTimeSyncController != null && command.HitTime > 0)
        {
            Move.SetupMovement(_njs, _audioTimeSyncController, command.HitTime, _hitZOffset);
        }

        Coordinator?.InitializeWithCommand(command);
    }

    internal void UpdateState()
    {
        Coordinator?.UpdateState();
    }

    internal bool IsInvalid()
    {
        return Coordinator.IsInvalid();
    }

    internal void SetPause(GamePauseStepEnum gamePauseStepEnum)
    {
        if (_gamePauseStepEnum == gamePauseStepEnum) return;

        switch (gamePauseStepEnum)
        {
            case GamePauseStepEnum.Pause:
                RootObject.SetActive(false);
                break;
            case GamePauseStepEnum.WillResume:
                RootObject.SetActive(true);
                if (Coordinator is not null) Coordinator.enabled = true;
                if (Collider is not null) Collider.enabled = false;
                if (Move is not null) Move.enabled = false;
                if (Effect is not null) Effect.enabled = false;
                break;
            case GamePauseStepEnum.Resume:
                RootObject.SetActive(true);
                if (Coordinator is not null) Coordinator.enabled = true;
                if (Collider is not null) Collider.enabled = true;
                if (Move is not null) Move.enabled = true;
                if (Effect is not null) Effect.enabled = true;
                break;
        }

        _gamePauseStepEnum = gamePauseStepEnum;
    }

    internal FaraBombCalcScoreEnum GetFaraBombScoreMode()
    {
        return Coordinator.GetScoreMode();
    }
}
