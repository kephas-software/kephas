// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeEnvironmentTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime environment test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime
{
    using Kephas.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class RuntimeEnvironmentTest
    {
#if NET461
#else
        [Test]
        public void IsNetFull()
        {
            Assert.IsFalse(RuntimeEnvironment.IsNetFull);
        }

        [Test]
        public void IsNetCore()
        {
            Assert.IsTrue(RuntimeEnvironment.IsNetCore);
        }

        [Test]
        public void IsNetNative()
        {
            Assert.IsFalse(RuntimeEnvironment.IsNetNative);
        }
#endif
    }
}
