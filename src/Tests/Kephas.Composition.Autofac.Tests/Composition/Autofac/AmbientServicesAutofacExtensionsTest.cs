// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesAutofacExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF ambient services builder extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Composition.Autofac
{
    using global::Autofac;

    using Kephas.Application;
    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Services;

    using NUnit.Framework;

    [TestFixture]
    public class AmbientServicesAutofacExtensionsTest
    {
        [Test]
        public void WithAutofacCompositionContainer_defaults()
        {
            var ambientServices = new AmbientServices();
            var builder = ambientServices;
            builder
                .WithDynamicAppRuntime(a => a.Name.Contains("Kephas") && !a.Name.Contains("Test"))
                .WithAutofacCompositionContainer();

            var compositionContext = ambientServices.CompositionContainer;
            Assert.IsInstanceOf<AutofacCompositionContainer>(compositionContext);
        }

        [Test]
        public void WithAutofacCompositionContainer_with_open_generic_override()
        {
            var ambientServices = new AmbientServices();
            var builder = ambientServices;
            builder
                .WithDynamicAppRuntime(a => !a.Name.Contains("Test"))
                .WithAutofacCompositionContainer(c => c.WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpen<>) }));

            var compositionContext = ambientServices.CompositionContainer;
            var moreOpen = compositionContext.GetExport<IOpen<int>>();
            Assert.IsInstanceOf<MoreOpen<int>>(moreOpen);
        }

        [Test]
        public void WithAutofacCompositionContainer_with_open_generic_override_and_dependency()
        {
            var ambientServices = new AmbientServices();
            var builder = ambientServices;
            builder
                .WithDynamicAppRuntime(a => !a.Name.Contains("Test"))
                .WithAutofacCompositionContainer(c => c.WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpenWithDependency<>), typeof(Dependency) }));

            var compositionContext = ambientServices.CompositionContainer;
            var moreOpen = compositionContext.GetExport<IOpen<int>>();
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