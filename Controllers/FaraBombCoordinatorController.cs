using System;
using System.Collections.Generic;
using FaraBombRush.Enums;
using FaraBombRush.Interfaces;
using FaraBombRush.Models;
using UnityEngine;

namespace FaraBombRush.Controllers;

public class FaraBombCoordinatorController : MonoBehaviour
{
    private FaraBombColliderController _collider;
    private FaraBombMoveController _movement;
    private FaraBombEffectController _effect;
    private FaraBombStateEnum _currentState = FaraBombStateEnum.Idle;
        
    private readonly List<IFaraBombComponent> _components = [];
    private bool _isInitialized;

    public void Setup(
        FaraBombColliderController collider,
        FaraBombMoveController movement,
        FaraBombEffectController effect)
    {
        _collider = collider ?? throw new ArgumentNullException(nameof(collider));
        _movement = movement ?? throw new ArgumentNullException(nameof(movement));
        _effect = effect ?? throw new ArgumentNullException(nameof(effect));

        _components.Clear();
        _components.Add(_collider);
        _components.Add(_movement);
        _components.Add(_effect);
    }

    public void InitializeWithCommand(BombCommandModel command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));
            
        _currentState = FaraBombStateEnum.Idle;
        _isInitialized = true;

        // 初期化後すぐに移動状態に遷移
        TransitionTo(FaraBombStateEnum.Move);
    }

    public void UpdateState()
    {
        if (!_isInitialized) return;

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
        if (_movement.LimitPosition())
        {
            TransitionTo(FaraBombStateEnum.Idle);
        }
        else if (_collider.CollisionLeftSaber() || _collider.CollisionRightSaber())
        {
            TransitionTo(FaraBombStateEnum.Explosion);
        }
    }

    private void HandleExplosionState()
    {
        if (_effect.ExplosionCompleted())
        {
            TransitionTo(FaraBombStateEnum.Idle);
        }
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
                _movement.Disable();
                _collider.Disable();
                _effect.Disable();
                break;
            case FaraBombStateEnum.Move:
                _movement.Enable();
                _collider.Enable();
                _effect.Disable();
                break;
            case FaraBombStateEnum.Explosion:
                _movement.Disable();
                _collider.Disable();
                _effect.Enable();
                break;
        }
    }

    public bool IsInvalid() => _currentState == FaraBombStateEnum.Idle;
}