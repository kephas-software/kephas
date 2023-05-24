// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesModelRegistryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application services serviceRegistry test class.
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
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.ModelRegistries;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using Kephas.Services.Reflection;
    using Kephas.Testing;
    using Kephas.Testing.Services;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AppServicesModelRegistryTest : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly>(base.GetAssemblies())
            {
                typeof(AppServicesModelRegistry).Assembly, /* Kephas.Model */
            };
        }

        [Test]
        public void AppServicesRegistry_Injection_success()
        {
            var container = this.CreateServicesBuilder()
                .BuildWithDependencyInjection();
            var registry = container.ResolveMany<IRuntimeModelRegistry>().OfType<AppServicesModelRegistry>().SingleOrDefault();
            Assert.IsNotNull(registry);
        }

        [Test]
        public async Task GetRuntimeElementsAsync_from_Kephas_Model()
        {
            IAppServiceCollection appServices = this.CreateAppServices();
            var appServicesInfos = new List<ContractDeclaration>
            {
                   new (typeof(int), Substitute.For<IAppServiceInfo>()),
                   new (typeof(string), Substitute.For<IAppServiceInfo>()),
                   new (typeof(bool), Substitute.For<IAppServiceInfo>()),
            };

            var registry = new AppServicesModelRegistry(appServices, appServices.GetAppRuntime(), appServices.GetTypeRegistry(), (sc, amb) => true);
            var elements = await registry.GetRuntimeElementsAsync();
            var types = elements.OfType<IRuntimeTypeInfo>().ToList();

            Assert.AreEqual(3, types.Count);
            Assert.IsTrue(types.All(t => appServicesInfos.Any(ti => ti.ContractDeclarationType == t.Type)));
            Assert.IsTrue(types.All(t => t[AppServicesModelRegistry.AppServiceKey] is IAppServiceInfo));
        }

        [Test]
        public async Task GetRuntimeElementsAsync_with_filter()
        {
            IAppServiceCollection appServices = this.CreateAppServices();
            var appServicesInfos = new List<ContractDeclaration>
            {
                new (typeof(int), Substitute.For<IAppServiceInfo>()),
                new (typeof(string), Substitute.For<IAppServiceInfo>()),
                new (typeof(bool), Substitute.For<IAppServiceInfo>()),
            };

            var registry = new AppServicesModelRegistry(appServices, appServices.GetAppRuntime(), appServices.GetTypeRegistry(), (sc, amb) => sc.ContractDeclarationType == typeof(int));
            var elements = await registry.GetRuntimeElementsAsync();
            var types = elements.OfType<IRuntimeTypeInfo>().ToList();

            Assert.AreEqual(1, types.Count);
            Assert.IsTrue(types.All(t => appServicesInfos.Any(ti => ti.ContractDeclarationType == t.Type)));
        }

        [Test]
        public async Task GetRuntimeElementsAsync_with_default_filter()
        {
            var appServices = new AppServiceCollectionBuilder(this.CreateAppServices())
                .WithStaticAppRuntime(settings => settings.IsAppAssembly = asm => asm.Name!.StartsWith("Kephas"))
                .AppServices;
            var appServicesInfos = new List<ContractDeclaration>
            {
                new (typeof(int), Substitute.For<IAppServiceInfo>()),
                new (typeof(string), Substitute.For<IAppServiceInfo>()),
                new (typeof(bool), Substitute.For<IAppServiceInfo>()),
                new (typeof(IRuntimeModelRegistry), Substitute.For<IAppServiceInfo>()),
                new (typeof(IModelSpace), Substitute.For<IAppServiceInfo>()),
            };

            var registry = new AppServicesModelRegistry(appServices, appServices.GetAppRuntime(), appServices.GetTypeRegistry());
            var elements = await registry.GetRuntimeElementsAsync();
            var types = elements.OfType<IRuntimeTypeInfo>().ToList();

            Assert.AreEqual(2, types.Count);
            Assert.IsTrue(types.All(t => t.Type.Assembly.GetName().Name.StartsWith("Kephas")));
        }
    }
}