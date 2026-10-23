using FaraBombRush.Enums;
using Xunit;

namespace FaraBombRush.Tests;

public class GameModeEnumHelperTests
{
    [Theory]
    [InlineData("None", "None")]
    [InlineData("none", "None")]
    [InlineData(" None ", "None")]
    [InlineData("Interactive", "Interactive")]
    [InlineData("auto", "Auto")]
    [InlineData("Battle", "Battle")]
    [InlineData("", "Interactive")]
    [InlineData(null, "Interactive")]
    [InlineData("Nothing", "Interactive")]
    public void Normalize_keeps_None_and_known_names(string saved, string expected)
    {
        Assert.Equal(expected, GameModeEnumHelper.Normalize(saved));
    }

    [Fact]
    public void New_users_get_Interactive_by_default()
    {
        // PluginConfig.GameMode の既定値と同じ式
        Assert.Equal("Interactive", GameModeEnum.Interactive.ToString());
        Assert.True(GameModeEnumHelper.RunsMod(GameModeEnum.Interactive.ToString()));
    }

    [Theory]
    [InlineData("Interactive", true)]
    [InlineData("Auto", true)]
    [InlineData("auto", true)]
    [InlineData("None", false)]
    [InlineData("none", false)]
    [InlineData("Battle", false)]
    [InlineData("", true)]
    [InlineData(null, true)]
    public void RunsMod_is_false_for_None_and_Battle(string saved, bool expected)
    {
        Assert.Equal(expected, GameModeEnumHelper.RunsMod(saved));
    }

    [Theory]
    [InlineData("None", "None")]
    [InlineData("Interactive", "Interactive")]
    [InlineData("Auto", "Auto")]
    [InlineData("Battle", "None")]
    [InlineData("battle", "None")]
    public void ToSelectable_shows_Battle_as_None(string saved, string expected)
    {
        Assert.Equal(expected, GameModeEnumHelper.ToSelectable(saved));
    }

    [Fact]
    public void Selectable_names_include_None_and_exclude_Battle()
    {
        Assert.Equal(new[] { "None", "Interactive", "Auto" }, GameModeEnumHelper.SelectableNames);
    }
}
