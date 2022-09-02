// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultOfTValueTest.cs" company="Kephas Software SRL">
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
    using Kephas.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class OperationResultOfTValueTest
    {
        [Test]
        public async Task OperationResult_task_completion_sets_state()
        {
            var opResult = new OperationResult<int>(Task.FromResult(12));
            await opResult.AsTask();

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public async Task OperationResult_task_completion_sets_state_delay()
        {
            var opResult = new OperationResult<int>(Task.Delay(100).ContinueWith(t => 12));
            await opResult.AsTask();

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public async Task OperationResult_task_completion_sets_state_exception()
        {
            var opResult = new OperationResult<int>(Task.FromException<int>(new ArgumentException("arg")));
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
            var opResult = new OperationResult<int>(Task.FromCanceled<int>(new CancellationToken(true)));
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
            var opResult = new OperationResult<int>(10);
            var result = await opResult.AsTask();

            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task GetAwaiter_subsequent_result()
        {
            var opResult = new OperationResult<int>();
            var result = await opResult.AsTask();
            Assert.AreEqual(0, result);

            opResult.Value = 40;
            result = await opResult.AsTask();

            Assert.AreEqual(40, result);
        }

        [Test]
        public async Task GetAwaiter_task()
        {
            var opResult = new OperationResult<int>(Task.FromResult(10));
            var result = await opResult.AsTask();

            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task AsTask()
        {
            var opResult = new OperationResult<int>(Task.FromResult(10));
            var result = await opResult.AsTask();

            Assert.AreEqual(10, result);
            Assert.AreEqual(10, opResult.Value);
        }

        [Test]
        public void Deconstruct()
        {
            IOperationResult<string> result = new OperationResult<string>("hello").Complete();
            var (value, state) = result;

            Assert.AreEqual("hello", value);
            Assert.AreEqual(OperationState.Completed, state);
        }
    }
}