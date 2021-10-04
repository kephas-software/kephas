﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacInjectorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Injection.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using global::Autofac;
    using global::Autofac.Core.Registration;
    using Kephas.Injection;
    using Kephas.Injection.Autofac;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Reflection;
    using Kephas.Testing.Injection;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="AutofacInjector"/>.
    /// </summary>
    [TestFixture]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    public class AutofacInjectorTest : AutofacInjectionTestBase
    {
        public AutofacInjector CreateInjector(params Type[] types)
        {
            var builder = this.WithEmptyConfiguration();
            builder.RegisterTypes(types);
            return new AutofacInjector(builder);
        }

        [Test]
        public void Resolve_from_ambient_services_self()
        {
            var ambientServices = new AmbientServices();
            var container = this.CreateInjectorWithBuilder(ambientServices);
            var containerExport = container.Resolve(typeof(IAmbientServices));
            var ambientExport = container.Resolve(typeof(IAmbientServices));

            Assert.IsNotNull(containerExport);
            Assert.AreSame(ambientExport, containerExport);
            Assert.AreSame(ambientServices, containerExport);
        }

        [Test]
        public void Resolve_from_ambient_services_instance()
        {
            var ambientServices = new AmbientServices();
            var container = this.CreateInjectorWithBuilder(ambientServices);
            var containerExport = container.Resolve(typeof(ILogManager));
            var ambientExport = container.Resolve(typeof(ILogManager));

            Assert.IsNotNull(containerExport);
            Assert.AreSame(ambientExport, containerExport);
        }

        [Test]
        public void Resolve_from_ambient_services_instance_type()
        {
            var ambientServices = new AmbientServices();
            var container = this.CreateInjectorWithBuilder(ambientServices);
            var containerExport = container.Resolve(typeof(ITypeLoader));
            var ambientExport = container.Resolve(typeof(ITypeLoader));

            Assert.IsNotNull(containerExport);
            Assert.AreSame(ambientExport, containerExport);
        }

        [Test]
        public void Resolve_success()
        {
            var container = this.CreateInjector(typeof(ExportedClass));
            var exported = container.Resolve(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryResolve_success()
        {
            var container = this.CreateInjector(typeof(ExportedClass));
            var exported = container.TryResolve(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryResolve_failure()
        {
            var container = this.CreateInjector();
            var exported = container.TryResolve(typeof(ExportedClass));

            Assert.IsNull(exported);
        }

        [Test]
        public void Resolve_generic_success()
        {
            var container = this.CreateInjector(typeof(ExportedClass));
            var exported = container.Resolve<ExportedClass>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryResolve_generic_success()
        {
            var container = this.CreateInjector(typeof(ExportedClass));
            var exported = container.TryResolve<ExportedClass>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryResolve_generic_failure()
        {
            var container = this.CreateInjector();
            var exported = container.TryResolve<ExportedClass>();

            Assert.IsNull(exported);
        }

        [Test]
        public void Resolve_failure()
        {
            var container = this.CreateInjector();
            Assert.Throws<ComponentNotRegisteredException>(() => container.Resolve(typeof(ExportedClass)));
        }

        [Test]
        public void ResolveMany_success()
        {
            var container = this.CreateInjector(typeof(ExportedClass));
            var exported = container.ResolveMany(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(1, exportedList.Count);
            Assert.IsInstanceOf<ExportedClass>(exportedList[0]);
        }

        [Test]
        public void ResolveMany_empty()
        {
            var container = this.CreateInjector(typeof(ExportedClass));
            var exported = container.ResolveMany(typeof(string));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(0, exportedList.Count);
        }

        [Test]
        public void ResolveMany_various_same_contract_registrations()
        {
            var container = this.CreateInjector(
                parts: new[] { typeof(IFilter), typeof(OneFilter), typeof(TwoFilter) },
                config: b => { b.WithAppServiceInfosProvider(new MultiFilterAppServiceInfosProvider()); });

            var filters = container.ResolveMany(typeof(IFilter));

            Assert.AreEqual(3, filters.Count());
            Assert.IsTrue(filters.OfType<OneFilter>().Any());
            Assert.IsTrue(filters.OfType<TwoFilter>().Any());
        }

        [Test]
        public void GetService_various_same_contract_registrations()
        {
            var container = this.CreateInjector(
                parts: new[] { typeof(IFilter), typeof(OneFilter), typeof(TwoFilter) },
                config: b => { b.WithAppServiceInfosProvider(new MultiFilterAppServiceInfosProvider()); });

            var rawFilters = container.ToServiceProvider().GetService(typeof(IEnumerable<IFilter>));
            var filters = rawFilters as IEnumerable<IFilter>;

            Assert.AreEqual(3, filters.Count());
            Assert.IsTrue(filters.OfType<OneFilter>().Any());
            Assert.IsTrue(filters.OfType<TwoFilter>().Any());
        }

        [Test]
        public void Dispose()
        {
            var container = this.CreateInjector();
            container.Dispose();
            Assert.Throws<ObjectDisposedException>(() => container.TryResolve<IList<string>>());
        }

        [Test]
        public void Dispose_multiple()
        {
            var container = this.CreateInjector();
            container.Dispose();
            container.Dispose();
        }

        [Test]
        public void CreateScopedInjector_ScopeExportedClass()
        {
            var container = this.CreateInjectorWithBuilder(
                b => b.WithRegistration(
                    new AppServiceInfo(
                        typeof(ScopeExportedClass),
                        typeof(ScopeExportedClass),
                        AppServiceLifetime.Scoped)));
            using (var scopedContext = container.CreateScopedInjector())
            using (var otherScopedContext = container.CreateScopedInjector())
            {
                var scopedInstance1 = scopedContext.Resolve<ScopeExportedClass>();
                var scopedInstance2 = scopedContext.Resolve<ScopeExportedClass>();

                Assert.AreSame(scopedInstance1, scopedInstance2);

                var otherScopedInstance = otherScopedContext.Resolve<ScopeExportedClass>();
                Assert.AreNotSame(scopedInstance1, otherScopedInstance);
            }
        }

        [Test]
        public void Resolve_ambient_services()
        {
            var ambientServices = new AmbientServices();
            var service = Substitute.For<IAsyncInitializable>();
            ambientServices.Register(typeof(IAsyncInitializable), service);

            var container = this.CreateInjectorWithBuilder(ambientServices);

            var actualService = container.Resolve<IAsyncInitializable>();
            Assert.AreSame(service, actualService);
        }

        [Test]
        public void Resolve_ambient_services_factory()
        {
            var ambientServices = new AmbientServices();
            ambientServices.RegisterTransient(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>());

            var container = this.CreateInjectorWithBuilder(ambientServices);

            var service1 = container.Resolve<IAsyncInitializable>();
            var service2 = container.Resolve<IAsyncInitializable>();
            Assert.AreNotSame(service1, service2);
        }

        [Test]
        public void Resolve_ambient_services_not_available_after_first_failed_request()
        {
            var ambientServices = new AmbientServices();
            var container = this.CreateInjectorWithBuilder(ambientServices);

            var service = container.TryResolve<IAsyncInitializable>();
            Assert.IsNull(service);

            ambientServices.Register(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>());

            // This is null because the injector caches the export providers, and after a first request
            // when the export was not available, will cache the empty export providers.
            service = container.TryResolve<IAsyncInitializable>();
            Assert.IsNull(service);
        }

        public class ScopeExportedClass
        {
            public IInjector Injector { get; }

            public ScopeExportedClass(IInjector injector)
            {
                this.Injector = injector;
            }
        }

        public class ExportedClass
        {
        }

        public class ExportedClassImplicitImporter
        {
            public ExportedClass ExportedClass { get; set; }
        }

        public class ExportedClassImplicitFactoryImporter
        {
            public IExportFactory<ExportedClass> ExportedClassFactory { get; set; }
        }

        public class ExportedClassImplicitManyFactoryImporter
        {
            public ICollection<IExportFactory<ExportedClass>> ExportedClassFactoryCollection { get; set; }
            public IList<IExportFactory<ExportedClass>> ExportedClassFactoryList { get; set; }
            public IEnumerable<IExportFactory<ExportedClass>> ExportedClassFactoryEnumerable { get; set; }
            public IExportFactory<ExportedClass>[] ExportedClassFactoryArray { get; set; }
        }

        public class TestMetadata : AppServiceMetadata
        {
            public TestMetadata(IDictionary<string, object?>? metadata)
                : base(metadata)
            {
            }
        }

        public class MultiFilterAppServiceInfosProvider : IAppServiceInfosProvider
        {
            public IEnumerable<(Type contractDeclarationType, IAppServiceInfo appServiceInfo)> GetAppServiceInfos(dynamic? context = null)
            {
                yield return (typeof(IFilter), new AppServiceInfo(typeof(IFilter), typeof(OneFilter), AppServiceLifetime.Singleton));
                yield return (typeof(IFilter), new AppServiceInfo(typeof(IFilter), typeof(TwoFilter), AppServiceLifetime.Transient));
                yield return (typeof(IFilter), new AppServiceInfo(typeof(IFilter), Substitute.For<IFilter>()));
            }
        }

        public interface IFilter { }

        public class OneFilter : IFilter { }

        public class TwoFilter : IFilter { }
    }
}