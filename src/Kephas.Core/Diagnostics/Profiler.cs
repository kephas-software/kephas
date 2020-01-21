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
            Action action,
            ILogger logger = null,
            [CallerMemberName] string memberName = null)
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
            Action action,
            ILogger logger = null,
            [CallerMemberName] string memberName = null)
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
            Action action,
            ILogger logger = null,
            [CallerMemberName] string memberName = null)
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
            Action action,
            ILogger logger = null,
            [CallerMemberName] string memberName = null)
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
        public static IOperationResult WithStopwatch(Action action, ILogger logger = null, LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string memberName = null)
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

            result.MergeMessage($"{memberName}. Ended at: {DateTime.Now:s}. Elapsed: {stopwatch.Elapsed:c}.")
                .Elapsed(stopwatch.Elapsed)
                .OperationState(OperationState.Completed);
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
            Func<Task> action,
            ILogger logger = null,
            [CallerMemberName] string memberName = null)
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
            Func<Task> action,
            ILogger logger = null,
            [CallerMemberName] string memberName = null)
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
            Func<Task> action,
            ILogger logger = null,
            [CallerMemberName] string memberName = null)
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
            Func<Task> action,
            ILogger logger = null,
            [CallerMemberName] string memberName = null)
        {
            return WithStopwatchAsync(action, logger, LogLevel.Trace, memberName);
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
        public static async Task<IOperationResult> WithStopwatchAsync(Func<Task> action, ILogger logger = null, LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string memberName = null)
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

            result.MergeMessage($"{memberName}. Ended at: {DateTime.Now:s}. Elapsed: {stopwatch.Elapsed:c}.")
                .Elapsed(stopwatch.Elapsed)
                .OperationState(OperationState.Completed);
            logger?.Log(logLevel, "{operation}. Ended at: {endedAt:s}. Elapsed: {elapsed:c}.", memberName, DateTime.Now, stopwatch.Elapsed);

            return result;
        }
    }
}