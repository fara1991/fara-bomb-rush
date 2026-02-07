namespace FaraBombRush.Models;

internal class BombCommandModel
{
    internal int BombId { get; set; }

    internal int PositionIndex { get; set; }

    internal float HitTime { get; set; } // Time when bomb should reach the player
}