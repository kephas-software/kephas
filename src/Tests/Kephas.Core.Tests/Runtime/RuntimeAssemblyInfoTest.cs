// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeAssemblyInfoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime assembly information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime
{
    using System.Linq;

    using Kephas.Runtime;

    using NUnit.Framework;

    [TestFixture]
    public class RuntimeAssemblyInfoTest
    {
        [Test]
        public void Name()
        {
            var assemblyInfo = new RuntimeAssemblyInfo(typeof(RuntimeAssemblyInfoTest).Assembly);
            Assert.AreEqual("Kephas.Core.Tests", assemblyInfo.Name);
        }

        [Test]
        public void Types()
        {
            var assemblyInfo = new RuntimeAssemblyInfo(typeof(RuntimeAssemblyInfoTest).Assembly);
            Assert.IsTrue(assemblyInfo.Types.Cast<IRuntimeTypeInfo>().Any(t => t.Type == this.GetType()));
        }

        [Test]
        public void Types_declaring_container_set()
        {
            var assemblyInfo = RuntimeAssemblyInfo.GetRuntimeAssembly(typeof(RuntimeAssemblyInfoTest).Assembly);
            Assert.IsTrue(assemblyInfo.Types.All(t => t.DeclaringContainer == assemblyInfo));
        }
    }
}