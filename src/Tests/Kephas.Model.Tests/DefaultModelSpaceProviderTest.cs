// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultModelSpaceProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the default model space provider test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Model.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Injection;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Runtime.Configuration;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Runtime;
    using Kephas.Testing.Model;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DefaultModelSpaceProviderTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_Dimensions()
        {
            var contextFactory = this.CreateInjectableFactoryMock(() => new ModelConstructionContext(Substitute.For<IServiceProvider>()));
            var provider = new DefaultModelSpaceProvider(
                contextFactory,
                Array.Empty<IModelInfoProvider>(),
                this.GetRuntimeModelElementFactory());

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();

            Assert.IsTrue((modelSpace as IConstructibleElement)?.ConstructionState.IsCompleted);
        }

        [Test]
        public async Task InitializeAsync_using_injection()
        {
            var container = this.BuildServiceProvider(typeof(IModelSpace).Assembly);
            var provider = container.Resolve<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();

            Assert.IsTrue((modelSpace as IConstructibleElement)?.ConstructionState.IsCompleted);
        }

        protected override IEnumerable<Assembly> GetAssemblies()
        {
            var baseAssemblies = base.GetAssemblies().ToList();
            baseAssemblies.Add(typeof(IModelSpace).Assembly);
            return baseAssemblies;
        }

        private IRuntimeModelElementFactory GetRuntimeModelElementFactory()
        {
            return new DefaultRuntimeModelElementFactory(
                new List<IExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>>(),
                new List<IExportFactory<IRuntimeModelElementConfigurator, RuntimeModelElementConfiguratorMetadata>>(),
                new RuntimeTypeRegistry());
        }
    }
}