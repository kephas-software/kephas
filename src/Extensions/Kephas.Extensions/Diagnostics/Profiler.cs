// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Profiler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides operations for profiling the code.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Kephas.Extensions.Diagnostics;

/// <summary>
/// Provides operations for profiling the code.
/// </summary>
public static class Profiler
{
    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Warning"/> level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan ProfileTimeWarningStopwatch(
        this Action action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTime(action, logger, LogLevel.Warning, memberName);

    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Warning"/> level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (T result, TimeSpan elapsed) ProfileTimeWarningStopwatch<T>(
        this Func<T> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTime(action, logger, LogLevel.Warning, memberName);

    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Information"/> level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan ProfileTimeInfoStopwatch(
        this Action action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTime(action, logger, LogLevel.Information, memberName);

    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Information"/> level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (T result, TimeSpan elapsed) ProfileTimeInfoStopwatch<T>(
        this Func<T> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTime(action, logger, LogLevel.Information, memberName);

    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan ProfileTimeDebugStopwatch(
        this Action action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTime(action, logger, LogLevel.Debug, memberName);

    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (T result, TimeSpan elapsed) ProfileTimeDebugStopwatch<T>(
        this Func<T> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTime(action, logger, LogLevel.Debug, memberName);

    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TimeSpan ProfileTimeTraceStopwatch(
        this Action action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTime(action, logger, LogLevel.Trace, memberName);

    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (T result, TimeSpan elapsed) ProfileTimeTraceStopwatch<T>(
        this Func<T> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTime(action, logger, LogLevel.Trace, memberName);

    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at the indicated log level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    public static TimeSpan ProfileTime(
        this Action action,
        ILogger? logger = null,
        LogLevel logLevel = LogLevel.Debug,
        [CallerMemberName] string? memberName = null)
    {
        ArgumentNullException.ThrowIfNull(action);

        logger?.Log(logLevel, "Started: {Operation}.", memberName);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        action();
        stopwatch.Stop();

        logger?.Log(logLevel, "Ended: {Operation}. Elapsed: {Elapsed:c}.", memberName, stopwatch.Elapsed);

        return stopwatch.Elapsed;
    }

    /// <summary>
    /// Executes the action with a stopwatch, optionally logging the elapsed time at the indicated
    /// log level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">Optional. The logger.</param>
    /// <param name="logLevel">Optional. The log level.</param>
    /// <param name="memberName">Optional. Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    public static (T result, TimeSpan elapsed) ProfileTime<T>(
        this Func<T> action,
        ILogger? logger = null,
        LogLevel logLevel = LogLevel.Debug,
        [CallerMemberName] string? memberName = null)
    {
        ArgumentNullException.ThrowIfNull(action);

        logger?.Log(logLevel, "Started: {Operation}.", memberName);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = action();
        stopwatch.Stop();

        logger?.Log(logLevel, "Ended: {Operation}. Elapsed: {Elapsed:c}.", memberName, stopwatch.Elapsed);

        return (result, stopwatch.Elapsed);
    }

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Warning"/> level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TimeSpan> ProfileTimeWarningAsync(
        this Func<Task> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTimeAsync(action, logger, LogLevel.Warning, memberName);

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Warning"/> level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<(T result, TimeSpan elapsed)> ProfileTimeWarningAsync<T>(
        this Func<Task<T>> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTimeAsync(action, logger, LogLevel.Warning, memberName);

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Information"/> level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TimeSpan> ProfileTimeInfoAsync(
        this Func<Task> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTimeAsync(action, logger, LogLevel.Information, memberName);

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Information"/> level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<(T result, TimeSpan elapsed)> ProfileTimeInfoAsync<T>(
        this Func<Task<T>> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTimeAsync(action, logger, LogLevel.Information, memberName);

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TimeSpan> ProfileTimeDebugAsync(
        this Func<Task> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTimeAsync(action, logger, LogLevel.Debug, memberName);

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Debug"/> level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<(T result, TimeSpan elapsed)> ProfileTimeDebugAsync<T>(
        this Func<Task<T>> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTimeAsync(action, logger, LogLevel.Debug, memberName);

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<TimeSpan> ProfileTimeTraceAsync(
        this Func<Task> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTimeAsync(action, logger, LogLevel.Trace, memberName);

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Trace"/> level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<(T result, TimeSpan elapsed)> ProfileTimeTraceAsync<T>(
        this Func<Task<T>> action,
        ILogger? logger = null,
        [CallerMemberName] string? memberName = null) =>
        ProfileTimeAsync<T>(action, logger, LogLevel.Trace, memberName);

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at the indicated log level.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="logLevel">The log level.</param>
    /// <param name="memberName">Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    public static async Task<TimeSpan> ProfileTimeAsync(
        this Func<Task> action,
        ILogger? logger = null,
        LogLevel logLevel = LogLevel.Debug,
        [CallerMemberName] string? memberName = null)
    {
        ArgumentNullException.ThrowIfNull(action);

        logger?.Log(logLevel, "Started: {Operation}.", memberName);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await action();
        stopwatch.Stop();

        logger?.Log(logLevel, "Ended: {Operation}. Elapsed: {Elapsed:c}.", memberName, stopwatch.Elapsed);

        return stopwatch.Elapsed;
    }

    /// <summary>
    /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed
    /// time at the indicated log level.
    /// </summary>
    /// <typeparam name="T">The operation return type.</typeparam>
    /// <param name="action">The action.</param>
    /// <param name="logger">Optional. The logger.</param>
    /// <param name="logLevel">Optional. The log level.</param>
    /// <param name="memberName">Optional. Name of the member.</param>
    /// <returns>
    /// The elapsed time.
    /// </returns>
    public static async Task<(T result, TimeSpan elapsed)> ProfileTimeAsync<T>(
        this Func<Task<T>> action,
        ILogger? logger = null,
        LogLevel logLevel = LogLevel.Debug,
        [CallerMemberName] string? memberName = null)
    {
        ArgumentNullException.ThrowIfNull(action);

        logger?.Log(logLevel, "Started: {Operation}.", memberName);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = await action();
        stopwatch.Stop();

        logger?.Log(logLevel, "Ended: {Operation}. Elapsed: {Elapsed:c}.", memberName, stopwatch.Elapsed);

        return (result, stopwatch.Elapsed);
    }
}