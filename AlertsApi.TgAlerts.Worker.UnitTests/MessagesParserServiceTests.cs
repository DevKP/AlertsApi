using System;
using System.Linq;
using AlertsApi.TgAlerts.Worker.Services;
using AlertsApi.TgAlerts.Worker.Models;
using FluentAssertions;
using TL;
using Xunit;

namespace AlertsApi.TgAlerts.Worker.UnitTests;

public class MessagesParserServiceTests
{
    private readonly MessagesParserService _sut;

    public MessagesParserServiceTests()
    {
        _sut = new MessagesParserService();
    }

    [Fact]
    public void ParseMessages_WhenStartMessageTextInAValidFormat_ShouldReturnTgAlert()
    {
        // Arrange
        var message = new Message()
        {
            date = new DateTime(2022, 01, 01),
            message = "🔴 07:47 Повітряна тривога в м. Слов'янськ та Слов'янська територіальна громада\nСлідкуйте за подальшими повідомленнями.\n#м_Словянськ_та_Словянська_територіальна_громада"
        };

        // Act
        var result = _sut.ParseMessages(new[] { message }).First();

        // Assert
        result.Active.Should().BeTrue();
        result.FetchedAt.Should().Be(message.date);
        result.LocationHashTag.Should().Be("м_Словянськ_та_Словянська_територіальна_громада");
        result.LocationTitle.Should().Be("м. Слов'янськ та Слов'янська територіальна громада");
        result.OriginalMessage.Should().Be(message.message);
    }

    [Fact]
    public void ParseMessages_WhenStopMessageTextInAValidFormat_ShouldReturnTgAlert()
    {
        // Arrange
        var message = new Message()
        {
            date = new DateTime(2022, 01, 01),
            message = "🟢 08:36 Відбій тривоги в м. Слов'янськ та Слов'янська територіальна громада.\nСлідкуйте за подальшими повідомленнями.\n#м_Словянськ_та_Словянська_територіальна_громада"
        };

        // Act
        var result = _sut.ParseMessages(new[] { message }).First();

        // Assert
        result.Active.Should().BeFalse();
        result.FetchedAt.Should().Be(message.date);
        result.LocationHashTag.Should().Be("м_Словянськ_та_Словянська_територіальна_громада");
        result.LocationTitle.Should().Be("м. Слов'янськ та Слов'янська територіальна громада");
        result.OriginalMessage.Should().Be(message.message);
    }

    [Fact]
    public void ParseMessages_WhenMessageTextMissingHashTag_ShouldSkipIt()
    {
        // Arrange
        var message = new Message()
        {
            date = new DateTime(2022, 01, 01),
            message = "🟢 08:36 Відбій тривоги в м. Слов'янськ та Слов'янська територіальна громада.\nСлідкуйте за подальшими повідомленнями."
        };

        // Act
        var result = _sut.ParseMessages(new[] { message });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ParseMessages_WhenMessageTextHaveUnknownLocationFormat_ShouldAssignEmptyString()
    {
        // Arrange
        var message = new Message()
        {
            date = new DateTime(2022, 01, 01),
            message = "🟢 08:36 Відбій тривоги у локації м. Слов'янськ та Слов'янська територіальна громада.\nСлідкуйте за подальшими повідомленнями.\n#м_Словянськ_та_Словянська_територіальна_громада"
        };

        // Act
        var result = _sut.ParseMessages(new[] { message }).First();

        // Assert
        result.Active.Should().BeFalse();
        result.FetchedAt.Should().Be(message.date);
        result.LocationHashTag.Should().Be("м_Словянськ_та_Словянська_територіальна_громада");
        result.LocationTitle.Should().BeEmpty();
        result.OriginalMessage.Should().Be(message.message);
    }

    [Fact]
    public void ParseMessages_WhenMessageTextDontContainStateEmoji_ShouldReturnActiveAlert()
    {
        // Arrange
        var message = new Message()
        {
            date = new DateTime(2022, 01, 01),
            message = "08:36 Відбій тривоги в м. Слов'янськ та Слов'янська територіальна громада.\nСлідкуйте за подальшими повідомленнями.\n#м_Словянськ_та_Словянська_територіальна_громада"
        };

        // Act
        var result = _sut.ParseMessages(new[] { message }).First();

        // Assert
        result.Active.Should().BeTrue();
        result.FetchedAt.Should().Be(message.date);
        result.LocationHashTag.Should().Be("м_Словянськ_та_Словянська_територіальна_громада");
        result.LocationTitle.Should().Be("м. Слов'янськ та Слов'янська територіальна громада");
        result.OriginalMessage.Should().Be(message.message);
    }
}