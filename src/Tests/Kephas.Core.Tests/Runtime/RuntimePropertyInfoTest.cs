// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimePropertyInfoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the runtime property information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime
{
    using Kephas.Runtime;

    using NUnit.Framework;

    [TestFixture]
    public class RuntimePropertyInfoTest
    {
        [Test]
        public void ToString()
        {
            var propInfo = new RuntimePropertyInfo<string, int>(typeof(string).GetProperty("Length"));

            var toString = propInfo.ToString();
            Assert.AreEqual("Length: System.Int32", toString);
        }
    }
}