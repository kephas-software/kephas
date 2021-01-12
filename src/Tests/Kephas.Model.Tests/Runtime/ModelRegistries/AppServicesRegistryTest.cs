// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesRegistryTest.cs" company="Kephas Software SRL">
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
    using Kephas.Composition;
    using Kephas.Composition.Lite.Hosting;
    using Kephas.Logging;
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.ModelRegistries;
    using Kephas.Reflection;
    using Kephas.Services.Composition;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Composition;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AppServicesRegistryTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(
            IAmbientServices ambientServices = null,
            IEnumerable<Assembly> assemblies = null,
            IEnumerable<Type> parts = null,
            Action<LiteCompositionContainerBuilder> config = null,
            ILogManager? logManager = null,
            IAppRuntime? appRuntime = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(AppServicesRegistry).GetTypeInfo().Assembly); /* Kephas.Model */
            return base.CreateContainer(ambientServices, assemblyList, parts, config);
        }

        [Test]
        public void AppServicesRegistry_Composition_success()
        {
            var container = this.CreateContainer();
            var registry = container.GetExports<IRuntimeModelRegistry>().OfType<AppServicesRegistry>().SingleOrDefault();
            Assert.IsNotNull(registry);
        }

        [Test]
        public async Task GetRuntimeElementsAsync_from_Kephas_Model()
        {
            var ambientServices = new AmbientServices().WithStaticAppRuntime();
            var appServicesInfos = new List<(Type contractType, IAppServiceInfo appServiceInfo)>
                                       {
                                           (typeof(IModelSpace), Substitute.For<IAppServiceInfo>()),
                                           (typeof(IModelSpaceProvider), Substitute.For<IAppServiceInfo>()),
                                           (typeof(bool), Substitute.For<IAppServiceInfo>()),
                                       };

            ambientServices.SetAppServiceInfos(appServicesInfos);

            var registry = new AppServicesRegistry(ambientServices);
            var elements = await registry.GetRuntimeElementsAsync();
            var types = elements.OfType<Type>().ToList();

            Assert.AreEqual(2, types.Count);
            Assert.IsTrue(types.All(t => appServicesInfos.Any(ti => ti.contractType == t && t.Assembly == typeof(IModelSpace).Assembly)));
        }
    }
}