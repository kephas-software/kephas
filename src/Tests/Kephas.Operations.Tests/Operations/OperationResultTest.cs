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
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Operations;
    using NUnit.Framework;

    [TestFixture]
    public class OperationResultTest
    {
        [Test]
        public void OperationResult_complete()
        {
            var opResult = new OperationResult(12).Complete();

            Assert.AreEqual(OperationState.Completed, opResult.OperationState);
            Assert.AreEqual(1f, opResult.PercentCompleted);
            Assert.AreEqual(12, opResult.Value);
        }

        [Test]
        public void OperationResult_complete_exception()
        {
            var opResult = new ArgumentException("arg").ToOperationResult<object?>();
            Assert.AreEqual(OperationState.Failed, opResult.OperationState);
            Assert.AreEqual(0f, opResult.PercentCompleted);
            Assert.Throws<ArgumentException>(() => { var _ = opResult.Value; });
        }

        [Test]
        public void Messages_Clear()
        {
            var opResult = new OperationResult().MergeException(new InvalidOperationException());

            CollectionAssert.IsNotEmpty(opResult.Messages);
            opResult.Messages.Clear();
            CollectionAssert.IsEmpty(opResult.Messages);
        }

        [Test]
        public void Deconstruct()
        {
            IOperationResult result = new OperationResult("hello").Complete();
            var (value, state) = result;

            Assert.AreEqual("hello", value);
            Assert.AreEqual(OperationState.Completed, state);
        }
    }
}
