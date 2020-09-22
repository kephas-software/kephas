// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationMessageTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data i/o message test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Operations
{
    using Kephas.Operations;

    using NUnit.Framework;

    [TestFixture]
    public class OperationMessageTest
    {
        [Test]
        public void ToString_contains_message()
        {
            var msg = new OperationMessage("hello");
            var expected = $"[{msg.Timestamp:s}] hello";
            Assert.AreEqual(expected, msg.ToString());
        }
    }
}