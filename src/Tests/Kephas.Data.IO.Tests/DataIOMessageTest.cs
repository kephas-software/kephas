// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOMessageTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data i/o message test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class DataIOMessageTest
    {
        [Test]
        public void ToString_contains_message()
        {
            var msg = new DataIOMessage("hello");
            var expected = $"{msg.Timestamp:s} hello";
            Assert.AreEqual(expected, msg.ToString());
        }
    }
}