// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Collections.Concurrent;
    using Kephas.Data.Formatting;
    using Kephas.Dynamic;

    /// <summary>
    /// Encapsulates the result of an operation.
    /// </summary>
    public record OperationResult : IExpandoMixin, IOperationResult, IDataFormattable
    {
        private ConcurrentDictionary<string, object?>? dynamicData;
        private object? value;
        private OperationState operationState;
        private float percentCompleted;
        private DateTimeOffset? startedAt = DateTimeOffset.Now;
        private DateTimeOffset? endedAt;
        private TimeSpan? elapsed;
        private OperationResultAwaiter? awaiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        public OperationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="value">The return value.</param>
        public OperationResult(object value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public OperationResult(Task task)
        {
            this.SetAwaiter(OperationResultAwaiter.Create(task, this.UpdateInternalState));
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets the return value.
        /// </summary>
        /// <value>
        /// The return value.
        /// </value>
        public object? Value
        {
            get
            {
                if (this.awaiter != null)
                {
                    var awaiterTask = this.awaiter.GetTask();
                    var taskStatus = awaiterTask.Status;
                    return awaiterTask.IsCompleted
                        ? this.awaiter.GetResult()
                        : throw new InvalidOperationException($"The awaited task did not complete execution (state: {taskStatus}).");
                }

                return this.value;
            }

            set
            {
                if (this.awaiter != null)
                {
                    throw new InvalidOperationException($"Cannot set the return value when awaiting a task.");
                }

                this.SetProperty(ref this.value, value);
            }
        }

        /// <summary>
        /// Gets or sets the state of the operation.
        /// </summary>
        /// <value>
        /// The state of the operation.
        /// </value>
        public OperationState OperationState
        {
            get => this.operationState;
            set => this.SetProperty(ref this.operationState, value);
        }

        /// <summary>
        /// Gets or sets the percent completed.
        /// </summary>
        /// <value>
        /// The percent completed.
        /// </value>
        public virtual float PercentCompleted
        {
            get => this.percentCompleted;
            set => this.SetProperty(ref this.percentCompleted, value);
        }

        /// <summary>
        /// Gets or sets the time when the job started.
        /// </summary>
        public DateTimeOffset? StartedAt
        {
            get => this.startedAt;
            set => this.SetProperty(ref this.startedAt, value);
        }

        /// <summary>
        /// Gets or sets the time when the job ended.
        /// </summary>
        public DateTimeOffset? EndedAt
        {
            get => this.endedAt;
            set => this.SetProperty(ref this.endedAt, value);
        }

        /// <summary>
        /// Gets or sets the elapsed time.
        /// </summary>
        /// <value>
        /// The elapsed time.
        /// </value>
        public virtual TimeSpan Elapsed
        {
            get => this.elapsed ?? (this.startedAt.HasValue ? DateTimeOffset.Now - this.startedAt.Value : TimeSpan.Zero);
            set => this.SetProperty(ref this.elapsed, value);
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ICollection<IOperationMessage> Messages { get; set; } = new ConcurrentCollection<IOperationMessage>();

        /// <summary>
        /// Converts this object to a task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public Task<object?> AsTask()
        {
            var task = this.GetUnderlyingTask();
            return task as Task<object?> ?? task.ContinueWith(t => this.Value);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return $"{this.OperationState} ({this.PercentCompleted:P1})";
        }

        /// <summary>
        /// Converts this object to a serialization friendly representation.
        /// </summary>
        /// <param name="context">Optional. The formatting context.</param>
        /// <returns>A serialization friendly object representing this object.</returns>
        public virtual object ToData(object? context = null)
        {
            return new Expando
            {
                [nameof(this.OperationState)] = this.OperationState,
                [nameof(this.Elapsed)] = this.Elapsed,
                [nameof(this.PercentCompleted)] = this.PercentCompleted,
                [nameof(this.Messages)] = this.Messages.Count == 0 ? null : this.Messages.Select(m => m.ToData(context)).ToArray(),
                [nameof(this.Value)] = this.OperationState == OperationState.Completed
                    ? this.Value.ToData(context)
                    : null,
            };
        }

        /// <summary>
        /// Gets the inner dictionary.
        /// </summary>
        IDictionary<string, object?> IExpandoMixin.InnerDictionary => this.dynamicData ??= new();

        /// <summary>
        /// Gets the underlying task.
        /// </summary>
        /// <returns>The underlying task.</returns>
        protected Task GetUnderlyingTask() => this.awaiter?.GetTask() ?? this.CreateTask();

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Creates the task representing this operation result.
        /// </summary>
        /// <returns>
        /// The task representing this operation result.
        /// </returns>
        protected virtual Task CreateTask()
        {
            return Task.FromResult(this.Value);
        }

        /// <summary>
        /// Sets an awaiter.
        /// </summary>
        /// <remarks>
        /// Should not be called outside of the constructor context.
        /// </remarks>
        /// <param name="awaiter">The awaiter.</param>
        protected void SetAwaiter(OperationResultAwaiter awaiter)
        {
            this.awaiter = awaiter;
        }

        /// <summary>
        /// Updates the internal state described by the provided task.
        /// </summary>
        /// <param name="t">A Task to process.</param>
        protected void UpdateInternalState(Task t)
        {
            var opState = t.IsFaulted
                ? OperationState.Failed
                : t.IsCanceled
                    ? OperationState.Canceled
                    : t.Status == TaskStatus.RanToCompletion
                        ? OperationState.Completed
                        : t.Status == TaskStatus.Running
                            ? OperationState.InProgress
                            : OperationState.NotStarted;

            if (opState == this.operationState)
            {
                return;
            }

            this.OperationState = opState;
            if (t.Exception != null)
            {
                t.Exception.InnerExceptions.ForEach(ex => this.MergeException(ex));
            }
            else if (this.OperationState == OperationState.Completed)
            {
                this.PercentCompleted = 1f;
            }
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T">The field type.</typeparam>
        /// <param name="field">The field.</param>
        /// <param name="value">The value.</param>
        /// <param name="name">The name.</param>
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return;
            }

            field = value;
            this.OnPropertyChanged(name);
        }
    }

    /// <summary>
    /// Encapsulates the result of an operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public record OperationResult<TValue> : OperationResult, IOperationResult<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{TValue}"/> class.
        /// </summary>
        public OperationResult()
            : this((TValue)default!)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{TValue}"/> class.
        /// </summary>
        /// <param name="value">The return value.</param>
        public OperationResult(TValue value)
            : base(value!)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{TValue}"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public OperationResult(Task<TValue> task)
            : this((TValue)default!)
        {
            this.SetAwaiter(OperationResultAwaiter.Create(task, this.UpdateInternalState));
        }

        /// <summary>
        /// Gets or sets the return value.
        /// </summary>
        /// <value>
        /// The return value.
        /// </value>
        public new TValue Value
        {
            get => (TValue)base.Value!;
            set => base.Value = value;
        }

        /// <summary>
        /// Converts this object to a task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public new Task<TValue> AsTask()
        {
            var task = this.GetUnderlyingTask();
            return task as Task<TValue> ?? task.ContinueWith(t => this.Value);
        }

        /// <summary>
        /// Creates the task representing this operation result.
        /// </summary>
        /// <returns>
        /// The task representing this operation result.
        /// </returns>
        protected override Task CreateTask() => Task.FromResult(this.Value);
    }
}