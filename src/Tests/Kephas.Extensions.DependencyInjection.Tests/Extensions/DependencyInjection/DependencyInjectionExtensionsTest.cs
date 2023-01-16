// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Extensions.DependencyInjection
{
    using Kephas.Application;
    using Kephas.Services;
    using Kephas.Services.Builder;
    using Kephas.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    [TestFixture]
    public class DependencyInjectionExtensionsTest : TestBase
    {
        [Test]
        public void BuildWithDependencyInjection_defaults()
        {
            var appServices = this.CreateAppServices();
            var builder = new AppServiceCollectionBuilder(appServices);
            var injector = builder
                .WithDynamicAppRuntime(rt => rt.IsAppAssembly = a => a.Name.Contains("Kephas") && !a.Name.Contains("Test"))
                .BuildWithDependencyInjection();

            Assert.IsInstanceOf<ServiceProvider>(injector);
        }

        [Test]
        public void BuildWithDependencyInjection_with_open_generic_override()
        {
            var appServices = this.CreateAppServices();
            var builder = new AppServiceCollectionBuilder(appServices)
                .WithDynamicAppRuntime(rt => rt.IsAppAssembly = a => !a.Name.Contains("Test"))
                .WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpen<>) });
            var injector = builder.BuildWithDependencyInjection();

            var moreOpen = injector.Resolve<IOpen<int>>();
            Assert.IsInstanceOf<MoreOpen<int>>(moreOpen);
        }

        [Test]
        public void BuildWithDependencyInjection_with_open_generic_override_and_dependency()
        {
            var appServices = this.CreateAppServices();
            var builder = new AppServiceCollectionBuilder(appServices)
                .WithDynamicAppRuntime(rt => rt.IsAppAssembly = a => !a.Name.Contains("Test"))
                .WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpenWithDependency<>), typeof(Dependency) });
            var injector = builder.BuildWithDependencyInjection();

            var moreOpen = injector.Resolve<IOpen<int>>();
            Assert.IsInstanceOf<MoreOpenWithDependency<int>>(moreOpen);
        }

        [Test]
        public void Autofac_generic_export_with_ctor_dependency()
        {
            var conventions = new ServiceCollection();

            conventions.AddSingleton<Dependency>();
            conventions.AddSingleton(typeof(IOpen<>), typeof(MoreOpenWithDependency<>));

            using (var container = conventions.BuildServiceProvider())
            {
                var myService = container.Resolve(typeof(IOpen<object>));
                Assert.IsInstanceOf<MoreOpenWithDependency<object>>(myService);
            }
        }

        [SingletonAppServiceContract(AsOpenGeneric = true)]
        public interface IOpen<T> { }

        public class DefaultOpen<T> : IOpen<T> { }

        [OverridePriority(Priority.High)]
        public class MoreOpen<T> : IOpen<T> { }

        [OverridePriority(Priority.High)]
        public class MoreOpenWithDependency<T> : IOpen<T>
        {
            public MoreOpenWithDependency(Dependency dep)
            {
            }
        }

        [AppServiceContract]
        public class Dependency { }
    }
}