// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationResultTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the operation result test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Operations
{
    using System.Threading.Tasks;

    using Kephas.Operations;
    using NUnit.Framework;

    [TestFixture]
    public class OperationResultTest
    {
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

            opResult.ReturnValue = 40;
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

            Assert.AreEqual(12, opResult.ReturnValue);
        }
    }

    [TestFixture]
    public class OperationResultOfTValueTest
    {
        [Test]
        public async Task GetAwaiter_default_result()
        {
            var opResult = new OperationResult<int>(10);
            var result = await opResult;

            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task GetAwaiter_subsequent_result()
        {
            var opResult = new OperationResult<int>();
            var result = await opResult;
            Assert.AreEqual(0, result);

            opResult.ReturnValue = 40;
            result = await opResult;

            Assert.AreEqual(40, result);
        }

        [Test]
        public async Task GetAwaiter_task()
        {
            var opResult = new OperationResult<int>(Task.FromResult(10));
            var result = await opResult;

            Assert.AreEqual(10, result);
        }

        [Test]
        public async Task AsTask()
        {
            var opResult = new OperationResult<int>(Task.FromResult(10));
            var result = await opResult.AsTask();

            Assert.AreEqual(10, result);
            Assert.AreEqual(10, opResult.ReturnValue);
        }
    }
}
