using UnityEngine;

namespace FaraBombRush.Controllers.Components;

internal class FaraBombMoveController : FaraBombComponentBaseController
{
    private const float DefaultNoteJumpSpeed = 20.0f;
    private const float BombDeletePositionZ = -3f;

    private float _njs = DefaultNoteJumpSpeed;
    private AudioTimeSyncController _audioTimeSyncController;
    private float _hitTime;
    private float _hitZOffset;

    internal void SetupMovement(float njs, AudioTimeSyncController audioTimeSyncController, float hitTime, float hitZOffset)
    {
        _njs = njs;
        _audioTimeSyncController = audioTimeSyncController;
        _hitTime = hitTime;
        _hitZOffset = hitZOffset;
    }

    private void Update()
    {
        Move();
    }

    protected override void InitializeComponent()
    {
    }

    private void Move()
    {
        if (_audioTimeSyncController != null)
        {
            // Calculate position based on remaining time to hit
            float songTime = _audioTimeSyncController.songTime;
            float timeLeft = _hitTime - songTime;

            // Adjust NJS by time scale (for practice mode speed changes)
            float timeScale = _audioTimeSyncController.timeScale;
            float adjustedNjs = timeScale > 0.001f ? _njs / timeScale : _njs;

            // Calculate Z position: (time left * speed) + offset
            float targetZ = (timeLeft * adjustedNjs) + _hitZOffset;

            // Update position
            var currentPos = transform.position;
            transform.position = new Vector3(currentPos.x, currentPos.y, targetZ);
        }
        else
        {
            // Fallback to old movement system
            var movement = Vector3.back * (_njs * Time.deltaTime);
            transform.position += movement;
        }
    }

    internal bool LimitPosition()
    {
        return transform.position.z < BombDeletePositionZ;
    }
}
