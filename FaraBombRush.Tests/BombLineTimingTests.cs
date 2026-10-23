using FaraBombRush.Models;
using Xunit;

namespace FaraBombRush.Tests;

public class BombLineTimingTests
{
    [Theory]
    [InlineData(12f, 1f, 1f / 12f)]
    [InlineData(18f, 1f, 1f / 18f)]
    [InlineData(20f, 1f, 0.05f)]
    public void Spacing_is_one_meter_divided_by_njs(float njs, float timeScale, float expected)
    {
        Assert.Equal(expected, BombLineTiming.SpacingSeconds(njs, timeScale), 5);
    }

    [Theory]
    [InlineData(12f, 120f)]
    [InlineData(18f, 180f)]
    [InlineData(10f, 90f)]
    public void Spacing_is_much_shorter_than_the_old_one_beat(float njs, float bpm)
    {
        var oldSeconds = 60f / bpm; // 変更前: BombLineDiffBeat(1.0) 拍
        Assert.True(BombLineTiming.SpacingSeconds(njs, 1f) < oldSeconds / 4f);
    }

    [Fact]
    public void Spacing_in_z_stays_one_meter_for_any_njs_and_time_scale()
    {
        // FaraBombMoveController: z = (hitTime - songTime) * njs / timeScale
        foreach (var njs in new[] { 8f, 12f, 16f, 22f })
        foreach (var timeScale in new[] { 0.5f, 1f, 1.5f })
        {
            var dt = BombLineTiming.SpacingSeconds(njs, timeScale);
            var dz = dt * njs / timeScale;
            Assert.Equal(BombLineTiming.SpacingMeters, dz, 4);
        }
    }

    [Fact]
    public void Invalid_inputs_do_not_throw_or_produce_nan()
    {
        Assert.False(float.IsNaN(BombLineTiming.SpacingSeconds(0f, 0f)));
        Assert.False(float.IsInfinity(BombLineTiming.SpacingSeconds(-5f, -1f)));
    }
}
