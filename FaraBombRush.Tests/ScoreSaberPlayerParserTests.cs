using FaraBombRush.Models;
using Xunit;

namespace FaraBombRush.Tests;

public class ScoreSaberPlayerParserTests
{
    // 2026-10 に https://scoresaber.com/api/player/{id}/basic が実際に返した形（"replaysWatched" と "silenced" がある）
    // 構造は実応答のまま、名前と ID は架空の値に置き換えてある
    private const string RealResponse =
        "{\"id\":\"76561190000000000\",\"name\":\"TestPlayer\",\"profilePicture\":\"https://cdn.scoresaber.com/avatars/76561190000000000.jpg\"," +
        "\"bio\":null,\"country\":\"US\",\"pp\":22007.999836146522,\"rank\":1,\"countryRank\":1,\"role\":null," +
        "\"badges\":[{\"description\":\"x\",\"image\":\"y\"}],\"histories\":\"999999\"," +
        "\"scoreStats\":{\"totalScore\":4204421586,\"totalRankedScore\":1727404524,\"averageRankedAccuracy\":95.9,\"totalPlayCount\":4279,\"rankedPlayCount\":1227,\"replaysWatched\":11999}," +
        "\"permissions\":0,\"banned\":false,\"silenced\":false,\"inactive\":false,\"firstSeen\":\"2018-07-14T15:48:46.000Z\"}";

    [Fact]
    public void Reads_pp_from_the_real_response()
    {
        Assert.True(ScoreSaberPlayerParser.TryParsePp(RealResponse, out var pp, out var error));
        Assert.Null(error);
        Assert.Equal(22008f, pp, 0);
    }

    [Fact]
    public void Ignores_unknown_and_missing_fields()
    {
        Assert.True(ScoreSaberPlayerParser.TryParsePp("{\"pp\":4321.5}", out var pp, out _));
        Assert.Equal(4321.5f, pp);
    }

    [Fact]
    public void Accepts_integer_pp_and_zero()
    {
        Assert.True(ScoreSaberPlayerParser.TryParsePp("{\"pp\":0}", out var zero, out _));
        Assert.Equal(0f, zero);
        Assert.True(ScoreSaberPlayerParser.TryParsePp("{\"pp\":9000}", out var pp, out _));
        Assert.Equal(9000f, pp);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not json")]
    [InlineData("{\"errorMessage\":\"Connection not found: 1\"}")]
    [InlineData("{\"pp\":null}")]
    [InlineData("{\"pp\":\"abc\"}")]
    [InlineData("{\"pp\":-1}")]
    [InlineData("{\"pp\":{}}")]
    [InlineData("[1,2,3]")]
    public void Reports_an_error_instead_of_returning_zero(string json)
    {
        var ok = ScoreSaberPlayerParser.TryParsePp(json, out var pp, out var error);
        Assert.False(ok);
        Assert.Equal(0f, pp);
        Assert.False(string.IsNullOrEmpty(error));
    }
}
