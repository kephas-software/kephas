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
    using NUnit.Framework;

    [TestFixture]
    public class OperationResultOfTValueTest
    {
        [Test]
        public void OperationResult_complete()
        {
            var opResult = new OperationResult<int>(12).Complete();

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public void OperationResult_complete_exception()
        {
            var opResult = new ArgumentException("arg").ToOperationResult<int>();
            Assert.AreEqual(OperationState.Failed, opResult.OperationState);
            Assert.AreEqual(0f, opResult.PercentCompleted);
            Assert.Throws<ArgumentException>(() => { var _ = opResult.Value; });
        }

        [Test]
        public void Value_setter()
        {
            var opResult = new OperationResult<int>(10);
            var result = opResult.Value;

            Assert.AreEqual(10, result);
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