// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesRegistryTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application services registry test class.
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
    using Kephas.Model.Runtime;
    using Kephas.Model.Runtime.ModelRegistries;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Testing.Composition.Mef;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AppServicesRegistryTest : CompositionTestBase
    {
        public override ICompositionContext CreateContainer(IEnumerable<Assembly> assemblies, IEnumerable<Type> parts = null, Action<MefCompositionContainerBuilder> config = null)
        {
            var assemblyList = new List<Assembly>(assemblies ?? new Assembly[0]);
            assemblyList.Add(typeof(AppServicesRegistry).GetTypeInfo().Assembly); /* Kephas.Model */
            return base.CreateContainer(assemblyList, parts, config);
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
            var appRuntime = Substitute.For<IAppRuntime>();
            appRuntime.GetAppAssembliesAsync(Arg.Any<Func<AssemblyName, bool>>(), CancellationToken.None)
                .Returns(Task.FromResult((IEnumerable<Assembly>)new[] { typeof(AppServicesRegistry).GetTypeInfo().Assembly }));

            var registry = new AppServicesRegistry(appRuntime, new DefaultTypeLoader());
            var elements = await registry.GetRuntimeElementsAsync();
            var types = elements.OfType<Type>().ToList();
            Assert.IsTrue(types.All(t => t.GetTypeInfo().GetCustomAttribute<AppServiceContractAttribute>() != null));
        }
    }
}