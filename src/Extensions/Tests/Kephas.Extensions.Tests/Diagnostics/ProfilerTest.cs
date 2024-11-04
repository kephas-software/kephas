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
    [Theory]
    [MemberData(nameof(GetActionProfileTime_LogsElapsedTime_Sync_Data))]
    public void ProfileTime_Action_LogsElapsedTime(
        Func<Action, ILogger?, string, TimeSpan> profile,
        LogLevel expectedLogLevel)
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        Action action = () =>
        {
            /* Simulate work */
        };

        // Act
        var elapsed = profile(action, logger, "method");

        // Assert
        elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        logger.Received().Log(
            expectedLogLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Started")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
        logger.Received().Log(
            expectedLogLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Ended")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Theory]
    [MemberData(nameof(GetFuncProfileTime_LogsElapsedTime_Sync_Data))]
    public void ProfileTime_Func_LogsElapsedTime(
        Func<Func<int>, ILogger?, string, (int result, TimeSpan elapsed)> profile,
        LogLevel expectedLogLevel)
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        Func<int> func = () => 42;

        // Act
        var (result, elapsed) = profile(func, logger, "method");

        // Assert
        result.Should().Be(42);
        elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        logger.Received().Log(
            expectedLogLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Started")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
        logger.Received().Log(
            expectedLogLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Ended")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Theory]
    [MemberData(nameof(GetActionProfileTime_LogsElapsedTime_Data))]
    public async Task ProfileTimeAsync_Action_LogsElapsedTime(
        Func<Func<Task>, ILogger?, string, Task<TimeSpan>> profile,
        LogLevel expectedLogLevel)
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        Func<Task> action = async () => await Task.Delay(10);

        // Act
        var elapsed = await profile(action, logger, "method");

        // Assert
        elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        logger.Received().Log(
            expectedLogLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Started")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
        logger.Received().Log(
            expectedLogLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Ended")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Theory]
    [MemberData(nameof(GetFuncProfileTime_LogsElapsedTime_Data))]
    public async Task ProfileTimeAsync_Func_LogsElapsedTime(
        Func<Func<Task<int>>, ILogger?, string, Task<(int result, TimeSpan elapsed)>> profile,
        LogLevel expectedLogLevel)
    {
        // Arrange
        var logger = Substitute.For<ILogger>();
        Func<Task<int>> func = async () =>
        {
            await Task.Delay(10);
            return 42;
        };

        // Act
        var (result, elapsed) = await profile(func, logger, "method");

        // Assert
        result.Should().Be(42);
        elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        logger.Received().Log(
            expectedLogLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Started")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
        logger.Received().Log(
            expectedLogLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString().Contains("Ended")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    public static TheoryData<Func<Func<Task<int>>, ILogger?, string, Task<(int result, TimeSpan elapsed)>>, LogLevel> GetFuncProfileTime_LogsElapsedTime_Data() =>
        new()
        {
            { Profiler.ProfileTimeWarningAsync, LogLevel.Warning },
            { Profiler.ProfileTimeInfoAsync, LogLevel.Information },
            { Profiler.ProfileTimeDebugAsync, LogLevel.Debug },
            { Profiler.ProfileTimeTraceAsync, LogLevel.Trace },
        };

    public static TheoryData<Func<Func<Task>, ILogger?, string, Task<TimeSpan>>, LogLevel> GetActionProfileTime_LogsElapsedTime_Data() =>
        new()
        {
            { Profiler.ProfileTimeWarningAsync, LogLevel.Warning },
            { Profiler.ProfileTimeInfoAsync, LogLevel.Information },
            { Profiler.ProfileTimeDebugAsync, LogLevel.Debug },
            { Profiler.ProfileTimeTraceAsync, LogLevel.Trace },
        };

    public static TheoryData<Func<Action, ILogger?, string, TimeSpan>, LogLevel> GetActionProfileTime_LogsElapsedTime_Sync_Data() =>
        new()
        {
            { Profiler.ProfileTimeWarning, LogLevel.Warning },
            { Profiler.ProfileTimeInfo, LogLevel.Information },
            { Profiler.ProfileTimeDebug, LogLevel.Debug },
            { Profiler.ProfileTimeTrace, LogLevel.Trace },
        };

    public static TheoryData<Func<Func<int>, ILogger?, string, (int result, TimeSpan elapsed)>, LogLevel> GetFuncProfileTime_LogsElapsedTime_Sync_Data() =>
        new()
        {
            { Profiler.ProfileTimeWarning, LogLevel.Warning },
            { Profiler.ProfileTimeInfo, LogLevel.Information },
            { Profiler.ProfileTimeDebug, LogLevel.Debug },
            { Profiler.ProfileTimeTrace, LogLevel.Trace },
        };
}