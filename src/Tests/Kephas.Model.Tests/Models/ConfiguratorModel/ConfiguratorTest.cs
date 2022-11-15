// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfiguratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the configurator test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests.Models.ConfiguratorModel
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Model.Runtime.Configuration;
    using Kephas.Reflection;
    using Kephas.Testing.Model;
    using NUnit.Framework;

    public class ConfiguratorTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_named_configurator()
        {
            var container = this.CreateInjectorForModel(new[] { typeof(NamedConfigurator) }, new[] { typeof(INamed) });
            var provider = container.Resolve<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var namedClassifier = modelSpace.Classifiers.Single(c => c.Name == "Named");

            var nameProperty = namedClassifier.Properties.FirstOrDefault(p => p.Name == nameof(INamed.Name));
            Assert.NotNull(nameProperty);

            Assert.AreEqual(1, nameProperty.Annotations.Count());

            //...
        }

        [Test]
        public async Task InitializeAsync_named_configurator_does_not_interfere_with_other()
        {
            var container = this.CreateInjectorForModel(new[] { typeof(NamedConfigurator) }, new[] { typeof(INamed), typeof(IOtherNamed) });
            var provider = container.Resolve<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();
            var otherNamedClassifier = modelSpace.Classifiers.Single(c => c.Name == "OtherNamed");

            var nameProperty = otherNamedClassifier.Properties.FirstOrDefault(p => p.Name == nameof(IOtherNamed.Name));
            Assert.NotNull(nameProperty);

            Assert.AreEqual(0, nameProperty.Annotations.Count());

            //...
        }

        [Test]
        public void InitializeAsync_derived_named_configurator_does_not_override_base_named()
        {
            var container = this.CreateInjectorForModel(new[] { typeof(DerivedNamedConfigurator) }, new[] { typeof(INamed), typeof(IDerivedNamed) });
            var provider = container.Resolve<IModelSpaceProvider>();

            Assert.ThrowsAsync<ModelConfigurationException>(() => provider.InitializeAsync());
        }
    }
}