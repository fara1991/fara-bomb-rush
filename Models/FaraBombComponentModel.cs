using FaraBombRush.Configs;
using FaraBombRush.Controllers;
using UnityEngine;
using Zenject;

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
    private PluginConfig _config;

    [Inject]
    private void Construct(PluginConfig config)
    {
        _config = config;
    }

    private void OnDisable()
    {
        DisableComponents();
    }

    public void Initialize(GameObject rootObject, GameObject bombObject, GameObject effectObject)
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
        if (!_config.IsBombCutEnable)
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
        Coordinator.enabled = true;
        Collider.enabled = true;
        Move.enabled = true;
        Effect.enabled = true;
    }

    private void DisableComponents()
    {
        Coordinator.enabled = false;
        Collider.enabled = false;
        Move.enabled = false;
        Effect.enabled = false;
    }

    public void InitializeWithCommand(BombCommandModel command)
    {
        Coordinator.InitializeWithCommand(command);
    }

    public void UpdateState()
    {
        Coordinator.UpdateState();
    }

    public bool IsInvalid()
    {
        return Coordinator.IsInvalid();
    }
}