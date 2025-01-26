using FaraBombRush.Interfaces;
using UnityEngine;

namespace FaraBombRush.Controllers.Components;

public class FaraBombMoveController : FaraBombComponentBase
{
    private const float DefaultNoteJumpSpeed = 18.0f;
    private const float BombDeletePositionZ = -3f;
    private bool _isPaused;

    private float _noteJumpSpeed = DefaultNoteJumpSpeed;

    private void Update()
    {
        if (_isPaused) return;

        Move();
    }

    public override void InitializeComponent()
    {
        _noteJumpSpeed = DefaultNoteJumpSpeed;
        Plugin.Logger.Debug("Initializing BombController");
    }

    private void Move()
    {
        var movement = Vector3.back * (_noteJumpSpeed * Time.deltaTime);

        // ルートオブジェクトの移動のみを行う
        transform.position += movement;
    }

    public bool LimitPosition()
    {
        return transform.position.z < BombDeletePositionZ;
    }

    public void SetNoteJumpSpeed(float speed)
    {
        _noteJumpSpeed = Mathf.Max(0.1f, speed);
    }

    public void SetSongPause(bool isPaused)
    {
        _isPaused = isPaused;
    }
}