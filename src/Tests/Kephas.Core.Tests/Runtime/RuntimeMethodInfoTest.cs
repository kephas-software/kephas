// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeMethodInfoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime method information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime
{
    using System.Collections.Generic;

    using Kephas.Runtime;

    using NUnit.Framework;

    [TestFixture]
    public class RuntimeMethodInfoTest
    {
        [Test]
        public void ToString_name_parameters_and_type()
        {
            var methodInfo = new RuntimeMethodInfo(typeof(string).GetMethod(nameof(string.Copy)));

            var toString = methodInfo.ToString();
            Assert.AreEqual("Copy(str: System.String): System.String", toString);
        }

        [Test]
        public void IsStatic()
        {
            var methodInfo = new RuntimeMethodInfo(typeof(string).GetMethod(nameof(string.Copy)));

            Assert.IsTrue(methodInfo.IsStatic);
        }

        [Test]
        public void Invoke()
        {
            var methodInfo = new RuntimeMethodInfo(typeof(string).GetMethod(nameof(string.Copy)));
            var result = methodInfo.Invoke(null, new object[] { "test" });
            Assert.AreEqual("test", result);
        }
    }
}