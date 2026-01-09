using UnityEngine;

namespace FaraBombRush.Controllers.Components;

internal class FaraBombMoveController : FaraBombComponentBaseController
{
    private const float DefaultNoteJumpSpeed = 20.0f;
    private const float BombDeletePositionZ = -3f;

    private void Update()
    {
        Move();
    }

    protected override void InitializeComponent()
    {
    }

    private void Move()
    {
        var movement = Vector3.back * (DefaultNoteJumpSpeed * Time.deltaTime);

        // ルートオブジェクトの移動のみを行う
        transform.position += movement;
    }

    internal bool LimitPosition()
    {
        return transform.position.z < BombDeletePositionZ;
    }
}