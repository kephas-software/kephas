// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Profiler.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        public static TimeSpan WithWarningStopwatch(
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
        public static TimeSpan WithInfoStopwatch(
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
        public static TimeSpan WithDebugStopwatch(
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
        public static TimeSpan WithTraceStopwatch(
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
        public static TimeSpan WithStopwatch(Action action, ILogger logger = null, LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string memberName = null)
        {
            if (action == null)
            {
                return TimeSpan.Zero;
            }

            logger?.Log(logLevel, $"{memberName}. Started at: {DateTime.Now:s}.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();

            logger?.Log(logLevel, $"{memberName}. Ended at: {DateTime.Now:s}. Elapsed: {stopwatch.Elapsed:c}.");

            return stopwatch.Elapsed;
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
        public static Task<TimeSpan> WithWarningStopwatchAsync(
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
        public static Task<TimeSpan> WithInfoStopwatchAsync(
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
        public static Task<TimeSpan> WithDebugStopwatchAsync(
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
        public static Task<TimeSpan> WithTraceStopwatchAsync(
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
        public static async Task<TimeSpan> WithStopwatchAsync(Func<Task> action, ILogger logger = null, LogLevel logLevel = LogLevel.Debug, [CallerMemberName] string memberName = null)
        {
            if (action == null)
            {
                return TimeSpan.Zero;
            }

            logger?.Log(logLevel, $"{memberName}. Started at: {DateTime.Now:s}.");

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await action().PreserveThreadContext();
            stopwatch.Stop();

            logger?.Log(logLevel, $"{memberName}. Ended at: {DateTime.Now:s}. Elapsed: {stopwatch.Elapsed:c}.");

            return stopwatch.Elapsed;
        }
    }
}