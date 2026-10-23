using FaraBombRush.Models;
using Xunit;

namespace FaraBombRush.Tests;

public class BombChatCommandParserTests
{
    [Theory]
    [InlineData("!bomb", BombChatCommandKind.Bomb, null)]
    [InlineData("!bombline", BombChatCommandKind.Line, null)]
    [InlineData("!BombLine 5", BombChatCommandKind.Line, 5)]
    [InlineData("!bombreset 12", BombChatCommandKind.Reset, 12)]
    public void Accepts_valid_commands(string chat, BombChatCommandKind kind, int? position)
    {
        Assert.True(BombChatCommandParser.TryParse(chat, out var command));
        Assert.Equal(kind, command.Kind);
        Assert.Equal(position, command.Position);
    }

    [Theory]
    [InlineData("")]
    [InlineData("!bombs")]
    [InlineData("hello !bomb")]
    [InlineData("!bomb 0")]
    [InlineData("!bomb 13")]
    [InlineData("!bomb +5")]
    [InlineData("!bomb 1 2")]
    public void Rejects_invalid_commands(string chat)
    {
        Assert.False(BombChatCommandParser.TryParse(chat, out _));
    }
}
