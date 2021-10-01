// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Lock.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Class used for execution synchronization.
    /// </summary>
    public sealed class Lock : IDisposable
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Enters the lock and ensures that the action is executed within the lock.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        /// <returns>An asynchronous result.</returns>
        public async Task EnterAsync(Func<Task> action)
        {
            Requires.NotNull(action, nameof(action));

            await this.semaphore.WaitAsync().PreserveThreadContext();
            try
            {
                await action().PreserveThreadContext();
            }
            finally
            {
                this.semaphore.Release();
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
            Requires.NotNull(action, nameof(action));

            await this.semaphore.WaitAsync(cancellationToken).PreserveThreadContext();
            try
            {
                await action(cancellationToken).PreserveThreadContext();
            }
            finally
            {
                this.semaphore.Release();
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
            Requires.NotNull(action, nameof(action));

            await this.semaphore.WaitAsync().PreserveThreadContext();
            try
            {
                return await action().PreserveThreadContext();
            }
            finally
            {
                this.semaphore.Release();
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
            Requires.NotNull(action, nameof(action));

            await this.semaphore.WaitAsync(cancellationToken).PreserveThreadContext();
            try
            {
                return await action(cancellationToken).PreserveThreadContext();
            }
            finally
            {
                this.semaphore.Release();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.semaphore.Dispose();
        }
    }
}