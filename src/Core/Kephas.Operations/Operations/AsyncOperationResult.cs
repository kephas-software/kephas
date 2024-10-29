// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncOperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using Kephas.Collections;
using Kephas.Threading.Tasks;

namespace Kephas.Operations;

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

    private T UpdateInternalState(Task<T> t, bool throwOnError)
    {
        Reset();

        var opState = t.IsFaulted
            ? OperationStatus.Failed
            : t.IsCanceled
                ? OperationStatus.Canceled
                : t.Status == TaskStatus.RanToCompletion
                    ? OperationStatus.Completed
                    : t.Status == TaskStatus.Running
                        ? OperationStatus.InProgress
                        : OperationStatus.None;

        OperationStatus = opState;
        var value = default(T);
        switch (opState)
        {
            case OperationStatus.Failed:
                t.Exception!.InnerExceptions.ForEach(ex => this.MergeException(ex));
                if (throwOnError)
                {
                    throw t.Exception?.InnerException ?? new OperationFailedException("Operation failed.");
                }
                break;
            case OperationStatus.Canceled:
                this.MergeException(new TaskCanceledException());
                if (throwOnError)
                {
                    throw new TaskCanceledException();
                }
                break;
            case OperationStatus.Completed:
                PercentCompleted = 1f;
                value = t.Result;
                break;
        }

        return value!;
    }

    private void Reset()
    {
        PercentCompleted = 0f;
        OperationStatus = OperationStatus.None;
        EndedAt = null;
        StartedAt = DateTimeOffset.Now;
        Messages.Clear();
    }

    [MemberNotNull(nameof(task))]
    private void SetTask(Task<T> task)
    {
        ArgumentNullException.ThrowIfNull(task);
        if (task == this.task) return;

        if (task.IsCompleted)
        {
            this.task = task;
            UpdateInternalState(task, throwOnError: false);
            return;
        }

        this.task = task.ContinueWith(t => UpdateInternalState(t, throwOnError: true));
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
        : base(GetTaskOfObject(task))
    {
    }

    private static Task<object?> GetTaskOfObject(Task task)
    {
        ArgumentNullException.ThrowIfNull(task);

        return task is Task<object?> taskOfObject
            ? taskOfObject
            : task.IsCompletedSuccessfully
                ? Task.FromResult(task.GetResult())
                : task.IsCanceled
                    ? Task.FromCanceled<object?>(new CancellationToken(true))
                    : task.IsFaulted
                        ? Task.FromException<object?>(task.Exception!.InnerException!)
                        : task.ContinueWith(t => t.GetResult());
    }
}

