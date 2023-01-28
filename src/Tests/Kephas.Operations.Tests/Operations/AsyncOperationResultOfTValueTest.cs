// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncOperationResultOfTValueTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Operations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using NUnit.Framework;

    [TestFixture]
    public class AsyncOperationResultOfTValueTest
    {
        [Test]
        public async Task AsyncOperationResult_task_completion_sets_state()
        {
            var opResult = new AsyncOperationResult<int>(Task.FromResult(12));
            await opResult.AsTask();

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public async Task AsyncOperationResult_task_completion_sets_state_delay()
        {
            var opResult = new AsyncOperationResult<int>(Task.Delay(100).ContinueWith(t => 12));
            await opResult.AsTask();

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public async Task AsyncOperationResult_task_completion_sets_state_exception()
        {
            var opResult = new AsyncOperationResult<int>(Task.FromException<int>(new ArgumentException("arg")));
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
            var opResult = new AsyncOperationResult<int>(Task.FromCanceled<int>(new CancellationToken(true)));
            await opResult.AsTask().ContinueWith(t =>
            {
                Assert.AreEqual(OperationState.Canceled, opResult.OperationState);
                Assert.AreEqual(0f, opResult.PercentCompleted);
                Assert.Throws<TaskCanceledException>(() => { var _ = opResult.Value; });
            });
        }

        [Test]
        public async Task GetAwaiter_default_result()
        {
            var opResult = new AsyncOperationResult<int>(Task.FromResult(10));
            var result = await opResult.AsTask();

            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task GetAwaiter_subsequent_result()
        {
            var opResult = new AsyncOperationResult<int>(Task.FromResult(0));
            var result = await opResult.AsTask();
            Assert.AreEqual(0, result);

            opResult.Value = 40;
            result = await opResult.AsTask();

            Assert.AreEqual(40, result);
        }

        [Test]
        public async Task GetAwaiter_await()
        {
            var opResult = new AsyncOperationResult<int>(Task.FromResult(10));
            var result = await opResult;

            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task GetAwaiter_task()
        {
            var opResult = new AsyncOperationResult<int>(Task.FromResult(10));
            var result = await opResult.AsTask();

            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task AsTask()
        {
            var opResult = new AsyncOperationResult<int>(Task.FromResult(10));
            var result = await opResult.AsTask();

            Assert.AreEqual(10, result);
            Assert.AreEqual(10, opResult.Value);
        }

        [Test]
        public void Deconstruct()
        {
            IOperationResult<string> result = new AsyncOperationResult<string>(Task.FromResult("hello")).Complete();
            var (value, state) = result;

            Assert.AreEqual("hello", value);
            Assert.AreEqual(OperationState.Completed, state);
        }
    }
}