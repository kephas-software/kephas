// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOExceptionTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
            var expected = $"{exception.Timestamp:s} {exception.GetType()}: hello";
            Assert.AreEqual(expected, exception.ToString());
        }
    }
}