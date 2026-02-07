using FaraBombRush.Enums;
using UnityEngine;

namespace FaraBombRush.Controllers.Components;

internal class FaraBombColliderController : FaraBombComponentBaseController
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
        if (_isCollidedEnter || !IsSaberCollider(other)) return;

        _collisionSaberName = other.gameObject.name;
        _isCollidedEnter = true;
    }

    protected override void InitializeComponent()
    {
        // コライダーはbombObjectについているものを使用
        _sphereCollider ??= transform.GetComponent<SphereCollider>();
        EnableCollider();
    }

    private void EnableCollider()
    {
        _isCollidedEnter = false;
        _collisionSaberName = string.Empty;
        _sphereCollider.enabled = true;
    }

    private void DisableCollider()
    {
        _sphereCollider.enabled = false;
        _isCollidedEnter = false;
        _collisionSaberName = string.Empty;
    }

    private bool IsSaberCollider(Collider collider)
    {
        return collider.gameObject.name == SaberObjectEnum.LeftSaber.ToString() ||
               collider.gameObject.name == SaberObjectEnum.RightSaber.ToString();
    }

    internal bool CollisionLeftSaber()
    {
        return _isCollidedEnter && _collisionSaberName == SaberObjectEnum.LeftSaber.ToString();
    }

    internal bool CollisionRightSaber()
    {
        return _isCollidedEnter && _collisionSaberName == SaberObjectEnum.RightSaber.ToString();
    }

    protected internal override void Enable()
    {
        base.Enable();
        gameObject.SetActive(true);
    }

    protected internal override void Disable()
    {
        base.Disable();
        gameObject.SetActive(false);
    }
}
