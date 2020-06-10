// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeTypeRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime
{
    using System.Linq;

    using Kephas.Reflection;
    using Kephas.Runtime;
    using NUnit.Framework;

    [TestFixture]
    public class RuntimeTypeRegistryTest
    {
        [Test]
        public void GetRuntimeType_non_generic()
        {
            var registry = new RuntimeTypeRegistry();
            var typeInfo = registry.GetTypeInfo(typeof(string));

            Assert.IsFalse(typeInfo.IsGenericType());
        }

        [Test]
        public void Types_declaring_container_set()
        {
            var registry = new RuntimeTypeRegistry();
            var assemblyInfo = registry.GetAssemblyInfo(typeof(RuntimeAssemblyInfoTest).Assembly);
            Assert.IsTrue(assemblyInfo.Types.All(t => t.DeclaringContainer == assemblyInfo));
        }
    }
}