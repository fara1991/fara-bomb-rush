using System;
using System.Collections.Generic;
using FaraBombRush.Enums;
using FaraBombRush.Interfaces;
using FaraBombRush.Models;
using UnityEngine;

namespace FaraBombRush.Controllers;

public class FaraBombCoordinatorController : MonoBehaviour
{
    // Components
    private readonly List<IFaraBombComponent> _components = [];
    private FaraBombColliderController _collider;
    private FaraBombEffectController _effect;
    private FaraBombMoveController _movement;

    private FaraBombStateEnum _currentState = FaraBombStateEnum.Idle;

    public void Setup(
        FaraBombColliderController collider = null,
        FaraBombMoveController movement = null,
        FaraBombEffectController effect = null)
    {
        _components.Clear();
        if (collider is not null)
        {
            _collider = collider;
            _components.Add(_collider);
        }

        if (movement is not null)
        {
            _movement = movement;
            _components.Add(_movement);
        }

        if (effect is not null)
        {
            _effect = effect;
            _components.Add(_effect);
        }
    }

    public void InitializeWithCommand(BombCommandModel command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        _currentState = FaraBombStateEnum.Idle;

        // 初期化後すぐに移動状態に遷移
        TransitionTo(FaraBombStateEnum.Move);
    }

    public void UpdateState()
    {
        switch (_currentState)
        {
            case FaraBombStateEnum.Idle:
                break;
            case FaraBombStateEnum.Move:
                HandleMoveState();
                break;
            case FaraBombStateEnum.Explosion:
                HandleExplosionState();
                break;
        }
    }

    private void HandleMoveState()
    {
        if (_movement is not null && _movement.LimitPosition())
            TransitionTo(FaraBombStateEnum.Idle);
        else if (_collider is not null)
        {
            if (_collider.CollisionLeftSaber() || _collider.CollisionRightSaber())
                TransitionTo(FaraBombStateEnum.Explosion);
        }
    }

    private void HandleExplosionState()
    {
        if (_effect is not null && _effect.ExplosionCompleted()) TransitionTo(FaraBombStateEnum.Idle);
    }

    private void TransitionTo(FaraBombStateEnum newState)
    {
        _currentState = newState;
        OnStateEnter(newState);
        Plugin.Logger.Debug($"Enabling {newState.ToString()}");
    }

    private void OnStateEnter(FaraBombStateEnum state)
    {
        switch (state)
        {
            case FaraBombStateEnum.Idle:
                _movement?.Disable();
                _collider?.Disable();
                _effect?.Disable();
                break;
            case FaraBombStateEnum.Move:
                _movement?.Enable();
                _collider?.Enable();
                _effect?.Disable();
                break;
            case FaraBombStateEnum.Explosion:
                _movement?.Disable();
                _collider?.Disable();
                _effect?.Enable();
                break;
        }
    }

    public bool IsInvalid()
    {
        return _currentState == FaraBombStateEnum.Idle;
    }
}