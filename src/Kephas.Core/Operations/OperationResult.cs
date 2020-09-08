// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;

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
    using Kephas.Data.Formatting;
    using Kephas.Dynamic;

    /// <summary>
    /// Encapsulates the result of an operation.
    /// </summary>
    public class OperationResult : Expando, IOperationResult, IDataFormattable
    {
        private object? value;
        private OperationState operationState;
        private float percentCompleted;
        private TimeSpan elapsed;
        private OperationResultAwaiter? awaiter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        public OperationResult()
        {
            this.Exceptions = new ConcurrentCollection<Exception>();
            this.Messages = new ConcurrentCollection<IOperationMessage>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="value">The return value.</param>
        public OperationResult(object value)
            : this()
        {
            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public OperationResult(Task task)
            : this()
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
        /// Gets or sets the elapsed time.
        /// </summary>
        /// <value>
        /// The elapsed time.
        /// </value>
        public virtual TimeSpan Elapsed
        {
            get => this.elapsed;
            set => this.SetProperty(ref this.elapsed, value);
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public ICollection<IOperationMessage> Messages { get; set; }

        /// <summary>
        /// Gets the exceptions.
        /// </summary>
        /// <value>
        /// The exceptions.
        /// </value>
        public ICollection<Exception> Exceptions { get; }

        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        public OperationResultAwaiter GetAwaiter() => this.awaiter ?? this.CreateAwaiter();

        /// <summary>
        /// Converts this object to a task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public Task AsTask() => this.awaiter?.GetTask() ?? this.CreateTask();

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
        public virtual object ToData(IDataFormattingContext? context = null)
        {
            return new Expando
            {
                [nameof(this.OperationState)] = this.OperationState,
                [nameof(this.Elapsed)] = this.Elapsed,
                [nameof(this.PercentCompleted)] = this.PercentCompleted,
                [nameof(this.Messages)] = this.Messages.Select(m => m.ToData(context)).ToArray(),
                [nameof(this.Exceptions)] = this.Exceptions.Select(e => new ExceptionData(e)),
                [nameof(this.Value)] = this.OperationState == OperationState.Completed
                    ? this.Value.ToData(context)
                    : null,
            };
        }

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
        /// Creates the operation result awaiter.
        /// </summary>
        /// <returns>
        /// The operation result awaiter.
        /// </returns>
        protected virtual OperationResultAwaiter CreateAwaiter()
        {
            return new OperationResultAwaiter<object?>(Task.FromResult(this.Value));
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

        /// <summary>
        /// Internal implementation of a concurrent collection.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        private class ConcurrentCollection<T> : ConcurrentQueue<T>, ICollection<T>
        {
            /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</summary>
            /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.</returns>
            public bool IsReadOnly => false;

            /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
            /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
            /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
            public void Add(T item)
            {
                this.Enqueue(item);
            }

#if NETSTANDARD2_1
#else
            /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
            /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
            void ICollection<T>.Clear()
            {
                while (this.TryDequeue(out _)) { }
            }
#endif

            /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.</summary>
            /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
            /// <returns>true if <paramref name="item">item</paramref> is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.</returns>
            public bool Contains(T item)
            {
                return ((IEnumerable<T>)this).Contains(item);
            }

            /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</summary>
            /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
            /// <returns>true if <paramref name="item">item</paramref> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if <paramref name="item">item</paramref> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.</returns>
            /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.</exception>
            bool ICollection<T>.Remove(T item)
            {
                throw new NotSupportedException();
            }
        }
    }

    /// <summary>
    /// Encapsulates the result of an operation.
    /// </summary>
    /// <typeparam name="TValue">Type of the value.</typeparam>
    public class OperationResult<TValue> : OperationResult, IOperationResult<TValue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{TValue}"/> class.
        /// </summary>
        public OperationResult()
            : this((TValue)default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{TValue}"/> class.
        /// </summary>
        /// <param name="value">The return value.</param>
        public OperationResult(TValue value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{TValue}"/> class.
        /// </summary>
        /// <param name="task">The task.</param>
        public OperationResult(Task<TValue> task)
            : this((TValue)default)
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
            get => (TValue)base.Value;
            set => base.Value = value;
        }

        /// <summary>
        /// Gets the operation result awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        public new OperationResultAwaiter<TValue> GetAwaiter() => (OperationResultAwaiter<TValue>)base.GetAwaiter();

        /// <summary>
        /// Converts this object to a task.
        /// </summary>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        public new Task<TValue> AsTask() => (Task<TValue>)base.AsTask();

        /// <summary>
        /// Gets the default operation result awaiter.
        /// </summary>
        /// <returns>
        /// The default operation result awaiter.
        /// </returns>
        protected override OperationResultAwaiter CreateAwaiter()
        {
            return new OperationResultAwaiter<TValue>(Task.FromResult(this.Value));
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