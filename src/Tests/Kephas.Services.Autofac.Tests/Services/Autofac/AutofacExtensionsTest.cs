// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Services.Autofac
{
    using global::Autofac;
    using Kephas.Application;
    using Kephas.Services;
    using Kephas.Services.Autofac;
    using Kephas.Services.Builder;
    using Kephas.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class AutofacExtensionsTest : TestBase
    {
        [Test]
        public void BuildWithAutofac_defaults()
        {
            var appServices = this.CreateAppServices();
            var injector = new AppServiceCollectionBuilder(appServices)
                .WithDynamicAppRuntime(rt => rt.IsAppAssembly = (a => a.Name.Contains("Kephas") && !a.Name.Contains("Test")))
                .BuildWithAutofac();

            Assert.IsInstanceOf<AutofacServiceProvider>(injector);
        }

        [Test]
        public void BuildWithAutofac_with_open_generic_override()
        {
            var appServices = this.CreateAppServices();
            var builder = new AppServiceCollectionBuilder(appServices)
                .WithDynamicAppRuntime(rt => rt.IsAppAssembly = a => !a.Name.Contains("Test"))
                .WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpen<>) });
            var injector = builder.BuildWithAutofac();

            var moreOpen = injector.Resolve<IOpen<int>>();
            Assert.IsInstanceOf<MoreOpen<int>>(moreOpen);
        }

        [Test]
        public void BuildWithAutofac_with_open_generic_override_and_dependency()
        {
            var appServices = this.CreateAppServices();
            var builder = new AppServiceCollectionBuilder(appServices)
                .WithDynamicAppRuntime(rt => rt.IsAppAssembly = a => !a.Name.Contains("Test"))
                .WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpenWithDependency<>), typeof(Dependency) });
            var injector = builder.BuildWithAutofac();

            var moreOpen = injector.Resolve<IOpen<int>>();
            Assert.IsInstanceOf<MoreOpenWithDependency<int>>(moreOpen);
        }

        [Test]
        public void Autofac_generic_export_with_ctor_dependency()
        {
            var conventions = new ContainerBuilder();

            conventions
                .RegisterType<Dependency>()
                .As<Dependency>();
            conventions
                .RegisterGeneric(typeof(MoreOpenWithDependency<>))
                .As(typeof(IOpen<>));

            using (var container = conventions.Build())
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