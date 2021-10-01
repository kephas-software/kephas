﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbientServicesSystemCompositionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the MEF ambient services builder extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Injection.SystemComposition
{
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.Linq;
    using Kephas.Application;
    using Kephas.Injection.SystemComposition;
    using Kephas.Services;
    using Kephas.Testing.Injection;
    using NUnit.Framework;

    [TestFixture]
    public class AmbientServicesSystemCompositionExtensionsTest
    {
        [Test]
        public void BuildWithSystemComposition_defaults()
        {
            IAmbientServices ambientServices = new AmbientServices();
            var builder = ambientServices;
            builder
                .WithDynamicAppRuntime(a => !a.Name.Contains("Test"))
                .BuildWithSystemComposition();

            var injector = ambientServices.Injector;
            Assert.IsInstanceOf<SystemCompositionInjector>(injector);
        }

        [Test]
        public void BuildWithSystemComposition_with_open_generic_override()
        {
            IAmbientServices ambientServices = new AmbientServices();
            var builder = ambientServices;
            builder
                .WithDynamicAppRuntime(a => !a.Name.Contains("Test"))
                .BuildWithSystemComposition(c => c.WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpen<>) }));

            var injector = ambientServices.Injector;
            var moreOpen = injector.Resolve<IOpen<int>>();
            Assert.IsInstanceOf<MoreOpen<int>>(moreOpen);
        }

        [Test]
        [Ignore("Until a fix is found for BUG https://github.com/dotnet/corefx/issues/40094, ignore this test.")]
        public void BuildWithSystemComposition_with_open_generic_override_and_dependency()
        {
            IAmbientServices ambientServices = new AmbientServices();
            var builder = ambientServices;
            builder
                .WithDynamicAppRuntime(a => !a.Name.Contains("Test"))
                .BuildWithSystemComposition(c => c.WithParts(new[] { typeof(IOpen<>), typeof(DefaultOpen<>), typeof(MoreOpenWithDependency<>), typeof(Dependency) }));

            var injector = ambientServices.Injector;
            var moreOpen = injector.Resolve<IOpen<int>>();
            Assert.IsInstanceOf<MoreOpenWithDependency<int>>(moreOpen);
        }

        [Test]
        [Ignore("Until a fix is found for BUG https://github.com/dotnet/corefx/issues/40094, ignore this test.")]
        public void SystemComposition_generic_export_with_ctor_dependency()
        {
            var conventions = new ConventionBuilder();

            conventions
                .ForType<Dependency>()
                .Export<Dependency>();
            conventions
                .ForType(typeof(MoreOpenWithDependency<>))
                .ExportInterfaces(
                i => i.GetGenericTypeDefinition() == typeof(IOpen<>),
                (type, builder) => builder.AsContractType(typeof(IOpen<>)))
                .SelectConstructor(ctors => ctors.First());

            var configuration = new ContainerConfiguration()
                .WithParts(new[] { typeof(IOpen<>), typeof(MoreOpenWithDependency<>) })
                .WithAssembly(this.GetType().Assembly, conventions);

            using var container = configuration.CreateContainer();
            var myService = container.GetExport(typeof(IOpen<object>));
            Assert.IsInstanceOf<MoreOpenWithDependency<object>>(myService);
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