// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Threading
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Helper class for locking.
    /// </summary>
    public static class LockHelper
    {
        /// <summary>
        /// Locks the <paramref name="syncObject"/> while the provided action is still executing.
        /// </summary>
        /// <param name="syncObject">The synchronization object.</param>
        /// <param name="action">The action to be executed.</param>
        /// <returns>An asynchronous result.</returns>
        public static async Task LockAsync(this object syncObject, Func<Task> action)
        {
            Requires.NotNull(syncObject, nameof(syncObject));
            Requires.NotNull(action, nameof(action));

            var lockTaken = false;
            try
            {
                Monitor.Enter(syncObject, ref lockTaken);
                await action().PreserveThreadContext();
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(syncObject);
                }
            }
        }

        /// <summary>
        /// Locks the <paramref name="syncObject"/> while the provided action is still executing.
        /// </summary>
        /// <param name="syncObject">The synchronization object.</param>
        /// <param name="action">The action to be executed.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>An asynchronous result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task LockAsync(this object syncObject, Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
        {
            return LockAsync(syncObject, () => action?.Invoke(cancellationToken) ?? throw new ArgumentNullException(nameof(action)));
        }

        /// <summary>
        /// Locks the <paramref name="syncObject"/> while the provided action is still executing.
        /// </summary>
        /// <typeparam name="T">The asynchronous result type.</typeparam>
        /// <param name="syncObject">The synchronization object.</param>
        /// <param name="action">The action to be executed.</param>
        /// <returns>An asynchronous result.</returns>
        public static async Task<T> LockAsync<T>(this object syncObject, Func<Task<T>> action)
        {
            Requires.NotNull(syncObject, nameof(syncObject));
            Requires.NotNull(action, nameof(action));

            var lockTaken = false;
            try
            {
                Monitor.Enter(syncObject, ref lockTaken);
                return await action().PreserveThreadContext();
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(syncObject);
                }
            }
        }


        /// <summary>
        /// Locks the <paramref name="syncObject"/> while the provided action is still executing.
        /// </summary>
        /// <typeparam name="T">The asynchronous result type.</typeparam>
        /// <param name="syncObject">The synchronization object.</param>
        /// <param name="action">The action to be executed.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>An asynchronous result.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<T> LockAsync<T>(this object syncObject, Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
        {
            return LockAsync(syncObject, () => action?.Invoke(cancellationToken) ?? throw new ArgumentNullException(nameof(action)));
        }
    }
}