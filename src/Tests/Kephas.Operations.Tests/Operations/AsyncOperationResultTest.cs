// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncOperationResultTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Operations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using NUnit.Framework;

    [TestFixture]
    public class AsyncOperationResultTest
    {
        [Test]
        public async Task AsyncOperationResult_task_completion_sets_state()
        {
            var opResult = new AsyncOperationResult(Task.FromResult((object)12));
            await opResult.AsTask();

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public async Task AsyncOperationResult_task_completion_sets_state_delay()
        {
            var opResult = new AsyncOperationResult(Task.Delay(100).ContinueWith(t => 12));
            await opResult.AsTask();

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public async Task AsyncOperationResult_task_completion_sets_state_exception()
        {
            var opResult = new AsyncOperationResult(Task.FromException(new ArgumentException("arg")));
            await opResult.AsTask().ContinueWith(t =>
            {
                Assert.AreEqual(OperationState.Failed, opResult.OperationState);
                Assert.AreEqual(0f, opResult.PercentCompleted);
                Assert.Throws<ArgumentException>(() => { var _ = opResult.Value; });
            });
        }

        [Test]
        public async Task AsyncOperationResult_task_completion_sets_state_canceled()
        {
            var opResult = new AsyncOperationResult(Task.FromCanceled(new CancellationToken(true)));
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
            var opResult = new AsyncOperationResult(Task.CompletedTask);
            var result = await opResult.AsTask();

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAwaiter_subsequent_result()
        {
            var opResult = new AsyncOperationResult(Task.CompletedTask);
            var result = await opResult.AsTask();
            Assert.IsNull(result);

            opResult.Value = 40;
            result = await opResult.AsTask();

            Assert.AreEqual(40, result);
        }

        [Test]
        public async Task GetAwaiter_task()
        {
            var opResult = new AsyncOperationResult(Task.FromResult((object)12));
            var result = await opResult.AsTask();

            Assert.AreEqual(12, result);
        }

        [Test]
        public async Task GetAwaiter_await()
        {
            var opResult = new AsyncOperationResult(Task.FromResult((object)12));
            var result = await opResult;

            Assert.AreEqual(12, result);
        }

        [Test]
        public async Task AsTask()
        {
            var opResult = new AsyncOperationResult(Task.FromResult((object)12));
            await opResult.AsTask();

            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public void Messages_Clear()
        {
            var opResult = new AsyncOperationResult(Task.CompletedTask).MergeException(new InvalidOperationException());

            CollectionAssert.IsNotEmpty(opResult.Messages);
            opResult.Messages.Clear();
            CollectionAssert.IsEmpty(opResult.Messages);
        }

        [Test]
        public void Deconstruct()
        {
            IOperationResult result = new AsyncOperationResult(Task.FromResult("hello")).Complete();
            var (value, state) = result;

            Assert.AreEqual("hello", value);
            Assert.AreEqual(OperationState.Completed, state);
        }
    }
}
