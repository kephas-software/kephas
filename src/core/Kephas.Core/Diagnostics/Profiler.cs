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
    using System.Threading.Tasks;

    /// <summary>
    /// Provides operations for profiling the code.
    /// </summary>
    public static class Profiler
    {
        /// <summary>
        /// Executes the action with a stopwatch.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The elapsed time.</returns>
        public static TimeSpan WithStopwatch(Action action)
        {
            if (action == null)
            {
                return new TimeSpan();
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            action();

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// Executes the action with a stopwatch for asynchronous actions.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The elapsed time.</returns>
        public static async Task<TimeSpan> WithStopwatchAsync(Func<Task> action)
        {
            if (action == null)
            {
                return new TimeSpan();
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await action();

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}