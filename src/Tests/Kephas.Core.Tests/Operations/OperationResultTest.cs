// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Threading.Tasks;

namespace Kephas.Core.Tests.Operations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using NUnit.Framework;

    [TestFixture]
    public class OperationResultTest
    {
        [Test]
        public async Task OperationResult_task_completion_sets_state()
        {
            var opResult = new OperationResult(Task.FromResult((object)12));
            await opResult;

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public async Task OperationResult_task_completion_sets_state_delay()
        {
            var opResult = new OperationResult(Task.Delay(100).ContinueWith(t => 12));
            await opResult;

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public async Task OperationResult_task_completion_sets_state_exception()
        {
            var opResult = new OperationResult(Task.FromException(new ArgumentException("arg")));
            await opResult.AsTask().ContinueWith(t =>
            {
                Assert.AreEqual(OperationState.Failed, opResult.OperationState);
                Assert.AreEqual(0f, opResult.PercentCompleted);
                Assert.Throws<ArgumentException>(() => { var _ = opResult.Value; });
            });
        }

        [Test]
        public async Task OperationResult_task_completion_sets_state_canceled()
        {
            var opResult = new OperationResult(Task.FromCanceled(new CancellationToken(true)));
            await opResult.AsTask().ContinueWith(t =>
            {
                Assert.AreEqual(OperationState.Canceled, opResult.OperationState);
                Assert.AreEqual(0f, opResult.PercentCompleted);
                Assert.Throws<TaskCanceledException>(() => { var _ = opResult.Value; });
            });
        }

        [Test]
        public async Task GetAwaiter_no_result()
        {
            var opResult = new OperationResult();
            var result = await opResult;

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAwaiter_subsequent_result()
        {
            var opResult = new OperationResult();
            var result = await opResult;
            Assert.IsNull(result);

            opResult.Value = 40;
            result = await opResult;

            Assert.AreEqual(40, result);
        }

        [Test]
        public async Task GetAwaiter_task()
        {
            var opResult = new OperationResult(Task.FromResult((object)12));
            var result = await opResult;

            Assert.AreEqual(12, result);
        }

        [Test]
        public async Task AsTask()
        {
            var opResult = new OperationResult(Task.FromResult((object)12));
            await opResult.AsTask();

            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public void Exceptions_Clear()
        {
            var opResult = new OperationResult().MergeException(new InvalidOperationException());

            CollectionAssert.IsNotEmpty(opResult.Exceptions);
            opResult.Exceptions.Clear();
            CollectionAssert.IsEmpty(opResult.Exceptions);
        }
    }
}
