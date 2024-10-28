// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Extensions.Diagnostics;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.Diagnostics;

public class ProfilerTest
{
    [Fact]
    public void ProfileTimeWarningStopwatch_Action_LogsElapsedTime()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        Action action = () =>
        {
            /* Simulate work */
        };

        // Act
        var elapsed = action.ProfileTimeWarningStopwatch(logger);

        // Assert
        elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Started")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Ended")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public void ProfileTimeWarningStopwatch_Func_LogsElapsedTime()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        Func<int> func = () => 42;

        // Act
        var (result, elapsed) = func.ProfileTimeWarningStopwatch(logger);

        // Assert
        result.Should().Be(42);
        elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Started")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Ended")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task ProfileTimeWarningAsync_Action_LogsElapsedTime()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        Func<Task> action = async () => await Task.Delay(10);

        // Act
        var elapsed = await action.ProfileTimeWarningAsync(logger);

        // Assert
        elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Started")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Ended")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task ProfileTimeWarningAsync_Func_LogsElapsedTime()
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        Func<Task<int>> func = async () =>
        {
            await Task.Delay(10);
            return 42;
        };

        // Act
        var (result, elapsed) = await func.ProfileTimeWarningAsync(logger);

        // Assert
        result.Should().Be(42);
        elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Started")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
        logger.Received().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Ended")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}