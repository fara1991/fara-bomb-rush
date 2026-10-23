using FaraBombRush.Enums;
using FaraBombRush.Models;
using Xunit;

namespace FaraBombRush.Tests;

public class BombLevelCalculatorTests
{
    // 閾値は 2000 / 4000 / 6000 / 8000 / 10000 / 12000（その値ちょうどは上のレベル）
    [Theory]
    [InlineData(0f, FaraBombLevelEnum.Beginner)]
    [InlineData(1999.9f, FaraBombLevelEnum.Beginner)]
    [InlineData(2000f, FaraBombLevelEnum.Easy)]
    [InlineData(3999.9f, FaraBombLevelEnum.Easy)]
    [InlineData(4000f, FaraBombLevelEnum.Normal)]
    [InlineData(5999f, FaraBombLevelEnum.Normal)]
    [InlineData(6000f, FaraBombLevelEnum.Hard)]
    [InlineData(7999.9f, FaraBombLevelEnum.Hard)]
    [InlineData(8000f, FaraBombLevelEnum.Expert)]
    [InlineData(9999.9f, FaraBombLevelEnum.Expert)]
    [InlineData(10000f, FaraBombLevelEnum.ExpertPlus)]
    [InlineData(11999f, FaraBombLevelEnum.ExpertPlus)]
    [InlineData(12000f, FaraBombLevelEnum.Lawless)]
    [InlineData(22008f, FaraBombLevelEnum.Lawless)]
    public void Level_follows_pp(float pp, FaraBombLevelEnum expected)
    {
        Assert.Equal(expected, BombLevelCalculator.FromPp(pp));
    }
}
