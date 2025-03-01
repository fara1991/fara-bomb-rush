using System;
using System.Collections.Generic;
using FaraBombRush.Controllers.Components;
using FaraBombRush.Enums;
using FaraBombRush.Models;
using UnityEngine;

namespace FaraBombRush.Controllers;

internal class FaraBombCoordinatorController : MonoBehaviour
{
    // Components
    private readonly List<FaraBombComponentBaseController> _components = [];
    private FaraBombCalcScoreEnum _calcScoreParameter = FaraBombCalcScoreEnum.None;
    private FaraBombColliderController _colliderComponent;

    private FaraBombStateEnum _currentStateEnum = FaraBombStateEnum.Idle;
    private FaraBombEffectController _effectComponent;
    private FaraBombMoveController _movementComponent;

    internal void Setup(
        FaraBombColliderController colliderComponent = null,
        FaraBombMoveController movementComponent = null,
        FaraBombEffectController effectComponent = null)
    {
        _components.Clear();
        if (colliderComponent is not null)
        {
            _colliderComponent = colliderComponent;
            _components.Add(_colliderComponent);
            _colliderComponent.Enable();
        }

        if (movementComponent is not null)
        {
            _movementComponent = movementComponent;
            _components.Add(_movementComponent);
            _movementComponent.Enable();
        }

        if (effectComponent is not null)
        {
            _effectComponent = effectComponent;
            _components.Add(_effectComponent);
            _effectComponent.Enable();
        }
    }

    internal void InitializeWithCommand(BombCommandModel command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        _currentStateEnum = FaraBombStateEnum.Idle;

        // 初期化後すぐに移動状態に遷移
        TransitionTo(FaraBombStateEnum.Move);
    }

    internal void UpdateState()
    {
        switch (_currentStateEnum)
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
        if (_movementComponent is not null && _movementComponent.LimitPosition())
        {
            TransitionTo(FaraBombStateEnum.Idle);
            _calcScoreParameter = FaraBombCalcScoreEnum.AddScore;
        }
        else if (_colliderComponent is not null)
        {
            if (!_colliderComponent.CollisionLeftSaber() && !_colliderComponent.CollisionRightSaber()) return;
            TransitionTo(FaraBombStateEnum.Explosion);
            _calcScoreParameter = FaraBombCalcScoreEnum.SubtractScore;
        }
    }

    private void HandleExplosionState()
    {
        if (_effectComponent is not null && _effectComponent.ExplosionCompleted()) TransitionTo(FaraBombStateEnum.Idle);
    }

    private void TransitionTo(FaraBombStateEnum newStateEnum)
    {
        _currentStateEnum = newStateEnum;
        OnStateEnter(newStateEnum);
    }

    private void OnStateEnter(FaraBombStateEnum stateEnum)
    {
        switch (stateEnum)
        {
            case FaraBombStateEnum.Idle:
                _movementComponent?.Disable();
                _colliderComponent?.Disable();
                _effectComponent?.Disable();
                break;
            case FaraBombStateEnum.Move:
                _movementComponent?.Enable();
                _colliderComponent?.Enable();
                _effectComponent?.Disable();
                break;
            case FaraBombStateEnum.Explosion:
                _movementComponent?.Disable();
                _colliderComponent?.Disable();
                _effectComponent?.Enable();
                break;
        }
    }

    internal bool IsInvalid()
    {
        return _currentStateEnum == FaraBombStateEnum.Idle;
    }

    internal FaraBombCalcScoreEnum GetScoreMode()
    {
        var scoreMode = _calcScoreParameter;
        _calcScoreParameter = FaraBombCalcScoreEnum.None;
        return scoreMode;
    }
}