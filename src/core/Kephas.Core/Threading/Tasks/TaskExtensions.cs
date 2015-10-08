// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskExtensions.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Extension methods for the <see cref="Task" /> and <see cref="Task{TResult}" /> classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for the <see cref="Task"/> and <see cref="Task{TResult}"/> classes.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// The default value of milliseconds to wait when simulating synchronous calls.
        /// </summary>
        public const int DefaultWaitMilliseconds = 20;

        /// <summary>
        /// Waits the task avoiding the current thread to be locked.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="waitMilliseconds">The milliseconds used to wait until checking again the state of the task.</param>
        /// <param name="throwOnTimeout">If set to <c>true</c> an exception is thrown on timeout.</param>
        /// <returns>
        ///   <c>true</c> if the task completed execution within the allotted time; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// It is a bad practice to run synchronously tasks meant to be async "by birth".
        /// However, if there is no other chance than waiting for a task to complete synchronously,
        /// DO NOT USE task.Wait(), because there are situations when it deadlocks the thread.
        /// An option would be to use task.ConfigureAwait(false).Wait(), but all the tasks down
        /// the task chain must be exactly the same way configured, which is not practicable.
        /// An alternative implementation might be the one provided below, but this must be tried if it really works:
        /// http://stackoverflow.com/questions/5095183/how-would-i-run-an-async-taskt-method-synchronously.
        /// For more information see also http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
        /// and http://blogs.msdn.com/b/pfxteam/archive/2012/04/13/10293638.aspx.
        /// </remarks>
        public static bool WaitNonLocking(this Task task, TimeSpan timeout, int? waitMilliseconds = null, bool throwOnTimeout = true)
        {
            Contract.Requires(task != null);

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

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Waits the task avoiding the current thread to be locked.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="waitMilliseconds">The milliseconds used to wait until checking again the state of the task.</param>
        /// <param name="throwOnTimeout">If set to <c>true</c> an exception is thrown on timeout.</param>
        /// <returns>
        /// The task result.
        /// </returns>
        public static T GetResultNonLocking<T>(this Task<T> task, TimeSpan timeout, int? waitMilliseconds = null, bool throwOnTimeout = true)
        {
            Contract.Requires(task != null);

            if (task.WaitNonLocking(timeout, waitMilliseconds, throwOnTimeout))
            {
                if (task.Exception != null && task.Exception.InnerExceptions.Count == 1)
                {
                    throw task.Exception.InnerException;
                }

                return task.Result;
            }

            return default(T);
        }

        /// <summary>
        /// Gets a task awaiter preserving the current server context upon continuation.
        /// </summary>
        /// <remarks>
        /// ConfigureAwait(false) is called and the current culture and current UI culture are preserved.
        /// </remarks>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>
        /// A <see cref="WithServerContextAwaiter{TResult}"/>.
        /// </returns>
        public static WithServerContextAwaiter<TResult> WithServerContext<TResult>(this Task<TResult> task)
        {
            Contract.Requires(task != null);

            return new WithServerContextAwaiter<TResult>(task);
        }

        /// <summary>
        /// Gets a task awaiter preserving the current culture upon continuation.
        /// </summary>
        /// <remarks>
        /// ConfigureAwait(false) is called and the current culture and current UI culture are preserved.
        /// </remarks>
        /// <param name="task">The task.</param>
        /// <returns>
        /// A <see cref="WithServerContextAwaiter"/>.
        /// </returns>
        public static WithServerContextAwaiter WithServerContext(this Task task)
        {
            Contract.Requires(task != null);

            return new WithServerContextAwaiter(task);
        }
    }
}