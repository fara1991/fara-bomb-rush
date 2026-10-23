namespace FaraBombRush.Models;

/// <summary>
/// !bombline（と Auto モードのライン）で、並ぶボム同士の間隔を決める純粋関数。Unity や Beat Saber に依存しない。
/// 間隔は距離（メートル）で決め、時間（HitTime の差）に直して使う。
/// 1.0.0 では「1 つ目から 1.0 だけ Z 方向にずらす」実装で、間隔は 1 m（ボムがほぼ接する近さ）だった。
/// その後 HitTime（曲の時間）基準に変えたときに、この 1.0 が「1 拍」の意味になり、間隔が NJS と BPM に比例して広がった。
/// </summary>
public static class BombLineTiming
{
    /// <summary>ライン内の隣り合うボムの間隔（メートル）。1.0.0 と同じ 1 m。</summary>
    public const float SpacingMeters = 1.0f;

    private const float MinNjs = 0.1f;
    private const float MinTimeScale = 0.001f;

    /// <summary>
    /// 隣り合うボムの HitTime の差（曲の時間の秒）。FaraBombMoveController は Z = 残り時間 × NJS ÷ timeScale で動かすので、
    /// Z の間隔を spacingMeters にするには、時間の差を spacingMeters × timeScale ÷ NJS にする。
    /// </summary>
    public static float SpacingSeconds(float njs, float timeScale, float spacingMeters = SpacingMeters)
    {
        var safeNjs = njs > MinNjs ? njs : MinNjs;
        var safeTimeScale = timeScale > MinTimeScale ? timeScale : 1f;
        return spacingMeters * safeTimeScale / safeNjs;
    }
}
