// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesAutofacExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF ambient services builder extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Extensions.DependencyInjection
{
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class AmbientServicesAutofacExtensionsTest : TestBase
    {
        [Test]
        public void BuildWithAutofac_defaults()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var builder = ambientServices;
            builder
                .WithDynamicAppRuntime(config: rt => rt.OnIsAppAssembly(a => a.Name.Contains("Kephas") && !a.Name.Contains("Test")))
                .BuildWithAutofac();

            var injector = ambientServices.Injector;
            Assert.IsInstanceOf<AutofacServiceProvider>(injector);
        }

        [Test]
        public void BuildWithAutofac_with_open_generic_override()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var builder = ambientServices;
            builder
                .WithDynamicAppRuntime(config: rt => rt.OnIsAppAssembly(a => !a.Name.Contains("Test")))
                .BuildWithAutofac(c => c.WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpen<>) }));

            var injector = ambientServices.Injector;
            var moreOpen = injector.Resolve<IOpen<int>>();
            Assert.IsInstanceOf<MoreOpen<int>>(moreOpen);
        }

        [Test]
        public void BuildWithAutofac_with_open_generic_override_and_dependency()
        {
            IAmbientServices ambientServices = this.CreateAmbientServices();
            var builder = ambientServices;
            builder
                .WithDynamicAppRuntime(config: rt => rt.OnIsAppAssembly(a => !a.Name.Contains("Test")))
                .BuildWithAutofac(c => c.WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpenWithDependency<>), typeof(Dependency) }));

            var injector = ambientServices.Injector;
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