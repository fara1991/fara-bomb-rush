using System.Text.Json.Serialization;

namespace FaraBombRush.Models;

public class BombCommandModel
{
    [JsonPropertyName("BombId")] public int BombId { get; set; }

    [JsonPropertyName("PositionIndex")] public int PositionIndex { get; set; }

    [JsonPropertyName("SpawnDelayTime")] public float SpawnDelayTime { get; set; }
}