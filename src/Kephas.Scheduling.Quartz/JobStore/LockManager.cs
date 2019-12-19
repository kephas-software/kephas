// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the lock manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Logging;
    using Kephas.Scheduling.Quartz.JobStore.Model;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Implements a simple distributed lock on top of MongoDB. It is not a reentrant lock so you can't 
    /// acquire the lock more than once in the same thread of execution.
    /// </summary>
    internal class LockManager : Loggable, IDisposable
    {
        private readonly string instanceName;

        private static readonly TimeSpan SleepThreshold = TimeSpan.FromMilliseconds(1000);

        private readonly ConcurrentDictionary<LockType, LockInstance> pendingLocks =
            new ConcurrentDictionary<LockType, LockInstance>();

        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockManager"/> class.
        /// </summary>
        /// <param name="instanceName">Name of the instance.</param>
        public LockManager(string instanceName)
        {
            this.instanceName = instanceName;
        }

        public void Dispose()
        {
            EnsureObjectNotDisposed();

            this.disposed = true;
            var locks = this.pendingLocks.ToArray();
            foreach (var keyValuePair in locks)
            {
                keyValuePair.Value.Dispose();
            }
        }

        public async Task<IDisposable> AcquireLock(
            IDataContext dataContext,
            LockType lockType,
            string instanceId,
            CancellationToken cancellationToken = default)
        {
            while (true)
            {
                this.EnsureObjectNotDisposed();
                if (await dataContext.TryAcquireLock(this.instanceName, lockType, instanceId, cancellationToken: cancellationToken))
                {
                    var lockInstance = new LockInstance(this, dataContext, lockType, instanceId);
                    this.AddLock(lockInstance);
                    return lockInstance;
                }

                await Task.Delay(SleepThreshold, cancellationToken).PreserveThreadContext();
            }
        }

        private void EnsureObjectNotDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(LockManager));
            }
        }

        private void AddLock(LockInstance lockInstance)
        {
            if (!this.pendingLocks.TryAdd(lockInstance.LockType, lockInstance))
            {
                throw new Exception($"Unable to add lock instance for lock {lockInstance.LockType} on {lockInstance.InstanceId}");
            }
        }

        private void LockReleased(LockInstance lockInstance)
        {
            if (!this.pendingLocks.TryRemove(lockInstance.LockType, out _))
            {
                this.Logger.Warn("Unable to remove pending lock {lockType} on {lockInstanceId}", lockInstance.LockType, lockInstance.InstanceId);
            }
        }

        private class LockInstance : IDisposable
        {
            private readonly LockManager lockManager;

            private readonly IDataContext dataContext;

            private bool disposed;

            public LockInstance(LockManager lockManager, IDataContext dataContext, LockType lockType, string instanceId)
            {
                this.lockManager = lockManager;
                this.dataContext = dataContext;
                this.LockType = lockType;
                this.InstanceId = instanceId;
            }

            public string InstanceId { get; }

            public LockType LockType { get; }

            public void Dispose()
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException(
                        nameof(LockInstance),
                        $"This lock {this.LockType} for {this.InstanceId} has already been disposed");
                }

                this.dataContext.ReleaseLock(this.lockManager.instanceName, this.LockType, this.InstanceId).WaitNonLocking();
                this.lockManager.LockReleased(this);
                this.disposed = true;
            }
        }
    }
}