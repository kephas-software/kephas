// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeAssemblyInfoTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime assembly information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Runtime
{
    using System.Linq;

    using Kephas.Reflection;
    using Kephas.Runtime;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class RuntimeAssemblyInfoTest
    {
        [Test]
        public void Name()
        {
            var registry = new RuntimeTypeRegistry();
            var assemblyInfo = new RuntimeAssemblyInfo(registry, typeof(RuntimeAssemblyInfoTest).Assembly, null);
            Assert.AreEqual("Kephas.Core.Tests", assemblyInfo.Name);
        }

        [Test]
        public void Types()
        {
            var registry = new RuntimeTypeRegistry();
            var assemblyInfo = new RuntimeAssemblyInfo(registry, typeof(RuntimeAssemblyInfoTest).Assembly, null);
            Assert.IsTrue(assemblyInfo.Types.Cast<IRuntimeTypeInfo>().Any(t => t.Type == this.GetType()));
        }

        [Test]
        public void Types_declaring_container_set_and_identical()
        {
            var registry = new RuntimeTypeRegistry();
            var assemblyInfo = registry.GetAssemblyInfo(typeof(RuntimeAssemblyInfoTest).Assembly);
            Assert.IsTrue(assemblyInfo.Types.All(t => t.DeclaringContainer == assemblyInfo));
        }

        [Test]
        public void Types_custom_type_loader()
        {
            var typeLoader = Substitute.For<ITypeLoader>();
            typeLoader.GetExportedTypes(typeof(RuntimeAssemblyInfoTest).Assembly)
                .Returns(new[] { typeof(RuntimeAssemblyInfoTest) });
            var registry = new RuntimeTypeRegistry();
            var assemblyInfo = new RuntimeAssemblyInfo(registry, typeof(RuntimeAssemblyInfoTest).Assembly, typeLoader);

            var types = assemblyInfo.Types;
            Assert.AreEqual(1, types.Count());
            Assert.AreEqual("RuntimeAssemblyInfoTest", types.Single().Name);
        }

        [Test]
        public void Equals()
        {
            var registry = new RuntimeTypeRegistry();
            var assemblyInfo = new RuntimeAssemblyInfo(registry, typeof(RuntimeAssemblyInfoTest).Assembly, null);
            var assemblyInfo2 = new RuntimeAssemblyInfo(registry, typeof(RuntimeAssemblyInfoTest).Assembly, null);
            Assert.IsTrue(assemblyInfo.Equals(assemblyInfo2));
        }

        [Test]
        public void op_Equality()
        {
            var registry = new RuntimeTypeRegistry();
            var assemblyInfo = new RuntimeAssemblyInfo(registry, typeof(RuntimeAssemblyInfoTest).Assembly, null);
            var assemblyInfo2 = new RuntimeAssemblyInfo(registry, typeof(RuntimeAssemblyInfoTest).Assembly, null);
            Assert.IsTrue(assemblyInfo == assemblyInfo2);
        }

        [Test]
        public void op_Inequality()
        {
            var registry = new RuntimeTypeRegistry();
            var assemblyInfo = new RuntimeAssemblyInfo(registry, typeof(RuntimeAssemblyInfoTest).Assembly, null);
            var assemblyInfo2 = new RuntimeAssemblyInfo(registry, typeof(RuntimeAssemblyInfoTest).Assembly, null);
            Assert.IsFalse(assemblyInfo != assemblyInfo2);
        }
    }
}