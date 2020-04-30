// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Profiler.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Provides operations for profiling the code.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Threading.Tasks;

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
        public static IOperationResult WithWarningStopwatch(
            this Action action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatch(action, logger, LogLevel.Warning, memberName);
        }

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
        public static IOperationResult<T> WithWarningStopwatch<T>(
            this Func<T> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatch(action, logger, LogLevel.Warning, memberName);
        }

        /// <summary>
        /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// The elapsed time.
        /// </returns>
        public static IOperationResult WithInfoStopwatch(
            this Action action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatch(action, logger, LogLevel.Info, memberName);
        }

        /// <summary>
        /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <typeparam name="T">The operation return type.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// The elapsed time.
        /// </returns>
        public static IOperationResult<T> WithInfoStopwatch<T>(
            this Func<T> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatch(action, logger, LogLevel.Info, memberName);
        }

        /// <summary>
        /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Debug"/> level.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// The elapsed time.
        /// </returns>
        public static IOperationResult WithDebugStopwatch(
            this Action action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatch(action, logger, LogLevel.Debug, memberName);
        }

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
        public static IOperationResult<T> WithDebugStopwatch<T>(
            this Func<T> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatch(action, logger, LogLevel.Debug, memberName);
        }

        /// <summary>
        /// Executes the action with a stopwatch, optionally logging the elapsed time at <see cref="LogLevel.Trace"/> level.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// The elapsed time.
        /// </returns>
        public static IOperationResult WithTraceStopwatch(
            this Action action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatch(action, logger, LogLevel.Trace, memberName);
        }

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
        public static IOperationResult<T> WithTraceStopwatch<T>(
            this Func<T> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatch(action, logger, LogLevel.Trace, memberName);
        }

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
        public static IOperationResult WithStopwatch(
            this Action action,
            ILogger? logger = null,
            LogLevel logLevel = LogLevel.Debug,
            [CallerMemberName] string? memberName = null)
        {
            var result = new OperationResult();
            if (action == null)
            {
                return result.MergeMessage($"No action provided for {memberName}.");
            }

            result.MergeMessage($"{memberName}. Started at: {DateTime.Now:s}.");
            logger?.Log(logLevel, "{operation}. Started at: {startedAt:s}.", memberName, DateTime.Now);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();

            result
                .MergeMessage($"{memberName}. Ended at: {DateTime.Now:s}. Elapsed: {stopwatch.Elapsed:c}.")
                .Complete(stopwatch.Elapsed);
            logger?.Log(logLevel, "{operation}. Ended at: {endedAt:s}. Elapsed: {elapsed:c}.", memberName, DateTime.Now, stopwatch.Elapsed);

            return result;
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
        public static IOperationResult<T> WithStopwatch<T>(
            this Func<T> action,
            ILogger? logger = null,
            LogLevel logLevel = LogLevel.Debug,
            [CallerMemberName] string? memberName = null)
        {
            var result = new OperationResult<T>();
            if (action == null)
            {
                return result.MergeMessage($"No action provided for {memberName}.");
            }

            result.MergeMessage($"{memberName}. Started at: {DateTime.Now:s}.");
            logger?.Log(logLevel, "{operation}. Started at: {startedAt:s}.", memberName, DateTime.Now);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            result.Value = action();
            stopwatch.Stop();

            result
                .MergeMessage($"{memberName}. Ended at: {DateTime.Now:s}. Elapsed: {stopwatch.Elapsed:c}.")
                .Complete(stopwatch.Elapsed);
            logger?.Log(logLevel, "{operation}. Ended at: {endedAt:s}. Elapsed: {elapsed:c}.", memberName, DateTime.Now, stopwatch.Elapsed);

            return result;
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
        public static Task<IOperationResult> WithWarningStopwatchAsync(
            this Func<Task> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatchAsync(action, logger, LogLevel.Warning, memberName);
        }

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
        public static Task<IOperationResult<T>> WithWarningStopwatchAsync<T>(
            this Func<Task<T>> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatchAsync(action, logger, LogLevel.Warning, memberName);
        }

        /// <summary>
        /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// The elapsed time.
        /// </returns>
        public static Task<IOperationResult> WithInfoStopwatchAsync(
            this Func<Task> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatchAsync(action, logger, LogLevel.Info, memberName);
        }

        /// <summary>
        /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Info"/> level.
        /// </summary>
        /// <typeparam name="T">The operation return type.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// The elapsed time.
        /// </returns>
        public static Task<IOperationResult<T>> WithInfoStopwatchAsync<T>(
            this Func<Task<T>> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatchAsync(action, logger, LogLevel.Info, memberName);
        }

        /// <summary>
        /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Debug"/> level.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// The elapsed time.
        /// </returns>
        public static Task<IOperationResult> WithDebugStopwatchAsync(
            this Func<Task> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatchAsync(action, logger, LogLevel.Debug, memberName);
        }

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
        public static Task<IOperationResult<T>> WithDebugStopwatchAsync<T>(
            this Func<Task<T>> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatchAsync(action, logger, LogLevel.Debug, memberName);
        }

        /// <summary>
        /// Executes the action with a stopwatch for asynchronous actions, optionally logging the elapsed time at <see cref="LogLevel.Trace"/> level.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>
        /// The elapsed time.
        /// </returns>
        public static Task<IOperationResult> WithTraceStopwatchAsync(
            this Func<Task> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatchAsync(action, logger, LogLevel.Trace, memberName);
        }

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
        public static Task<IOperationResult<T>> WithTraceStopwatchAsync<T>(
            this Func<Task<T>> action,
            ILogger? logger = null,
            [CallerMemberName] string? memberName = null)
        {
            return WithStopwatchAsync<T>(action, logger, LogLevel.Trace, memberName);
        }

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
        public static async Task<IOperationResult> WithStopwatchAsync(
            this Func<Task> action,
            ILogger? logger = null,
            LogLevel logLevel = LogLevel.Debug,
            [CallerMemberName] string? memberName = null)
        {
            var result = new OperationResult();
            if (action == null)
            {
                return result.MergeMessage($"No action provided for {memberName}.");
            }

            result.MergeMessage($"{memberName}. Started at: {DateTime.Now:s}.");
            logger?.Log(logLevel, "{operation}. Started at: {startedAt:s}.", memberName, DateTime.Now);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await action().PreserveThreadContext();
            stopwatch.Stop();

            result
                .MergeMessage($"{memberName}. Ended at: {DateTime.Now:s}. Elapsed: {stopwatch.Elapsed:c}.")
                .Complete(stopwatch.Elapsed);
            logger?.Log(logLevel, "{operation}. Ended at: {endedAt:s}. Elapsed: {elapsed:c}.", memberName, DateTime.Now, stopwatch.Elapsed);

            return result;
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
        public static async Task<IOperationResult<T>> WithStopwatchAsync<T>(
            this Func<Task<T>> action,
            ILogger? logger = null,
            LogLevel logLevel = LogLevel.Debug,
            [CallerMemberName] string? memberName = null)
        {
            var result = new OperationResult<T>();
            if (action == null)
            {
                return result.MergeMessage($"No action provided for {memberName}.");
            }

            result.MergeMessage($"{memberName}. Started at: {DateTime.Now:s}.");
            logger?.Log(logLevel, "{operation}. Started at: {startedAt:s}.", memberName, DateTime.Now);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            result.Value = await action().PreserveThreadContext();
            stopwatch.Stop();

            result
                .MergeMessage($"{memberName}. Ended at: {DateTime.Now:s}. Elapsed: {stopwatch.Elapsed:c}.")
                .Complete(stopwatch.Elapsed);
            logger?.Log(logLevel, "{operation}. Ended at: {endedAt:s}. Elapsed: {elapsed:c}.", memberName, DateTime.Now, stopwatch.Elapsed);

            return result;
        }
    }
}