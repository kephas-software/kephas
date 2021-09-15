// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefCompositionContainerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Tests for <see cref="MefCompositionContainer" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Composition.Mef;
using Kephas.Injection;
using Kephas.Injection.Conventions;
using Kephas.Injection.Hosting;

namespace Kephas.Tests.Composition.Mef
{
    using System;
    using System.Collections.Generic;
    using System.Composition;
    using System.Composition.Hosting;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Composition.Mef.Hosting;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Composition;
    using Kephas.Testing.Composition;

    using NSubstitute;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="SystemInjectionContainer"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class MefCompositionContainerTest : MefCompositionTestBase
    {
        public SystemInjectionContainer CreateContainer(params Type[] types)
        {
            return this.WithEmptyConfiguration()
                .WithParts(types)
                .CreateCompositionContainer();
        }

        public SystemInjectionContainer CreateExportProvidersContainer(params Type[] types)
        {
            var config = this.WithEmptyConfiguration();
            return this.WithExportProviders(config).WithParts(types).CreateCompositionContainer();
        }

        [Test]
        public void GetExport_from_ambient_services_self()
        {
            var ambientServices = new AmbientServices();
            var container = this.CreateContainerWithBuilder(ambientServices);
            var containerExport = container.GetExport(typeof(IAmbientServices));
            var ambientExport = container.GetExport(typeof(IAmbientServices));

            Assert.IsNotNull(containerExport);
            Assert.AreSame(ambientExport, containerExport);
            Assert.AreSame(ambientServices, containerExport);
        }

        [Test]
        public void GetExport_from_ambient_services_instance()
        {
            var ambientServices = new AmbientServices();
            var container = this.CreateContainerWithBuilder(ambientServices);
            var containerExport = container.GetExport(typeof(ILogManager));
            var ambientExport = container.GetExport(typeof(ILogManager));

            Assert.IsNotNull(containerExport);
            Assert.AreSame(ambientExport, containerExport);
        }

        [Test]
        public void GetExport_from_ambient_services_instance_type()
        {
            var ambientServices = new AmbientServices();
            var container = this.CreateContainerWithBuilder(ambientServices);
            var containerExport = container.GetExport(typeof(ITypeLoader));
            var ambientExport = container.GetExport(typeof(ITypeLoader));

            Assert.IsNotNull(containerExport);
            Assert.AreSame(ambientExport, containerExport);
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
        public void GetExport_with_exportfactory_success()
        {
            var container = this.CreateExportProvidersContainer(typeof(ExportedClass), typeof(ExportedClassImplicitFactoryImporter));
            var importer = container.GetExport<ExportedClassImplicitFactoryImporter>();

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.ExportedClassFactory);
            var exportedService = importer.ExportedClassFactory.CreateExport().Value;
            Assert.IsInstanceOf<ExportedClass>(exportedService);
        }

        [Test]
        public void GetExport_with_importmany_exportfactory_success()
        {
            var container = this.CreateExportProvidersContainer(typeof(ExportedClass), typeof(ExportedClassImplicitFactoryImporter));
            var importer = container.GetExport<ExportedClassImplicitFactoryImporter>();

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.ExportedClassFactory);
            var exportedService = importer.ExportedClassFactory.CreateExport().Value;
            Assert.IsInstanceOf<ExportedClass>(exportedService);
        }

        [Test]
        public void GetExport_with_exportfactory_metadata_success()
        {
            var container = this.CreateExportProvidersContainer(typeof(ExportedClass), typeof(ExportedClassImplicitFactoryMetadataImporter));
            var importer = container.GetExport<ExportedClassImplicitFactoryMetadataImporter>();

            Assert.IsNotNull(importer);
            Assert.IsNotNull(importer.ExportedClassMetadataFactory);
            var exportedService = importer.ExportedClassMetadataFactory.CreateExport().Value;
            var metadata = importer.ExportedClassMetadataFactory.Metadata;
            Assert.IsInstanceOf<ExportedClass>(exportedService);
            Assert.IsNotNull(metadata);
        }

        [Test]
        public void GetExport_failure()
        {
            var container = this.CreateContainer();
            Assert.Throws<CompositionFailedException>(() => container.GetExport(typeof(ExportedClass)));
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
            var exported = container.GetExports(typeof(IEnumerable<object>));

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
            var container = this.CreateContainerWithBuilder(typeof(ScopeExportedClass));
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
            ambientServices.Register(typeof(IAsyncInitializable), service);

            var container = this.CreateContainerWithBuilder(ambientServices);

            var actualService = container.GetExport<IAsyncInitializable>();
            Assert.AreSame(service, actualService);
        }

        [Test]
        public void GetExport_ambient_services_factory()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterTransient(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>());

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

            ambientServices.Register(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>());

            // This is null because the composition container caches the export providers, and after a first request
            // when the export was not available, will cache the empty export providers.
            service = container.TryGetExport<IAsyncInitializable>();
            Assert.IsNull(service);
        }

        private class ContainerServicesImporter
        {
            [Import]
            public IInjector InjectionContainer { get; set; }
        }

        [Export]
        [Shared(InjectionScopeNames.Default)]
        public class ScopeExportedClass
        {
            public IInjector Injector { get; }

            [ImportingConstructor]
            public ScopeExportedClass(IInjector injector)
            {
                this.Injector = injector;
            }
        }

        [Export]
        public class ExportedClass
        {
        }

        [Export(typeof(ExportedClass))]
        public class ExportedClass2 : ExportedClass
        {
        }

        [Export]
        public class ExportedClassImplicitImporter
        {
            [Import]
            public ExportedClass ExportedClass { get; set; }
        }

        [Export]
        public class ExportedClassImplicitFactoryImporter
        {
            [Import]
            public IExportFactory<ExportedClass> ExportedClassFactory { get; set; }
        }

        [Export]
        public class ExportedClassImplicitManyFactoryImporter
        {
            public ICollection<IExportFactory<ExportedClass>> ExportedClassFactoryCollection { get; set; }
            public IList<IExportFactory<ExportedClass>> ExportedClassFactoryList { get; set; }
            public IEnumerable<IExportFactory<ExportedClass>> ExportedClassFactoryEnumerable { get; set; }
            public IExportFactory<ExportedClass>[] ExportedClassFactoryArray { get; set; }
        }

        [Export]
        public class ExportedClassImplicitFactoryMetadataImporter
        {
            [Import]
            public IExportFactory<ExportedClass, TestMetadata> ExportedClassMetadataFactory { get; set; }
        }

        public class TestMetadata : AppServiceMetadata
        {
            public TestMetadata(IDictionary<string, object?>? metadata)
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
                IInjectionRegistrationContext registrationContext)
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