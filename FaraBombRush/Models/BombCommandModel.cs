namespace FaraBombRush.Models;

public class BombCommandModel
{
    public int BombId { get; set; }

    public int PositionIndex { get; set; }

    public float SpawnDelayTime { get; set; }
}