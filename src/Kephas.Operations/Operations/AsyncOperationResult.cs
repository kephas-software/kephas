// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncOperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

using Kephas.Collections;

/// <summary>
/// An asynchronous operation result.
/// </summary>
public record AsyncOperationResult : OperationResult, IAsyncOperationResult
{
    private AsyncOperationResultAwaiter? awaiter;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncOperationResult"/> class.
    /// </summary>
    /// <param name="task">The task.</param>
    public AsyncOperationResult(Task task)
    {
        _ = task ?? throw new ArgumentNullException(nameof(task));

        this.SetAwaiter(AsyncOperationResultAwaiter.Create(task, this.UpdateInternalState));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncOperationResult"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    protected AsyncOperationResult(object? value = null)
        : base(value)
    {
    }

    /// <summary>
    /// Sets an awaiter.
    /// </summary>
    /// <remarks>
    /// Should not be called outside of the constructor context.
    /// </remarks>
    /// <param name="awaiter">The awaiter.</param>
    protected void SetAwaiter(AsyncOperationResultAwaiter awaiter)
    {
        this.awaiter = awaiter;
    }

    /// <summary>
    /// Gets or sets the return value.
    /// </summary>
    /// <value>
    /// The return value.
    /// </value>
    public override object? Value
    {
        get
        {
            if (this.awaiter != null)
            {
                var awaiterTask = this.awaiter.GetTask();
                var taskStatus = awaiterTask.Status;
                return awaiterTask.IsCompleted
                    ? this.awaiter.GetResult()
                    : throw new InvalidOperationException(
                        $"The awaited task did not complete execution (state: {taskStatus}).");
            }

            return base.Value;
        }

        set
        {
            if (this.awaiter != null)
            {
                throw new InvalidOperationException($"Cannot set the return value when awaiting a task.");
            }

            base.Value = value;
        }
    }

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
    /// Gets the operation awaiter.
    /// </summary>
    /// <returns>The operation awaiter.</returns>
    public AsyncOperationResultAwaiter GetAwaiter() =>
        this.awaiter ?? throw new InvalidOperationException("The awaiter has not been set.");

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
    /// Gets the underlying task.
    /// </summary>
    /// <returns>The underlying task.</returns>
    protected Task GetUnderlyingTask() => this.awaiter?.GetTask() ?? this.CreateTask();

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

        if (opState == this.OperationState)
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
}

/// <summary>
/// Encapsulates the result of an asynchronous operation.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public record AsyncOperationResult<TValue> : AsyncOperationResult, IAsyncOperationResult<TValue>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncOperationResult{TValue}"/> class.
    /// </summary>
    /// <param name="task">The task.</param>
    public AsyncOperationResult(Task<TValue> task)
        : base((TValue)default!)
    {
        _ = task ?? throw new ArgumentNullException(nameof(task));

        this.SetAwaiter(AsyncOperationResultAwaiter.Create(task, this.UpdateInternalState));
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
    /// Gets the operation awaiter.
    /// </summary>
    /// <returns>The operation awaiter.</returns>
    public new AsyncOperationResultAwaiter<TValue> GetAwaiter() => (AsyncOperationResultAwaiter<TValue>)base.GetAwaiter();

    /// <summary>
    /// Creates the task representing this operation result.
    /// </summary>
    /// <returns>
    /// The task representing this operation result.
    /// </returns>
    protected override Task CreateTask() => Task.FromResult(this.Value);
}

