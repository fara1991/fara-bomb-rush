using FaraBombRush.Enums;
using FaraBombRush.Interfaces;
using UnityEngine;

namespace FaraBombRush.Controllers;

public class FaraBombColliderController : FaraBombComponentBase
{
    private bool _isCollidedEnter;
    private string _collisionSaberName;
    private SphereCollider _sphereCollider;

    public override void Initialize()
    {
        // コライダーはbombObjectについているものを使用
        _sphereCollider ??= transform.GetComponent<SphereCollider>();
        EnableCollider();
        Plugin.Logger.Debug("Initializing ColliderController");
    }

    private void EnableCollider()
    {
        _isCollidedEnter = false;
        _collisionSaberName = string.Empty;
        _sphereCollider.enabled = true;
        Plugin.Logger.Debug("Enable ColliderController");
    }

    private void DisableCollider()
    {
        _sphereCollider.enabled = false;
        _isCollidedEnter = false;
        _collisionSaberName = string.Empty;
        Plugin.Logger.Debug("Disable ColliderController");
    }

    private void OnTriggerEnter(Collider other)
    {
        Plugin.Logger.Debug("TriggerEnter ColliderController");
        Plugin.Logger.Debug($"TriggerEnter {other.gameObject.name}");
        if (_isCollidedEnter) return;
        if (!IsSaberCollider(other))
        {
            Plugin.Logger.Debug($"Non-saber collision detected with {other.gameObject.name}");
            return;
        }

        Plugin.Logger.Debug($"Saber collision detected: {other.gameObject.name}");
            
        _collisionSaberName = other.gameObject.name;
        _isCollidedEnter = true;
    }

    private bool IsSaberCollider(Collider collider)
    {
        return collider.gameObject.name == SaberObjectEnum.LeftSaber.ToString() ||
               collider.gameObject.name == SaberObjectEnum.RightSaber.ToString();
    }

    public bool CollisionLeftSaber()
    {
        return _isCollidedEnter && _collisionSaberName == SaberObjectEnum.LeftSaber.ToString();
    }

    public bool CollisionRightSaber()
    {
        return _isCollidedEnter && _collisionSaberName == SaberObjectEnum.RightSaber.ToString();
    }

    public override void Enable()
    {
        base.Enable();
        gameObject.SetActive(true);
    }

    public override void Disable()
    {
        base.Disable();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        EnableCollider();
    }
        
    private void OnDisable()
    {
        DisableCollider();
    }
}