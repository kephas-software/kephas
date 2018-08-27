namespace Kephas.Model.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Model.Construction;
    using Kephas.Model.Construction.Internal;
    using Kephas.Model.Runtime.Configuration;
    using Kephas.Model.Runtime.Configuration.Composition;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Composition;
    using Kephas.Model.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultModelSpaceProviderTest : ModelTestBase
    {
        [Test]
        public async Task InitializeAsync_Dimensions()
        {
            var provider = new DefaultModelSpaceProvider(
                Substitute.For<ICompositionContext>(),
                new IModelInfoProvider[0],
                this.GetRuntimeModelElementFactory());

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();

            Assert.IsTrue((modelSpace as IConstructibleElement)?.ConstructionState.IsCompleted);
        }

        [Test]
        public async Task InitializeAsync_using_composition()
        {
            var container = this.CreateContainer(typeof(IModelSpace).GetTypeInfo().Assembly);
            var provider = container.GetExport<IModelSpaceProvider>();

            await provider.InitializeAsync();

            var modelSpace = provider.GetModelSpace();

            Assert.IsTrue((modelSpace as IConstructibleElement)?.ConstructionState.IsCompleted);
        }

        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var baseAssemblies = base.GetDefaultConventionAssemblies().ToList();
            baseAssemblies.Add(typeof(IModelSpace).GetTypeInfo().Assembly);
            return baseAssemblies;
        }

        private IRuntimeModelElementFactory GetRuntimeModelElementFactory()
        {
            return new DefaultRuntimeModelElementFactory(
                new List<IExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>>(),
                new List<IExportFactory<IRuntimeModelElementConfigurator, RuntimeModelElementConfiguratorMetadata>>());
        }
    }
}