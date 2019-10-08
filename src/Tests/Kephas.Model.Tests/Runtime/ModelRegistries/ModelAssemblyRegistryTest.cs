// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAssemblyRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="ModelAssemblyRegistry" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Runtime.ModelRegistries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Lite.Hosting;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.ModelRegistries;
    using Kephas.Reflection;
    using Kephas.Testing.Composition;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="ModelAssemblyRegistry"/>.
    /// </summary>
    [TestFixture]
    public class ModelAssemblyRegistryTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(
            IAmbientServices ambientServices = null,
            IEnumerable<Assembly> assemblies = null,
            IEnumerable<Type> parts = null,
            Action<LiteCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(ModelAssemblyRegistry).GetTypeInfo().Assembly); /* Kephas.Model */
            return base.CreateContainer(ambientServices, assemblyList, parts, config);
        }

        [Test]
        public void ModelAssemblyRegistry_Composition_success()
        {
            var container = this.CreateContainer();
            var registry = container.GetExports<IRuntimeModelRegistry>().OfType<ModelAssemblyRegistry>().SingleOrDefault();
            Assert.IsNotNull(registry);
        }

        [Test]
        public async Task GetRuntimeElementsAsync_from_Kephas_Model()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ModelAssemblyRegistry).GetTypeInfo().Assembly });

            var registry = new ModelAssemblyRegistry(appRuntime, new DefaultTypeLoader(), new DefaultModelAssemblyAttributeProvider());
            var elements = await registry.GetRuntimeElementsAsync();
            var types = elements.OfType<Type>().ToList();
            Assert.IsTrue(types.All(t => t.Name.EndsWith("Dimension") || t.Name.EndsWith("DimensionElement")));
        }

        [Test]
        public async Task GetRuntimeElementsAsync_exclude_from_model()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime.GetAppAssemblies(Arg.Any<Func<AssemblyName, bool>>())
                .Returns(new[] { typeof(ModelAssemblyRegistryTest).GetTypeInfo().Assembly });

            var attrProvider = Substitute.For<IModelAssemblyAttributeProvider>();
            attrProvider.GetModelAssemblyAttributes(Arg.Any<Assembly>()).Returns(
                new List<ModelAssemblyAttribute> { new ModelAssemblyAttribute(typeof(ExcludedType)) });

            var registry = new ModelAssemblyRegistry(appRuntime, new DefaultTypeLoader(), attrProvider);
            var elements = await registry.GetRuntimeElementsAsync();
            var types = elements.OfType<Type>().ToList();
            Assert.AreEqual(0, types.Count);
        }

        [ExcludeFromModel]
        public class ExcludedType
        {
        }
    }
}