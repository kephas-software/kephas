// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacCompositionContainerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="AutofacCompositionContainer" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Composition.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using global::Autofac;
    using global::Autofac.Core.Registration;

    using Kephas.Composition;
    using Kephas.Composition.AttributedModel;
    using Kephas.Composition.Autofac.Hosting;
    using Kephas.Composition.Conventions;
    using Kephas.Composition.Hosting;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Composition.Mef;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="AutofacCompositionContainer"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacCompositionContainerTest : CompositionTestBase
    {
        public AutofacCompositionContainer CreateContainer(params Type[] types)
        {
            var builder = this.WithEmptyConfiguration();
            builder.RegisterTypes(types);
            return new AutofacCompositionContainer(builder);
        }

        [Test]
        public void GetExport_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExport(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryGetExport_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.TryGetExport(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryGetExport_failure()
        {
            var container = this.CreateContainer();
            var exported = container.TryGetExport(typeof(ExportedClass));

            Assert.IsNull(exported);
        }

        [Test]
        public void GetExport_generic_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExport<ExportedClass>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryGetExport_generic_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.TryGetExport<ExportedClass>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryGetExport_generic_failure()
        {
            var container = this.CreateContainer();
            var exported = container.TryGetExport<ExportedClass>();

            Assert.IsNull(exported);
        }

        [Test]
        public void GetExport_failure()
        {
            var container = this.CreateContainer();
            Assert.Throws<ComponentNotRegisteredException>(() => container.GetExport(typeof(ExportedClass)));
        }

        [Test]
        public void GetExports_success()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExports(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(1, exportedList.Count);
            Assert.IsInstanceOf<ExportedClass>(exportedList[0]);
        }

        [Test]
        public void GetExports_empty()
        {
            var container = this.CreateContainer(typeof(ExportedClass));
            var exported = container.GetExports(typeof(string));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(0, exportedList.Count);
        }

        [Test]
        public void GetExports_various_same_contract_registrations()
        {
            var container = this.CreateContainer(
                parts: new[] { typeof(IFilter), typeof(OneFilter), typeof(TwoFilter) },
                config: b => { b.WithConventionsRegistrar(new MultiFilterConventionsRegistrar()); });

            var filters = container.GetExports(typeof(IFilter));

            Assert.AreEqual(3, filters.Count());
            Assert.IsTrue(filters.OfType<OneFilter>().Any());
            Assert.IsTrue(filters.OfType<TwoFilter>().Any());
        }

        [Test]
        public void GetService_various_same_contract_registrations()
        {
            var container = this.CreateContainer(
                parts: new[] { typeof(IFilter), typeof(OneFilter), typeof(TwoFilter) },
                config: b => { b.WithConventionsRegistrar(new MultiFilterConventionsRegistrar()); });

            var rawFilters = container.ToServiceProvider().GetService(typeof(IEnumerable<IFilter>));
            var filters = rawFilters as IEnumerable<IFilter>;

            Assert.AreEqual(3, filters.Count());
            Assert.IsTrue(filters.OfType<OneFilter>().Any());
            Assert.IsTrue(filters.OfType<TwoFilter>().Any());
        }

        [Test]
        public void Dispose()
        {
            var container = this.CreateContainer();
            container.Dispose();
            Assert.Throws<ObjectDisposedException>(() => container.TryGetExport<IList<string>>());
        }

        [Test]
        public void Dispose_multiple()
        {
            var container = this.CreateContainer();
            container.Dispose();
            container.Dispose();
        }

        [Test]
        public void CreateScopedContext_ScopeExportedClass()
        {
            var container = this.CreateContainerWithBuilder(
                b => b.WithRegistration(
                    new AppServiceInfo(
                        typeof(ScopeExportedClass),
                        typeof(ScopeExportedClass),
                        AppServiceLifetime.Scoped)));
            using (var scopedContext = container.CreateScopedContext())
            using (var otherScopedContext = container.CreateScopedContext())
            {
                var scopedInstance1 = scopedContext.GetExport<ScopeExportedClass>();
                var scopedInstance2 = scopedContext.GetExport<ScopeExportedClass>();

                Assert.AreSame(scopedInstance1, scopedInstance2);

                var otherScopedInstance = otherScopedContext.GetExport<ScopeExportedClass>();
                Assert.AreNotSame(scopedInstance1, otherScopedInstance);
            }
        }

        [Test]
        public void GetExport_ambient_services()
        {
            var ambientServices = new AmbientServices();
            var service = Substitute.For<IAsyncInitializable>();
            ambientServices.RegisterService(typeof(IAsyncInitializable), service);

            var container = this.CreateContainerWithBuilder(ambientServices);

            var actualService = container.GetExport<IAsyncInitializable>();
            Assert.AreSame(service, actualService);
        }

        [Test]
        public void GetExport_ambient_services_factory()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterService(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>());

            var container = this.CreateContainerWithBuilder(ambientServices);

            var service1 = container.GetExport<IAsyncInitializable>();
            var service2 = container.GetExport<IAsyncInitializable>();
            Assert.AreNotSame(service1, service2);
        }

        [Test]
        public void GetExport_ambient_services_not_available_after_first_failed_request()
        {
            var ambientServices = new AmbientServices();
            var container = this.CreateContainerWithBuilder(ambientServices);

            var service = container.TryGetExport<IAsyncInitializable>();
            Assert.IsNull(service);

            ambientServices.RegisterService(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>());

            // This is null because the composition container caches the export providers, and after a first request
            // when the export was not available, will cache the empty export providers.
            service = container.TryGetExport<IAsyncInitializable>();
            Assert.IsNull(service);
        }

        private class ContainerServicesImporter
        {
            [Import]
            public ICompositionContext CompositionContainer { get; set; }
        }

        //[Export]
        //[Scoped(CompositionScopeNames.Default)]
        public class ScopeExportedClass
        {
            public ICompositionContext CompositionContext { get; }

            //[ImportingConstructor]
            public ScopeExportedClass(ICompositionContext compositionContext)
            {
                this.CompositionContext = compositionContext;
            }
        }

        //[Export]
        public class ExportedClass
        {
        }

        //[Export(typeof(ExportedClass))]
        public class ExportedClass2 : ExportedClass
        {
        }

        //[Export]
        public class ExportedClassImplicitImporter
        {
            [Import]
            public ExportedClass ExportedClass { get; set; }
        }

        //[Export]
        public class ExportedClassImplicitFactoryImporter
        {
            [Import]
            public IExportFactory<ExportedClass> ExportedClassFactory { get; set; }
        }

        //[Export]
        public class ExportedClassImplicitManyFactoryImporter
        {
            public ICollection<IExportFactory<ExportedClass>> ExportedClassFactoryCollection { get; set; }
            public IList<IExportFactory<ExportedClass>> ExportedClassFactoryList { get; set; }
            public IEnumerable<IExportFactory<ExportedClass>> ExportedClassFactoryEnumerable { get; set; }
            public IExportFactory<ExportedClass>[] ExportedClassFactoryArray { get; set; }
        }

        //[Export]
        public class ExportedClassImplicitFactoryMetadataImporter
        {
            [Import]
            public IExportFactory<ExportedClass, TestMetadata> ExportedClassMetadataFactory { get; set; }
        }

        public class TestMetadata : AppServiceMetadata
        {
            public TestMetadata(IDictionary<string, object> metadata)
                : base(metadata)
            {
            }
        }

        private class ConsumerOfExportedClassImplicitImporter
        {
            [Import]
            public ExportedClassImplicitImporter ImplicitImporter { get; set; }
        }

        private class ConsumerOfExportedClass
        {
            [Import]
            public ExportedClass ExportedClass { get; set; }
        }

        public class MultiFilterConventionsRegistrar : IConventionsRegistrar
        {
            public void RegisterConventions(
                IConventionsBuilder builder,
                IList<Type> candidateTypes,
                ICompositionRegistrationContext registrationContext)
            {
                builder.ForType(typeof(OneFilter)).ExportInterface(
                    typeof(IFilter),
                    (t, b) => b.AsContractType(typeof(IFilter)))
                    .Singleton();
                builder.ForType(typeof(TwoFilter)).ExportInterface(
                        typeof(IFilter),
                        (t, b) => b.AsContractType(typeof(IFilter)));
                builder.ForInstance(typeof(IFilter), Substitute.For<IFilter>());
            }
        }

        public interface IFilter { }

        public class OneFilter : IFilter { }

        public class TwoFilter : IFilter { }
    }
}