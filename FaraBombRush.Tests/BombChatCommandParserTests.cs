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

    [Theory]
    [InlineData("@user !bomb", BombChatCommandKind.Bomb, null)]
    [InlineData("@user !bomb 5", BombChatCommandKind.Bomb, 5)]
    [InlineData("@user !bombline 3", BombChatCommandKind.Line, 3)]
    [InlineData("@User_01 !BombReset", BombChatCommandKind.Reset, null)]
    [InlineData("!bomb 05", BombChatCommandKind.Bomb, 5)]
    [InlineData("!bomb 1", BombChatCommandKind.Bomb, 1)]
    [InlineData("!bomb 12", BombChatCommandKind.Bomb, 12)]
    [InlineData("  !bomb  7  ", BombChatCommandKind.Bomb, 7)]
    [InlineData("!bomb\t7", BombChatCommandKind.Bomb, 7)]
    [InlineData("!bomb　　7", BombChatCommandKind.Bomb, 7)]
    [InlineData("@user   !bomb    12", BombChatCommandKind.Bomb, 12)]
    public void Accepts_replies_extra_spaces_and_leading_zero(string chat, BombChatCommandKind kind, int? position)
    {
        Assert.True(BombChatCommandParser.TryParse(chat, out var command));
        Assert.Equal(kind, command.Kind);
        Assert.Equal(position, command.Position);
    }

    [Theory]
    [InlineData("!bomb​", null)] // zero width space
    [InlineData("!bomb ​", null)]
    [InlineData("​!bomb 5", 5)]
    [InlineData("!bo‍mb 5", 5)] // zero width joiner inside the command
    [InlineData("!bomb ​5", 5)]
    [InlineData("!bomb 5﻿", 5)] // zero width no-break space
    [InlineData("!bomb \U000E0000", null)] // tag character (7TV style), a surrogate pair
    [InlineData("!bomb 5 \U000E0000", 5)]
    [InlineData("@user ​!bomb 5 \U000E0000", 5)]
    [InlineData("!bomb\u0001 5", 5)] // control character
    public void Ignores_invisible_format_and_control_characters(string chat, int? position)
    {
        Assert.True(BombChatCommandParser.TryParse(chat, out var command));
        Assert.Equal(BombChatCommandKind.Bomb, command.Kind);
        Assert.Equal(position, command.Position);
    }

    [Theory]
    [InlineData("!bombs")]
    [InlineData("!bomberman")]
    [InlineData("@user !bombs")]
    [InlineData("@user !bomb 0")]
    [InlineData("@user !bomb 13")]
    [InlineData("@user !bomb abc")]
    [InlineData("!bomb abc")]
    [InlineData("@user")]
    [InlineData("@")]
    [InlineData("@ !bomb")]
    [InlineData("@user @other !bomb")] // only one leading mention is skipped
    [InlineData("@user hello !bomb")]
    [InlineData("!bomb @user")]
    [InlineData("!bomb 00")]
    [InlineData("!bomb 005")]
    [InlineData("!bomb ５")] // full-width digit: rejected on purpose (the "!" must be ASCII too)
    [InlineData("！bomb")] // full-width "!"
    [InlineData("!bomb 5​ 3")]
    [InlineData("​​")]
    public void Rejects_what_the_spec_does_not_accept(string chat)
    {
        Assert.False(BombChatCommandParser.TryParse(chat, out _));
    }

    [Fact]
    public void Plain_bomb_without_a_number_means_random_position()
    {
        Assert.True(BombChatCommandParser.TryParse("!bomb", out var command));
        Assert.Null(command.Position);
    }
}
