// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThreadContextAwaiter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Awaiter preserving the context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading.Tasks
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Awaiter preserving the thread context.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    [DebuggerStepThrough]
    public class ThreadContextAwaiter<TResult> : INotifyCompletion, ICriticalNotifyCompletion
    {
        /// <summary>
        /// The awaiter.
        /// </summary>
        private readonly ConfiguredTaskAwaitable<TResult>.ConfiguredTaskAwaiter awaiter;

        /// <summary>
        /// Thread context for the server.
        /// </summary>
        private readonly ThreadContext threadContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextAwaiter{TResult}"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public ThreadContextAwaiter(Task<TResult> task)
        {
            task = task ?? throw new ArgumentNullException(nameof(task));

            var configuredTaskAwaitable = task.ConfigureAwait(false);
            this.awaiter = configuredTaskAwaitable.GetAwaiter();
            this.threadContext = new ThreadContextBuilder().CreateThreadContext();
        }

        /// <summary>
        /// Gets a value indicating whether the awaiter is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is completed, <c>false</c> if not.
        /// </value>
        public bool IsCompleted => this.awaiter.IsCompleted;

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
        public void OnCompleted(Action continuation)
        {
            this.threadContext.Store();
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            this.awaiter.OnCompleted(continuation);
        }

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
        public void UnsafeOnCompleted(Action continuation)
        {
            this.threadContext.Store();
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            this.awaiter.UnsafeOnCompleted(continuation);
        }

        /// <summary>
        /// Gets this awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        public ThreadContextAwaiter<TResult> GetAwaiter() => this;

        /// <summary>
        /// Notifies the awaiter to get the result.
        /// </summary>
        /// <returns>
        /// The result returned by the task.
        /// </returns>
        [DebuggerStepThrough]
        public TResult GetResult()
        {
            this.threadContext.Restore();
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            return this.awaiter.GetResult();
        }
    }

    /// <summary>
    /// Awaiter preserving the thread context.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    [DebuggerStepThrough]
    public class ThreadContextAwaiter : INotifyCompletion, ICriticalNotifyCompletion
    {
        private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter awaiter;
        private readonly ThreadContext threadContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadContextAwaiter"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public ThreadContextAwaiter(Task task)
        {
            task = task ?? throw new ArgumentNullException(nameof(task));

            var configuredTaskAwaitable = task.ConfigureAwait(false);
            this.awaiter = configuredTaskAwaitable.GetAwaiter();
            this.threadContext = new ThreadContextBuilder().CreateThreadContext();
        }

        /// <summary>
        /// Gets a value indicating whether the awaiter is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is completed, <c>false</c> if not.
        /// </value>
        public bool IsCompleted => this.awaiter.IsCompleted;

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
        public void OnCompleted(Action continuation)
        {
            this.threadContext.Store();
            this.awaiter.OnCompleted(continuation);
        }

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param><exception cref="T:System.ArgumentNullException">The <paramref name="continuation"/> argument is null (Nothing in Visual Basic).</exception>
        public void UnsafeOnCompleted(Action continuation)
        {
            this.threadContext.Store();

            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            this.awaiter.UnsafeOnCompleted(continuation);
        }

        /// <summary>
        /// Gets this awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        public ThreadContextAwaiter GetAwaiter() => this;

        /// <summary>
        /// Notifies the awaiter to get the result.
        /// </summary>
        [DebuggerStepThrough]
        public void GetResult()
        {
            this.threadContext.Restore();
            this.awaiter.GetResult();
        }
    }
}
