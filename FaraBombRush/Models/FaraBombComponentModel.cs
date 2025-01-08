using FaraBombRush.Configs;
using FaraBombRush.Controllers;
using FaraBombRush.Controllers.Components;
using UnityEngine;

namespace FaraBombRush.Models;

/// <summary>
///     FaraBombの全コンポーネントを管理するクラス
/// </summary>
public class FaraBombComponentModel : MonoBehaviour
{
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

    public void Initialize(GameObject rootObject, GameObject bombObject, GameObject effectObject, PluginConfig pluginConfig)
    {
        RootObject = rootObject;
        BombObject = bombObject;
        EffectObject = effectObject;
        RootObject.transform.position = Vector3.zero;
        BombObject.transform.position = Vector3.zero;
        EffectObject.transform.position = Vector3.zero;

        Coordinator = RootObject.AddComponent<FaraBombCoordinatorController>();
        Move = RootObject.AddComponent<FaraBombMoveController>();
        Effect = EffectObject.AddComponent<FaraBombEffectController>();
        if (pluginConfig.IsBombCutEnable)
        {
            Collider = BombObject.AddComponent<FaraBombColliderController>();
        }

        Coordinator.Setup(Collider, Move, Effect);
        Collider?.Initialize();
        Move?.Initialize();
        Effect?.Initialize();
    }

    public void OnSpawned()
    {
        EnableComponents();
        RootObject?.SetActive(true);
        BombObject?.SetActive(true);
        EffectObject?.SetActive(true);
    }

    public void OnDespawned()
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

    public void InitializeWithCommand(BombCommandModel command)
    {
        Coordinator?.InitializeWithCommand(command);
    }

    public void UpdateState()
    {
        Coordinator?.UpdateState();
    }

    public bool IsInvalid()
    {
        return Coordinator.IsInvalid();
    }
}