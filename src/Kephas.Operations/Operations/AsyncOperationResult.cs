// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncOperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.Threading.Tasks;

namespace Kephas.Operations;

using Kephas.Collections;

/// <summary>
/// An asynchronous operation result.
/// </summary>
public record AsyncOperationResult<T> : OperationResult<T>, IAsyncOperationResult<T>
{
    private Task<T> task;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncOperationResult"/> class.
    /// </summary>
    /// <param name="task">The task.</param>
    public AsyncOperationResult(Task<T> task)
    {
        SetTask(task);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncOperationResult"/> class.
    /// </summary>
    /// <param name="value">The value.</param>
    protected AsyncOperationResult(T value = default!)
        : this(Task.FromResult(value))
    {
    }

    /// <summary>
    /// Gets or sets the return value.
    /// </summary>
    /// <value>
    /// The return value.
    /// </value>
    public override T Value
    {
        get
        {
            var taskStatus = task.Status;
            return task.IsCompleted
                ? task.Result
                : throw new InvalidOperationException(
                    $"The awaited task did not complete execution (state: {taskStatus}).");
        }

        set
        {
            var taskStatus = task.Status;
            if (!task.IsCompleted)
            {
                throw new InvalidOperationException($"Cannot set the value of an {nameof(AsyncOperationResult)} while the underlying task did not complete execution (state: {taskStatus}).");
            }

            SetTask(Task.FromResult(value));
        }
    }

    /// <summary>
    /// Converts this object to a task.
    /// </summary>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    public Task<T> AsTask() => task;

    /// <summary>
    /// Updates the internal state described by the provided task.
    /// </summary>
    /// <param name="t">A Task to process.</param>
    private void UpdateInternalState(Task t)
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

    [MemberNotNull(nameof(task))]
    internal void SetTask(Task<T> task)
    {
        this.task = task ?? throw new ArgumentNullException(nameof(task));
        task.ContinueWith(UpdateInternalState);
    }
}

/// <summary>
/// Encapsulates the result of an asynchronous operation.
/// </summary>
public record AsyncOperationResult : AsyncOperationResult<object?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AsyncOperationResult{TValue}"/> class.
    /// </summary>
    /// <param name="task">The task.</param>
    public AsyncOperationResult(Task task)
        : base(ToTaskOfObject(task))
    {
    }

    private static Task<object?> ToTaskOfObject(Task task) =>
        (task ?? throw new ArgumentNullException(nameof(task)))
        .ContinueWith(t => t.GetResult());
}

