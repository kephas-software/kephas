// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Extension methods for the <see cref="Task" /> and <see cref="Task{TResult}" /> classes.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// Extension methods for the <see cref="Task"/> and <see cref="Task{TResult}"/> classes.
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// Gets or sets the default value of milliseconds to wait a task in a completion check cycle when simulating synchronous calls.
        /// The default value is 20 milliseconds, but it can be changed to accomodate application needs.
        /// </summary>
        public static int DefaultWaitMilliseconds { get; set; } = 20;

        /// <summary>
        /// Gets or sets the default timeout when waiting for task completion in simulating synchronous calls.
        /// The default value is 30 seconds, but it can be changed to accomodate application needs.
        /// </summary>
        public static TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

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

            timeout ??= DefaultTimeout;
            var timeoutOccurred = false;
            var waitInterval = waitMilliseconds ?? DefaultWaitMilliseconds;
            var startTime = DateTime.Now;

            AggregateException? taskException = null;
            try
            {
                while (!task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
                {
                    task.Wait(waitInterval);
                    var elapsed = DateTime.Now - startTime;
                    if (elapsed > timeout)
                    {
                        if (throwOnTimeout)
                        {
                            throw new TaskTimeoutException(task, $"The allotted time of {timeout} expired. Please try again the operation.");
                        }

                        timeoutOccurred = true;
                        break;
                    }
                }
            }
            catch (AggregateException ex)
            {
                taskException = ex;
            }

            if (taskException == null && task.IsFaulted)
            {
                taskException = task.Exception;
            }

            if (taskException != null)
            {
                if (taskException.InnerExceptions.Count == 1)
                {
                    throw taskException.InnerException;
                }

                throw taskException;
            }

            if (task.IsCanceled)
            {
                throw new TaskCanceledException(task);
            }

            return !timeoutOccurred;
        }

        /// <summary>
        /// Waits the task avoiding the current thread to be locked.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">Optional. The timeout. The default value is <see cref="DefaultTimeout"/>.</param>
        /// <param name="waitMilliseconds">
        /// Optional. The milliseconds used to wait until checking again the state of the task. The default value is
        /// <see cref="DefaultWaitMilliseconds"/>.</param>
        /// <param name="throwOnTimeout">Optional. If set to <c>true</c> an exception is thrown on timeout.</param>
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

            return default;
        }

#if NETSTANDARD2_1
        /// <summary>
        /// Waits the task avoiding the current thread to be locked.
        /// </summary>
        /// <param name="valueTask">The value task.</param>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WaitNonLocking(this ValueTask valueTask, TimeSpan? timeout = null, int? waitMilliseconds = null, bool throwOnTimeout = true)
        {
            return WaitNonLocking(valueTask.AsTask(), timeout, waitMilliseconds, throwOnTimeout);
        }

        /// <summary>
        /// Waits the task avoiding the current thread to be locked.
        /// </summary>
        /// <typeparam name="T">The return type.</typeparam>
        /// <param name="valueTask">The value task.</param>
        /// <param name="timeout">Optional. The timeout. The default value is <see cref="DefaultTimeout"/>.</param>
        /// <param name="waitMilliseconds">
        /// Optional. The milliseconds used to wait until checking again the state of the task. The default value is
        /// <see cref="DefaultWaitMilliseconds"/>.</param>
        /// <param name="throwOnTimeout">Optional. If set to <c>true</c> an exception is thrown on timeout.</param>
        /// <returns>
        /// The task result.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetResultNonLocking<T>(this ValueTask<T> valueTask, TimeSpan? timeout = null, int? waitMilliseconds = null, bool throwOnTimeout = true)
        {
            return GetResultNonLocking(valueTask.AsTask(), timeout, waitMilliseconds, throwOnTimeout);
        }
#endif

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
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ThreadContextAwaiter PreserveThreadContext(this Task task)
        {
            Requires.NotNull(task, nameof(task));

            return new ThreadContextAwaiter(task);
        }

#if NETSTANDARD2_1
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
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueThreadContextAwaiter<TResult> PreserveThreadContext<TResult>(this ValueTask<TResult> task)
        {
            return new ValueThreadContextAwaiter<TResult>(task);
        }

        /// <summary>
        /// Gets a task awaiter preserving the current context upon continuation.
        /// </summary>
        /// <remarks>
        /// The returned awaiter does not continue on the captured context (<see cref="Task.ConfigureAwait"/><c>(false)</c> is called),
        /// but it can be configured to preserve some thread properties.
        /// This awaiter is useful on the server, where <c>await</c> should not continue on the starting thread (like the UI does), but properties like the current culture should be preserved.
        /// </remarks>
        /// <param name="valueTask">The value task.</param>
        /// <returns>
        /// A <see cref="ThreadContextAwaiter"/>.
        /// </returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueThreadContextAwaiter PreserveThreadContext(this ValueTask valueTask)
        {
            return new ValueThreadContextAwaiter(valueTask);
        }
#endif

        /// <summary>
        /// Configures a timeout for the provided task. If the task ends within the indicated time,
        /// the original task result is returned, otherwise a <see cref="TaskTimeoutException"/> occurs.
        /// </summary>
        /// <remarks>
        /// The <see cref="TaskTimeoutException"/> contains the original task that timed out.
        /// </remarks>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// The result of the original task.
        /// </returns>
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout)).PreserveThreadContext();
            if (completedTask == task)
            {
                EnsureCompletedSuccessfully(task);
                return;
            }

            throw new TaskTimeoutException(task);
        }

        /// <summary>
        /// Configures a timeout for the provided task. If the task ends within the indicated time,
        /// the original task result is returned, otherwise a <see cref="TaskTimeoutException"/> occurs.
        /// </summary>
        /// <remarks>
        /// The <see cref="TaskTimeoutException"/> contains the original task that timed out.
        /// </remarks>
        /// <typeparam name="T">The task result type.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// The result of the original task.
        /// </returns>
        public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
        {
            var completedTask = await Task.WhenAny(task, Task.Delay(timeout)).PreserveThreadContext();
            if (completedTask == task)
            {
                EnsureCompletedSuccessfully(task);
                return task.Result;
            }

            throw new TaskTimeoutException(task);
        }

        /// <summary>
        /// Converts the action into an asynchronous operation.
        /// </summary>
        /// <remarks>
        /// Caution is to be considered because, when the action
        /// does not complete in the indicated time or when cancellation is issued,
        /// the task running the operation does not complete.
        /// In this case, the running task is returned in the Task property of either the TaskCanceledException
        /// or TaskTimeoutException.
        /// </remarks>
        /// <param name="action">The action to be executed.</param>
        /// <param name="cancellationToken">Optional. a token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task AsAsync(
            this Action action,
            CancellationToken cancellationToken = default)
        {
            return AsAsync(
                action,
                null,
                cancellationToken);
        }

        /// <summary>
        /// Converts the action into an asynchronous operation.
        /// </summary>
        /// <remarks>
        /// Caution is to be considered because, when the action
        /// does not complete in the indicated time or when cancellation is issued,
        /// the task running the operation does not complete.
        /// In this case, the running task is returned in the Task property of either the TaskCanceledException
        /// or TaskTimeoutException.
        /// </remarks>
        /// <param name="action">The action to be executed.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <param name="cancellationToken">Optional. a token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task AsAsync(
            this Action action,
            int timeout,
            CancellationToken cancellationToken = default)
        {
            return AsAsync(
                action,
                TimeSpan.FromMilliseconds(timeout),
                cancellationToken);
        }

        /// <summary>
        /// Converts the action into an asynchronous operation.
        /// </summary>
        /// <remarks>
        /// Caution is to be considered because, when the action
        /// does not complete in the indicated time or when cancellation is issued,
        /// the task running the operation does not complete.
        /// In this case, the running task is returned in the Task property of either the TaskCanceledException
        /// or TaskTimeoutException.
        /// </remarks>
        /// <param name="action">The action to be executed.</param>
        /// <param name="timeout">The timeout. If <c>null</c> specified, indefinite wait is considered.</param>
        /// <param name="cancellationToken">Optional. a token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static async Task AsAsync(
            this Action action,
            TimeSpan? timeout,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(action, nameof(action));

            var task = Task.Run(action, cancellationToken);
            Task? completedTask = null;
            try
            {
                completedTask = await AsAsync(task, timeout, cancellationToken).PreserveThreadContext();
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw new TaskCanceledException(task);
            }

            if (completedTask != task)
            {
                throw cancellationToken.IsCancellationRequested
                          ? (Exception)new TaskCanceledException(task)
                          : new TaskTimeoutException(task);
            }

            EnsureCompletedSuccessfully(task);
        }

        /// <summary>
        /// Converts the function into an asynchronous operation.
        /// </summary>
        /// <remarks>
        /// Caution is to be considered because, when the function does not complete in the indicated
        /// time or when cancellation is issued, the task running the operation does not complete.
        /// In this case, the running task is returned in the Task property of either the TaskCanceledException
        /// or TaskTimeoutException.
        /// </remarks>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="func">The function to be executed.</param>
        /// <param name="cancellationToken">Optional. a token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task<T> AsAsync<T>(
            this Func<T> func,
            CancellationToken cancellationToken = default)
        {
            return AsAsync(
                func,
                null,
                cancellationToken);
        }

        /// <summary>
        /// Converts the function into an asynchronous operation.
        /// </summary>
        /// <remarks>
        /// Caution is to be considered because, when the function does not complete in the indicated
        /// time or when cancellation is issued, the task running the operation does not complete.
        /// In this case, the running task is returned in the Task property of either the TaskCanceledException
        /// or TaskTimeoutException.
        /// </remarks>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="func">The function to be executed.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <param name="cancellationToken">Optional. a token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static Task<T> AsAsync<T>(
            this Func<T> func,
            int timeout,
            CancellationToken cancellationToken = default)
        {
            return AsAsync(
                func,
                TimeSpan.FromMilliseconds(timeout),
                cancellationToken);
        }

        /// <summary>
        /// Converts the function into an asynchronous operation.
        /// </summary>
        /// <remarks>
        /// Caution is to be considered because, when the function does not complete in the indicated
        /// time or when cancellation is issued, the task running the operation does not complete.
        /// In this case, the running task is returned in the Task property of either the TaskCanceledException
        /// or TaskTimeoutException.
        /// </remarks>
        /// <typeparam name="T">The function return type.</typeparam>
        /// <param name="func">The function to be executed.</param>
        /// <param name="timeout">The timeout. If <c>null</c> specified, indefinite wait is considered.</param>
        /// <param name="cancellationToken">Optional. a token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public static async Task<T> AsAsync<T>(
            this Func<T> func,
            TimeSpan? timeout,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNull(func, nameof(func));

            var task = Task.Run(func, cancellationToken);
            Task? completedTask = null;
            try
            {
                completedTask = await AsAsync(task, timeout, cancellationToken).PreserveThreadContext();
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (OperationCanceledException)
            {
                throw new TaskCanceledException(task);
            }

            if (task == completedTask)
            {
                EnsureCompletedSuccessfully(task);
                return task.Result;
            }

            throw cancellationToken.IsCancellationRequested
                      ? (Exception)new TaskCanceledException(task)
                      : new TaskTimeoutException(task);
        }

        /// <summary>
        /// Gets the result type of a generic task type.
        /// If the task does not return any result, <c>null</c> is returned.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>
        /// The result type.
        /// </returns>
        public static Type? GetResultType(this Task task)
        {
            Requires.NotNull(task, nameof(task));

            var taskType = task.GetType();
            return taskType.IsConstructedGenericType
                ? taskType.GetGenericArguments()[0]
                : null;
        }

        /// <summary>
        /// Gets the result of a task.
        /// If the task does not return any result, <c>null</c> is returned.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>
        /// The result type.
        /// </returns>
        public static object? GetResult(this Task task)
        {
            Requires.NotNull(task, nameof(task));

            var taskType = task.GetType();
            if (taskType.IsConstructedGenericType)
            {
                return task.GetPropertyValue(nameof(Task<int>.Result));
            }

            return null;
        }

        /// <summary>
        /// Ensures that the task completed successfully.
        /// </summary>
        /// <param name="task">The task.</param>
        public static void EnsureCompletedSuccessfully(this Task task)
        {
            Requires.NotNull(task, nameof(task));

            if (!task.IsCompleted)
            {
                throw new TaskNotCompletedException(task);
            }

            var taskException = task.Exception;
            if (taskException != null)
            {
                throw taskException.InnerExceptions.Count == 1 ? taskException.InnerExceptions[0] : taskException;
            }

            if (task.IsCanceled)
            {
                throw new TaskCanceledException(task);
            }
        }

        /// <summary>
        /// Ensures that the task completed successfully.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="task">The task.</param>
        /// <returns>The task result.</returns>
        public static TResult EnsureCompletedSuccessfully<TResult>(this Task<TResult> task)
        {
            EnsureCompletedSuccessfully((Task)task);
            return task.Result;
        }

        private static async Task<Task> AsAsync(
            Task task,
            TimeSpan? timeout = null,
            CancellationToken cancellationToken = default)
        {
            if (task.IsCompleted)
            {
                return task;
            }

            using var cts = new CancellationTokenSource();
            using var registration = cancellationToken.Register(() => cts.Cancel());
            var delayTask = timeout == null ? Task.Delay(-1, cts.Token) : Task.Delay(timeout.Value, cts.Token);
            var completedTask = await Task.WhenAny(task, delayTask).PreserveThreadContext();
            if (completedTask == task)
            {
                return task;
            }

            // TODO: the original task will not end.
            cts.Token.ThrowIfCancellationRequested();
            return task.IsCompleted ? task : delayTask;
        }
    }
}