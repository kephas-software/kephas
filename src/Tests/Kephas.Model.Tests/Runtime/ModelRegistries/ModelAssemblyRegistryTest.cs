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
        /// <summary>
        /// Gets the default convention types to be considered when building the container. By default it includes Kephas.Core.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the default convention types in
        /// this collection.
        /// </returns>
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(ModelAssemblyRegistry).Assembly, /* Kephas.Model */
            };
        }

        [Test]
        public void ModelAssemblyRegistry_Injection_success()
        {
            var container = this.CreateServicesBuilder()
                .BuildWithDependencyInjection();
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