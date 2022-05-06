using System;
using System.Threading.Tasks;
using AlertsApi.Domain.Entities;
using AlertsApi.Domain.Repositories;
using AlertsApi.TgAlerts.Worker.Models;
using AlertsApi.TgAlerts.Worker.Services;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace AlertsApi.TgAlerts.Worker.UnitTests
{
    public class AlertsServiceTests
    {
        private readonly IAlertRepository _alertRepository = Substitute.For<IAlertRepository>();
        private readonly ILogger<AlertsService> _logger = Substitute.For<ILogger<AlertsService>>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly AlertsService _sut;

        public AlertsServiceTests()
        {
            _sut = new AlertsService(_alertRepository, _mapper, _logger);
        }

        [Fact]
        public async Task UpdateAlertsAsync_WhenReceivingActivateAlertFromPast_ShouldOnlySetStartTime()
        {
            // Arrange
            var alert = new Alert()
            {
                Active = false,
                EndTime = new DateTime(2022, 01, 02),
                StartTime = null
            };

            var telegramAlert = new TgAlert()
            {
                Active = true,
                FetchedAt = new DateTime(2022, 01, 01)
            };

            _alertRepository.GetAlertByHashTagAsync(Arg.Any<string>()).Returns(alert);

            // Act
            await _sut.UpdateAlertsAsync(new[] { telegramAlert });

            // Assert
            alert.Active.Should().BeFalse();
            alert.StartTime.Should().Be(telegramAlert.FetchedAt);
            alert.EndTime.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAlertsAsync_WhenReceivingNewAlertActivation_ShouldUpdateStartTimeAndSetEndTimeWithNull()
        {
            // Arrange
            var alert = new Alert()
            {
                Active = true,
                EndTime = null,
                StartTime = new DateTime(2022, 01, 02)
            };

            var telegramAlert = new TgAlert()
            {
                Active = true,
                FetchedAt = new DateTime(2022, 01, 03)
            };

            _alertRepository.GetAlertByHashTagAsync(Arg.Any<string>()).Returns(alert);

            // Act
            await _sut.UpdateAlertsAsync(new[] { telegramAlert });

            // Assert
            alert.Active.Should().BeTrue();
            alert.StartTime.Should().Be(telegramAlert.FetchedAt);
            alert.EndTime.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAlertsAsync_WhenReceivingActivateAlertFromPastAndAlertFullFilled_ShouldDontDoAnything()
        {
            // Arrange
            var alert = new Alert()
            {
                Active = false,
                EndTime = new DateTime(2022, 01, 03),
                StartTime = new DateTime(2022, 01, 02)
            };

            var telegramAlert = new TgAlert()
            {
                Active = true,
                FetchedAt = new DateTime(2022, 01, 01)
            };

            _alertRepository.GetAlertByHashTagAsync(Arg.Any<string>()).Returns(alert);

            // Act
            await _sut.UpdateAlertsAsync(new[] { telegramAlert });

            // Assert
            alert.Active.Should().BeFalse();
            alert.StartTime.Should().NotBe(telegramAlert.FetchedAt);
            alert.EndTime.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAlertsAsync_WhenReceivingNewActivateAlert_ShouldActivateAlertSetStartTimeAndResetEndTime()
        {
            // Arrange
            var alert = new Alert()
            {
                Active = false,
                EndTime = new DateTime(2022, 01, 03),
                StartTime = new DateTime(2022, 01, 02)
            };

            var telegramAlert = new TgAlert()
            {
                Active = true,
                FetchedAt = new DateTime(2022, 01, 04)
            };

            _alertRepository.GetAlertByHashTagAsync(Arg.Any<string>()).Returns(alert);

            // Act
            await _sut.UpdateAlertsAsync(new[] { telegramAlert });

            // Assert
            alert.Active.Should().BeTrue();
            alert.StartTime.Should().Be(telegramAlert.FetchedAt);
            alert.EndTime.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAlertsAsync_WhenReceivingNewDeactivateAlert_ShouldSetEndTimeAndDeactivate()
        {
            // Arrange
            var alert = new Alert()
            {
                Active = true,
                EndTime = null,
                StartTime = new DateTime(2022, 01, 01)
            };

            var telegramAlert = new TgAlert()
            {
                Active = false,
                FetchedAt = new DateTime(2022, 01, 02)
            };

            _alertRepository.GetAlertByHashTagAsync(Arg.Any<string>()).Returns(alert);

            // Act
            await _sut.UpdateAlertsAsync(new[] { telegramAlert });

            // Assert
            alert.Active.Should().BeFalse();
            alert.EndTime.Should().Be(telegramAlert.FetchedAt);
            alert.StartTime.Should().NotBeNull();
            alert.StartTime.Should().NotBe(telegramAlert.FetchedAt);
        }

        [Fact]
        public async Task UpdateAlertsAsync_WhenReceivingDeactivateAlertFromPast_ShouldDontDoAnything()
        {
            // Arrange
            var alert = new Alert()
            {
                Active = true,
                EndTime = null,
                StartTime = new DateTime(2022, 01, 02)
            };

            var telegramAlert = new TgAlert()
            {
                Active = false,
                FetchedAt = new DateTime(2022, 01, 01)
            };

            _alertRepository.GetAlertByHashTagAsync(Arg.Any<string>()).Returns(alert);

            // Act
            await _sut.UpdateAlertsAsync(new[] { telegramAlert });

            // Assert
            alert.Active.Should().BeTrue();
            alert.EndTime.Should().BeNull();
            alert.StartTime.Should().NotBeNull();
            alert.StartTime.Should().NotBe(telegramAlert.FetchedAt);
        }

        [Fact]
        public async Task UpdateAlertsAsync_WhenAlertActivationIsNew_ShouldSetStartTimeAndActivate()
        {
            // Arrange
            var telegramAlert = new TgAlert()
            {
                Active = true,
                FetchedAt = new DateTime(2022, 01, 01)
            };

            var alert = new Alert()
            {
                Active = telegramAlert.Active
            };

            _alertRepository.GetAlertByHashTagAsync(Arg.Any<string>()).ReturnsNull();
            _mapper.Map<TgAlert, Alert>(Arg.Any<TgAlert>()).Returns(alert);

            // Act
            await _sut.UpdateAlertsAsync(new[] { telegramAlert });

            // Assert
            alert.Active.Should().BeTrue();
            alert.StartTime.Should().Be(telegramAlert.FetchedAt);
            alert.EndTime.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAlertsAsync_WhenAlertDeactivationIsNew_ShouldSetEndTimeAndDeactivate()
        {
            // Arrange
            var telegramAlert = new TgAlert()
            {
                Active = false,
                FetchedAt = new DateTime(2022, 01, 01)
            };

            var alert = new Alert()
            {
                Active = telegramAlert.Active
            };

            _alertRepository.GetAlertByHashTagAsync(Arg.Any<string>()).ReturnsNull();
            _mapper.Map<TgAlert, Alert>(Arg.Any<TgAlert>()).Returns(alert);

            // Act
            await _sut.UpdateAlertsAsync(new[] { telegramAlert });

            // Assert
            alert.Active.Should().BeFalse();
            alert.StartTime.Should().BeNull();
            alert.EndTime.Should().Be(telegramAlert.FetchedAt);
        }
    }
}