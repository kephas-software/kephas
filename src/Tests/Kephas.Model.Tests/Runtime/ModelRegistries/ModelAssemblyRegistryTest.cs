// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelAssemblyRegistryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Composition;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.ModelRegistries;
    using Kephas.Reflection;
    using Kephas.Testing.Composition.Mef;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="ModelAssemblyRegistry"/>.
    /// </summary>
    [TestFixture]
    public class ModelAssemblyRegistryTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies, IEnumerable<Type> parts = null, Action<MefCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(ModelAssemblyRegistry).GetTypeInfo().Assembly); /* Kephas.Model */
            return base.CreateContainer(assemblyList, parts, config);
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
            var platformManager = Substitute.For<IAppRuntime>();
            platformManager.GetAppAssembliesAsync(Arg.Any<Func<AssemblyName, bool>>(), CancellationToken.None)
                .Returns(Task.FromResult((IEnumerable<Assembly>)new[] { typeof(ModelAssemblyRegistry).GetTypeInfo().Assembly }));

            var registry = new ModelAssemblyRegistry(platformManager, new DefaultTypeLoader(), new DefaultModelAssemblyAttributeProvider());
            var elements = await registry.GetRuntimeElementsAsync();
            var types = elements.OfType<Type>().ToList();
            Assert.IsTrue(types.All(t => t.Name.EndsWith("Dimension") || t.Name.EndsWith("DimensionElement")));
        }

        [Test]
        public async Task GetRuntimeElementsAsync_exclude_from_model()
        {
            var platformManager = Substitute.For<IAppRuntime>();
            platformManager.GetAppAssembliesAsync(Arg.Any<Func<AssemblyName, bool>>(), CancellationToken.None)
                .Returns(Task.FromResult((IEnumerable<Assembly>)new[] { typeof(ModelAssemblyRegistryTest).GetTypeInfo().Assembly }));

            var attrProvider = Substitute.For<IModelAssemblyAttributeProvider>();
            attrProvider.GetModelAssemblyAttributes(Arg.Any<Assembly>()).Returns(
                new List<ModelAssemblyAttribute> { new ModelAssemblyAttribute(typeof(ExcludedType)) });

            var registry = new ModelAssemblyRegistry(platformManager, new DefaultTypeLoader(), attrProvider);
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