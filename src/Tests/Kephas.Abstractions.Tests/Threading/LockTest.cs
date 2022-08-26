// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LockTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class LockTest
    {
        [Test]
        public async Task LockAsync_task_single()
        {
            var sync = new Dictionary<int, object>();
            using var l = new Lock();
            await l.EnterAsync(() => this.ForTaskAsync(sync));
        }

        [Test]
        public async Task LockAsync_task_multiple()
        {
            var sync = new Dictionary<int, object>();
            using var l = new Lock();
            await Task.WhenAll(
                l.EnterAsync(() => this.ForTaskAsync(sync)),
                l.EnterAsync(() => this.ForTaskAsync(sync)),
                l.EnterAsync(() => this.ForTaskAsync(sync)));
        }

        [Test]
        public async Task LockAsync_task_single_token()
        {
            var sync = new Dictionary<int, object>();
            using var l = new Lock();
            await l.EnterAsync(ct => this.ForTaskAsync(sync));
        }

        [Test]
        public async Task LockAsync_task_multiple_token()
        {
            var sync = new Dictionary<int, object>();
            using var l = new Lock();
            await Task.WhenAll(
                l.EnterAsync(ct => this.ForTaskAsync(sync)),
                l.EnterAsync(ct => this.ForTaskAsync(sync)),
                l.EnterAsync(ct => this.ForTaskAsync(sync)));
        }

        [Test]
        public async Task LockAsync_task_of_int_single()
        {
            var sync = new Dictionary<int, object>();
            using var l = new Lock();
            await l.EnterAsync(() => this.ForTaskOfIntAsync(sync));
        }

        [Test]
        public async Task LockAsync_task_of_int_multiple()
        {
            var sync = new Dictionary<int, object>();
            using var l = new Lock();
            await Task.WhenAll(
                l.EnterAsync(() => this.ForTaskOfIntAsync(sync)),
                l.EnterAsync(() => this.ForTaskOfIntAsync(sync)),
                l.EnterAsync(() => this.ForTaskOfIntAsync(sync)));
        }

        [Test]
        public async Task LockAsync_task_of_int_single_token()
        {
            var sync = new Dictionary<int, object>();
            using var l = new Lock();
            await l.EnterAsync(ct => this.ForTaskOfIntAsync(sync));
        }

        [Test]
        public async Task LockAsync_task_of_int_multiple_token()
        {
            var sync = new Dictionary<int, object>();
            using var l = new Lock();
            await Task.WhenAll(
                l.EnterAsync(ct => this.ForTaskOfIntAsync(sync)),
                l.EnterAsync(ct => this.ForTaskOfIntAsync(sync)),
                l.EnterAsync(ct => this.ForTaskOfIntAsync(sync)));
        }

        private async Task<int> ForTaskOfIntAsync(IDictionary<int, object> sync)
        {
            await this.ForTaskAsync(sync);
            return 0;
        }

        private async Task ForTaskAsync(IDictionary<int, object> sync)
        {
            await Task.Yield();

            if (sync.Count > 0)
            {
                throw new InvalidOperationException();
            }

            var key = Thread.CurrentThread.ManagedThreadId;
            sync[key] = "whatever";
            await Task.Delay(50).ConfigureAwait(false);
            sync.Remove(key);
        }
    }
}