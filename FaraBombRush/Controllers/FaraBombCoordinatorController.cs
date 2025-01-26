using System;
using System.Collections.Generic;
using FaraBombRush.Enums;
using FaraBombRush.Interfaces;
using FaraBombRush.Models;
using UnityEngine;
using FaraBombRush.Controllers.Components;
using static FaraBombRush.Enums.FaraBombCalcScoreEnum;
using UnityEngine.Serialization;

namespace FaraBombRush.Controllers;

public class FaraBombCoordinatorController : MonoBehaviour
{
    // Components
    private readonly List<IFaraBombComponent> _components = [];
    private FaraBombColliderController _colliderComponent;
    private FaraBombEffectController _effectComponent;
    private FaraBombMoveController _movementComponent;

    private FaraBombStateEnum _currentStateEnum = FaraBombStateEnum.Idle;
    private ScoreMode _calcScoreMode = ScoreMode.None;

    public void Setup(
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

    public void InitializeWithCommand(BombCommandModel command)
    {
        if (command is null) throw new ArgumentNullException(nameof(command));

        _currentStateEnum = FaraBombStateEnum.Idle;

        // 初期化後すぐに移動状態に遷移
        TransitionTo(FaraBombStateEnum.Move);
    }

    public void UpdateState()
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
            _calcScoreMode = ScoreMode.AddScore;
        }
        else if (_colliderComponent is not null)
        {
            if (!_colliderComponent.CollisionLeftSaber() && !_colliderComponent.CollisionRightSaber()) return;
            TransitionTo(FaraBombStateEnum.Explosion);
            _calcScoreMode = ScoreMode.SubtractScore;
        }
    }

    private void HandleExplosionState()
    {
        if (_effectComponent is not null && _effectComponent.ExplosionCompleted()) TransitionTo(FaraBombStateEnum.Idle);
    }

    private void TransitionTo(FaraBombStateEnum newState)
    {
        _currentStateEnum = newState;
        OnStateEnter(newState);
        Plugin.Logger.Debug($"Enabling {newState.ToString()}");
    }

    private void OnStateEnter(FaraBombStateEnum state)
    {
        switch (state)
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

    public bool IsInvalid()
    {
        return _currentStateEnum == FaraBombStateEnum.Idle;
    }

    public ScoreMode GetScoreMode()
    {
        var scoreMode = _calcScoreMode;
        _calcScoreMode = ScoreMode.None;
        return scoreMode;
    }
}