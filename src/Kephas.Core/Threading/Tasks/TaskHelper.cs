// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for the <see cref="Task" /> and <see cref="Task{TResult}" /> classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="Task"/> and <see cref="Task{TResult}"/> classes.
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// Initializes static members of the <see cref="TaskHelper"/> class.
        /// </summary>
        static TaskHelper()
        {
            CompletedTask = Task.FromResult(0);
            DefaultWaitMilliseconds = 20;
            DefaultTimeout = TimeSpan.FromSeconds(30);
        }

        /// <summary>
        /// Gets or sets the default value of milliseconds to wait a task in a completion check cycle when simulating synchronous calls.
        /// The default value is 20 milliseconds, but it can be changed to accomodate application needs.
        /// </summary>
        public static int DefaultWaitMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the default timeout when waiting for task completion in simulating synchronous calls.
        /// The default value is 30 seconds, but it can be changed to accomodate application needs.
        /// </summary>
        public static TimeSpan DefaultTimeout { get; set; }

        /// <summary>
        /// Gets a task that has already completed successfully.
        /// </summary>
        /// <value>
        /// A successfully completed task.
        /// </value>
        public static Task CompletedTask { get; }

        /// <summary>
        /// Waits the task avoiding the current thread to be locked.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout (optional). The default value is <see cref="DefaultTimeout"/>.</param>
        /// <param name="waitMilliseconds">The milliseconds used to wait until checking again the state of the task (optional). The default value is <see cref="DefaultWaitMilliseconds"/>.</param>
        /// <param name="throwOnTimeout">If set to <c>true</c> an exception is thrown on timeout.</param>
        /// <returns>
        ///   <c>true</c> if the task completed execution within the allotted time; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// It is a bad practice to run synchronously tasks meant to be async "by birth".
        /// However, if there is no other chance than waiting for a task to complete synchronously,
        /// DO NOT USE task.Wait(), because there are situations when it deadlocks the thread.
        /// An option would be to use task.ConfigureAwait(false).Wait(), but all the tasks down
        /// the task chain must be exactly the same way configured, which may not not be always the case.
        /// An alternative implementation might be the one provided below, but this must be tried if it really works:
        /// http://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously.
        /// For more information see also http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
        /// and http://blogs.msdn.com/b/pfxteam/archive/2012/04/13/10293638.aspx.
        /// </remarks>
        public static bool WaitNonLocking(this Task task, TimeSpan? timeout = null, int? waitMilliseconds = null, bool throwOnTimeout = true)
        {
            Requires.NotNull(task, nameof(task));

            timeout = timeout ?? DefaultTimeout;
            var timeoutOccurred = false;
            var waitInterval = waitMilliseconds ?? DefaultWaitMilliseconds;
            var startTime = DateTime.Now;
            while (!task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                task.Wait(waitInterval);
                var elapsed = DateTime.Now - startTime;
                if (elapsed > timeout)
                {
                    if (throwOnTimeout)
                    {
                        throw new TimeoutException($"The allotted time of {timeout} expired. Please try again the operation.");
                    }

                    timeoutOccurred = true;
                    break;
                }
            }

            if (task.IsFaulted)
            {
                throw task.Exception;
            }

            return !timeoutOccurred;
        }

        /// <summary>
        /// Waits the task avoiding the current thread to be locked.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout (optional). The default value is <see cref="DefaultTimeout"/>.</param>
        /// <param name="waitMilliseconds">The milliseconds used to wait until checking again the state of the task (optional). The default value is <see cref="DefaultWaitMilliseconds"/>.</param>
        /// <param name="throwOnTimeout">If set to <c>true</c> an exception is thrown on timeout.</param>
        /// <returns>
        /// The task result.
        /// </returns>
        public static T GetResultNonLocking<T>(this Task<T> task, TimeSpan? timeout = null, int? waitMilliseconds = null, bool throwOnTimeout = true)
        {
            Requires.NotNull(task, nameof(task));

            if (task.WaitNonLocking(timeout, waitMilliseconds, throwOnTimeout))
            {
                return task.Result;
            }

            return default(T);
        }

        /// <summary>
        /// Gets a task awaiter preserving the current context upon continuation.
        /// </summary>
        /// <remarks>
        /// The returned awaiter does not continue on the captured context (<see cref="Task.ConfigureAwait"/><c>(false)</c> is called),
        /// but it can be configured to preserve some thread properties.
        /// This awaiter is useful on the server, where <c>await</c> should not continue on the starting thread (like the UI does), but properties like the current culture should be preserved.
        /// </remarks>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>
        /// A <see cref="ThreadContextAwaiter{TResult}"/>.
        /// </returns>
        public static ThreadContextAwaiter<TResult> PreserveThreadContext<TResult>(this Task<TResult> task)
        {
            Requires.NotNull(task, nameof(task));

            return new ThreadContextAwaiter<TResult>(task);
        }

        /// <summary>
        /// Gets a task awaiter preserving the current context upon continuation.
        /// </summary>
        /// <remarks>
        /// The returned awaiter does not continue on the captured context (<see cref="Task.ConfigureAwait"/><c>(false)</c> is called),
        /// but it can be configured to preserve some thread properties.
        /// This awaiter is useful on the server, where <c>await</c> should not continue on the starting thread (like the UI does), but properties like the current culture should be preserved.
        /// </remarks>
        /// <param name="task">The task.</param>
        /// <returns>
        /// A <see cref="ThreadContextAwaiter"/>.
        /// </returns>
        public static ThreadContextAwaiter PreserveThreadContext(this Task task)
        {
            Requires.NotNull(task, nameof(task));

            return new ThreadContextAwaiter(task);
        }
    }
}