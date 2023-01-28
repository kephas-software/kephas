// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncOperationResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

/// <summary>
/// Contract for asynchronous operation results.
/// </summary>
public interface IAsyncOperationResult : IOperationResult
{
    /// <summary>
    /// Converts this object to a task.
    /// </summary>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    Task<object?> AsTask();

    /// <summary>
    /// Gets the operation awaiter.
    /// </summary>
    /// <returns>The operation awaiter.</returns>
    AsyncOperationResultAwaiter GetAwaiter();
}

/// <summary>
/// Contract for asynchronous typed operation result.
/// </summary>
/// <typeparam name="TValue">Type of the return value.</typeparam>
public interface IAsyncOperationResult<TValue> : IAsyncOperationResult, IOperationResult<TValue>
{
    /// <summary>
    /// Converts this object to a task.
    /// </summary>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    new Task<TValue> AsTask();

    /// <summary>
    /// Gets the operation awaiter.
    /// </summary>
    /// <returns>The operation awaiter.</returns>
    new AsyncOperationResultAwaiter<TValue> GetAwaiter();

    /// <summary>
    /// Converts this object to a task.
    /// </summary>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    Task<object?> IAsyncOperationResult.AsTask()
    {
        var task = this.AsTask();
        return task as Task<object?> ?? task.ContinueWith(t => (object?)t.Result);
    }
}