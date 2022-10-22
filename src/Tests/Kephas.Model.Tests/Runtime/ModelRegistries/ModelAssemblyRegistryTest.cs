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
    using Kephas.Logging;
    using Kephas.Model.AttributedModel;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.ModelRegistries;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Testing;
    using Kephas.Testing.Services;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="ModelAssemblyRegistry"/>.
    /// </summary>
    [TestFixture]
    public class ModelAssemblyRegistryTest : TestBase
    {
        public override IServiceProvider BuildServiceProvider(
            IAmbientServices? ambientServices = null,
            IEnumerable<Assembly>? assemblies = null,
            IEnumerable<Type>? parts = null,
            Action<IInjectorBuilder>? config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(ModelAssemblyRegistry).Assembly); /* Kephas.Model */
            return base.BuildServiceProvider(ambientServices, assemblyList, parts, config);
        }

        [Test]
        public void ModelAssemblyRegistry_Injection_success()
        {
            var container = this.BuildServiceProvider();
            var registry = container.ResolveMany<IRuntimeModelRegistry>().OfType<ModelAssemblyRegistry>().SingleOrDefault();
            Assert.IsNotNull(registry);
        }

        [Test]
        public async Task GetRuntimeElementsAsync_from_Kephas_Model()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssemblies()
                .Returns(new[] { typeof(ModelAssemblyRegistry).Assembly });

            var registry = new ModelAssemblyRegistry(appRuntime, new DefaultTypeLoader(null), new DefaultModelAssemblyAttributeProvider(), new RuntimeTypeRegistry());
            var elements = await registry.GetRuntimeElementsAsync();
            var types = elements.OfType<Type>().ToList();
            Assert.IsTrue(types.All(t => t.Name.EndsWith("Dimension") || t.Name.EndsWith("DimensionElement")));
        }

        [Test]
        public async Task GetRuntimeElementsAsync_exclude_from_model()
        {
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime
                .GetAppAssemblies()
                .Returns(new[] { typeof(ModelAssemblyRegistryTest).Assembly });

            var attrProvider = Substitute.For<IModelAssemblyAttributeProvider>();
            attrProvider.GetModelAssemblyAttributes(Arg.Any<Assembly>()).Returns(
                new List<ModelAssemblyAttribute> { new ModelAssemblyAttribute(typeof(ExcludedType)) });

            var registry = new ModelAssemblyRegistry(appRuntime, new DefaultTypeLoader(null), attrProvider, new RuntimeTypeRegistry());
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