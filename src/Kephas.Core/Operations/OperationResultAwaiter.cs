// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultAwaiter.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result awaiter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Operations
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// An operation result awaiter.
    /// </summary>
    public class OperationResultAwaiter : ICriticalNotifyCompletion, INotifyCompletion
    {
        private static readonly MethodInfo CreateMethodInfo = ReflectionHelper.GetGenericMethodOf(_ => Create<int>(null, null));

        private readonly Task task;
        private object? awaiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResultAwaiter"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public OperationResultAwaiter(Task task)
        {
            Requires.NotNull(task, nameof(task));

            this.task = task;
        }

        /// <summary>
        /// Gets a value indicating whether the awaiter is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is completed, <c>false</c> if not.
        /// </value>
        public virtual bool IsCompleted => this.GetAwaiter().IsCompleted;

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        public ThreadContextAwaiter GetAwaiter() => (ThreadContextAwaiter)this.GetAwaiterCore();

        /// <summary>
        /// Notifies the awaiter to get the result.
        /// </summary>
        /// <returns>
        /// The result.
        /// </returns>
        public object? GetResult() => this.GetResultCore();

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param>
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
            ((ICriticalNotifyCompletion)this.GetAwaiterCore()).UnsafeOnCompleted(continuation);
        }

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <param name="continuation">The action to invoke when the operation completes.</param>
        void INotifyCompletion.OnCompleted(Action continuation)
        {
            ((INotifyCompletion)this.GetAwaiterCore()).OnCompleted(continuation);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResultAwaiter"/> from the provided task. If the task
        /// returns a result, the appropriate <see cref="OperationResultAwaiter{TResult}"/> is created
        /// and returned.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="updateState">The action to invoke when the operation completes.</param>
        /// <returns>
        /// An OperationResultAwaiter.
        /// </returns>
        internal static OperationResultAwaiter Create(Task task, Action<Task> updateState)
        {
            Requires.NotNull(task, nameof(task));

            var taskResultType = task.GetResultType();
            if (taskResultType == null)
            {
                updateState(task);
                return new OperationResultAwaiter(task.ContinueWith(t => { updateState(t); t.WaitNonLocking(); }));
            }

            var createMethodInfo = CreateMethodInfo.MakeGenericMethod(taskResultType);
            return (OperationResultAwaiter)createMethodInfo.Call(null, task, updateState);
        }

        /// <summary>
        /// Creates a new <see cref="OperationResultAwaiter"/> from the provided task. If the task
        /// returns a result, the appropriate <see cref="OperationResultAwaiter{TResult}"/> is created
        /// and returned.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="task">The task.</param>
        /// <param name="updateState">The action to invoke when the operation completes.</param>
        /// <returns>
        /// An OperationResultAwaiter.
        /// </returns>
        internal static OperationResultAwaiter<TResult> Create<TResult>(Task<TResult> task, Action<Task> updateState)
        {
            Requires.NotNull(task, nameof(task));

            updateState(task);
            return new OperationResultAwaiter<TResult>(task.ContinueWith(t => { updateState(t); return t.GetResultNonLocking(); }));
        }

        /// <summary>
        /// Gets the task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected internal Task GetTask() => this.task;

        /// <summary>
        /// Gets the awaiter (core implementation).
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        protected virtual object GetAwaiterCore() => this.awaiter ??= this.CreateTaskAwaiter();

        /// <summary>
        /// Creates the task awaiter.
        /// </summary>
        /// <returns>
        /// The new task awaiter.
        /// </returns>
        protected virtual object CreateTaskAwaiter() => (this.task ?? Task.CompletedTask).PreserveThreadContext();

        /// <summary>
        /// Gets the result (core implementation).
        /// </summary>
        /// <returns>
        /// The result.
        /// </returns>
        protected virtual object? GetResultCore()
        {
            this.GetAwaiter().GetResult();
            return null;
        }
    }

    /// <summary>
    /// An operation result awaiter.
    /// </summary>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public class OperationResultAwaiter<TResult> : OperationResultAwaiter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResultAwaiter{TResult}"/>
        /// class.
        /// </summary>
        /// <param name="task">The task.</param>
        public OperationResultAwaiter(Task<TResult> task)
            : base(task)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the awaiter is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is completed, <c>false</c> if not.
        /// </value>
        public override bool IsCompleted => this.GetAwaiter().IsCompleted;

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        public new ThreadContextAwaiter<TResult> GetAwaiter() => (ThreadContextAwaiter<TResult>)this.GetAwaiterCore();

        /// <summary>
        /// Notifies the awaiter to get the result.
        /// </summary>
        /// <returns>
        /// The result.
        /// </returns>
        public new TResult GetResult() => this.GetAwaiter().GetResult();

        /// <summary>
        /// Creates the task awaiter.
        /// </summary>
        /// <returns>
        /// The new task awaiter.
        /// </returns>
        protected override object CreateTaskAwaiter()
            => ((Task<TResult>)this.GetTask() ?? Task.FromResult<TResult>(default)).PreserveThreadContext();

        /// <summary>
        /// Gets the result (core implementation).
        /// </summary>
        /// <returns>
        /// The result.
        /// </returns>
        protected override object? GetResultCore() => this.GetAwaiter().GetResult();
    }
}
