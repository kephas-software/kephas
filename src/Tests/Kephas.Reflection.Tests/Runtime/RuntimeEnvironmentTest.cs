// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeEnvironmentTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime environment test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Runtime
{
    using Kephas.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class RuntimeEnvironmentTest
    {
        [Test]
        public void IsNetFramework()
        {
            Assert.IsFalse(RuntimeEnvironment.IsNetFramework);
        }

        [Test]
        public void GetAppFramework()
        {
            var appFramework = RuntimeEnvironment.GetAppFrameworkMoniker();

#if NETCOREAPP3_1
            Assert.AreEqual("netcoreapp3.1", appFramework);
#elif NET6_0
            Assert.AreEqual("net6.0", appFramework);
#else
            Assert.IsTrue(appFramework.StartsWith("net"), "Expected a .NET Core app framework.");
#endif
        }

#if NETCOREAPP3_1
        [Test]
        public void IsNetCore()
        {
            Assert.IsTrue(RuntimeEnvironment.IsNetCore);
        }

        [Test]
        public void IsNet()
        {
            Assert.IsFalse(RuntimeEnvironment.IsNet);
        }

        [Test]
        public void FrameworkName()
        {
            Assert.AreEqual(RuntimeEnvironment.NetCoreRuntime, RuntimeEnvironment.FrameworkName);
        }
#else
        [Test]
        public void IsNetCore()
        {
            Assert.IsFalse(RuntimeEnvironment.IsNetCore);
        }

        [Test]
        public void IsNet()
        {
            Assert.IsTrue(RuntimeEnvironment.IsNet);
        }

        [Test]
        public void FrameworkName()
        {
            Assert.AreEqual(RuntimeEnvironment.NetRuntime, RuntimeEnvironment.FrameworkName);
        }
#endif

        [Test]
        public void IsNetNative()
        {
            Assert.IsFalse(RuntimeEnvironment.IsNetNative);
        }
    }
}
