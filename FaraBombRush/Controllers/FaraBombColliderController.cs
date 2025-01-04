using FaraBombRush.Enums;
using FaraBombRush.Interfaces;
using UnityEngine;

namespace FaraBombRush.Controllers;

public class FaraBombColliderController : FaraBombComponentBase
{
    private string _collisionSaberName;
    private bool _isCollidedEnter;
    private SphereCollider _sphereCollider;

    private void OnEnable()
    {
        EnableCollider();
    }

    private void OnDisable()
    {
        DisableCollider();
    }

    private void OnTriggerEnter(Collider other)
    {
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
}