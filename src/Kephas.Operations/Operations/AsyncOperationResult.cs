// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncOperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Threading.Tasks;

namespace Kephas.Operations;

using Kephas.Collections;

/// <summary>
/// An asynchronous operation result.
/// </summary>
public record AsyncOperationResult : OperationResult, IAsyncOperationResult
{
    private readonly Task task;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncOperationResult"/> class.
    /// </summary>
    /// <param name="task">The task.</param>
    public AsyncOperationResult(Task task)
    {
        this.task = task ?? throw new ArgumentNullException(nameof(task));
        task.ContinueWith(UpdateInternalState);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncOperationResult"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    protected AsyncOperationResult(object? value = null)
        : this(Task.FromResult(value))
    {
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
            var taskStatus = task.Status;
            return task.IsCompleted
                ? task.GetResult()
                : throw new InvalidOperationException(
                    $"The awaited task did not complete execution (state: {taskStatus}).");
        }

        set
        {
            throw new InvalidOperationException($"Cannot set the value of an {nameof(AsyncOperationResult)}.");
        }
    }

    /// <summary>
    /// Converts this object to a task.
    /// </summary>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    public Task AsTask() => task;

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
        : base(task)
    {
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
    public new Task<TValue> AsTask() => (Task<TValue>)base.AsTask();
}

