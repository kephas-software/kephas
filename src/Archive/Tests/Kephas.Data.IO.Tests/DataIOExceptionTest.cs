// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOExceptionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data i/o exception test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class DataIOExceptionTest
    {
        [Test]
        public void ToString_contains_message()
        {
            var exception = new DataIOException("hello");
            var expected = $"[{exception.Timestamp:s} {exception.Severity}] {exception.GetType()}: hello";
            Assert.AreEqual(expected, exception.ToString());
        }
    }
}