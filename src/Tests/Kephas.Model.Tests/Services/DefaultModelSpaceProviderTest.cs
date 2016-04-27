namespace Kephas.Model.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Kephas.Composition;
    using Kephas.Model.Construction;
    using Kephas.Model.Runtime.Configuration;
    using Kephas.Model.Runtime.Configuration.Composition;
    using Kephas.Model.Runtime.Construction;
    using Kephas.Model.Runtime.Construction.Composition;
    using Kephas.Model.Services;

    using NUnit.Framework;

    [TestFixture]
    public class DefaultModelSpaceProviderTest
    {
        [Test]
        public async Task InitializeAsync_Dimensions()
        {
            var provider = new DefaultModelSpaceProvider(new IModelInfoProvider[0], this.GetRuntimeModelElementFactory());

            await provider.InitializeAsync();

            //...
        }

        private IRuntimeModelElementFactory GetRuntimeModelElementFactory()
        {
            return new DefaultRuntimeModelElementFactory(
                new List<IExportFactory<IRuntimeModelElementConstructor, RuntimeModelElementConstructorMetadata>>(),
                new List<IExportFactory<IRuntimeModelElementConfigurator, RuntimeModelElementConfiguratorMetadata>>());
        }
    }
}