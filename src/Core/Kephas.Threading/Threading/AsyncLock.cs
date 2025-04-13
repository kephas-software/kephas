// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLock.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading;

using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Class used for execution synchronization in asynchronous calls.
/// </summary>
public sealed class AsyncLock : IDisposable
{
    private readonly SemaphoreSlim semaphore = new(1, 1);

    /// <summary>
    /// Enters the lock and ensures that the action is executed within the lock.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <returns>An asynchronous result.</returns>
    public async Task EnterAsync(Func<Task> action)
    {
        action = action ?? throw new ArgumentNullException(nameof(action));

        await semaphore.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Enters the lock and ensures that the action is executed within the lock.
    /// </summary>
    /// <param name="action">The action to be executed.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>An asynchronous result.</returns>
    public async Task EnterAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        action = action ?? throw new ArgumentNullException(nameof(action));

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            await action(cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Enters the lock and ensures that the action is executed within the lock.
    /// </summary>
    /// <typeparam name="T">The asynchronous result type.</typeparam>
    /// <param name="action">The action to be executed.</param>
    /// <returns>An asynchronous result.</returns>
    public async Task<T> EnterAsync<T>(Func<Task<T>> action)
    {
        action = action ?? throw new ArgumentNullException(nameof(action));

        await semaphore.WaitAsync();
        try
        {
            return await action();
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Enters the lock and ensures that the action is executed within the lock.
    /// </summary>
    /// <typeparam name="T">The asynchronous result type.</typeparam>
    /// <param name="action">The action to be executed.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>An asynchronous result.</returns>
    public async Task<T> EnterAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        action = action ?? throw new ArgumentNullException(nameof(action));

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await action(cancellationToken);
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        semaphore.Dispose();
    }
}