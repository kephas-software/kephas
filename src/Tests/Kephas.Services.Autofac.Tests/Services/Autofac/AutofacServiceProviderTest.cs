// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutofacServiceProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests.Services.Autofac
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using global::Autofac;
    using global::Autofac.Core.Registration;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Services;
    using Kephas.Services.Autofac;
    using Kephas.Services.Builder;
    using Kephas.Services.Reflection;
    using Kephas.Testing;
    using NSubstitute;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="AutofacServiceProvider"/>.
    /// </summary>
    [TestFixture]
    public class AutofacServiceProviderTest : TestBase
    {
        public AutofacServiceProvider CreateServiceProvider(params Type[] types)
        {
            var builder = new ContainerBuilder();
            builder.RegisterTypes(types);
            return new AutofacServiceProvider(builder, null);
        }

        [Test]
        public void Resolve_from_ambient_services_self()
        {
            var appServices = this.CreateAppServices();
            var container = this.CreateServicesBuilder(appServices).BuildWithAutofac();
            var containerExport = container.Resolve(typeof(IAppServiceCollection));
            var ambientExport = container.Resolve(typeof(IAppServiceCollection));

            Assert.IsNotNull(containerExport);
            Assert.AreSame(ambientExport, containerExport);
            Assert.AreSame(appServices, containerExport);
        }

        [Test]
        public void Resolve_from_ambient_services_instance()
        {
            var appServices = this.CreateAppServices();
            var container = this.CreateServicesBuilder(appServices).BuildWithAutofac();
            var containerExport = container.Resolve(typeof(ILogManager));
            var ambientExport = container.Resolve(typeof(ILogManager));

            Assert.IsNotNull(containerExport);
            Assert.AreSame(ambientExport, containerExport);
        }

        [Test]
        public void Resolve_from_ambient_services_instance_type()
        {
            var appServices = this.CreateAppServices();
            var container = this.CreateServicesBuilder(appServices).BuildWithAutofac();
            var containerExport = container.Resolve(typeof(ITypeLoader));
            var ambientExport = container.Resolve(typeof(ITypeLoader));

            Assert.IsNotNull(containerExport);
            Assert.AreSame(ambientExport, containerExport);
        }

        [Test]
        public void Resolve_success()
        {
            var container = this.CreateServiceProvider(typeof(ExportedClass));
            var exported = container.Resolve(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryResolve_success()
        {
            var container = this.CreateServiceProvider(typeof(ExportedClass));
            var exported = container.TryResolve(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryResolve_failure()
        {
            var container = this.CreateServiceProvider();
            var exported = container.TryResolve(typeof(ExportedClass));

            Assert.IsNull(exported);
        }

        [Test]
        public void Resolve_generic_success()
        {
            var container = this.CreateServiceProvider(typeof(ExportedClass));
            var exported = container.Resolve<ExportedClass>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryResolve_generic_success()
        {
            var container = this.CreateServiceProvider(typeof(ExportedClass));
            var exported = container.TryResolve<ExportedClass>();

            Assert.IsNotNull(exported);
            Assert.IsInstanceOf<ExportedClass>(exported);
        }

        [Test]
        public void TryResolve_generic_failure()
        {
            var container = this.CreateServiceProvider();
            var exported = container.TryResolve<ExportedClass>();

            Assert.IsNull(exported);
        }

        [Test]
        public void Resolve_failure()
        {
            var container = this.CreateServiceProvider();
            Assert.Throws<ComponentNotRegisteredException>(() => container.Resolve(typeof(ExportedClass)));
        }

        [Test]
        public void ResolveMany_success()
        {
            var container = this.CreateServiceProvider(typeof(ExportedClass));
            var exported = container.ResolveMany(typeof(ExportedClass));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(1, exportedList.Count);
            Assert.IsInstanceOf<ExportedClass>(exportedList[0]);
        }

        [Test]
        public void ResolveMany_empty()
        {
            var container = this.CreateServiceProvider(typeof(ExportedClass));
            var exported = container.ResolveMany(typeof(string));

            Assert.IsNotNull(exported);
            var exportedList = exported.ToList();
            Assert.AreEqual(0, exportedList.Count);
        }

        [Test]
        public void ResolveMany_various_same_contract_registrations()
        {
            var builder = this.CreateServicesBuilder()
                .WithServiceInfoProviders(new MultiFilterAppServiceInfoProvider());
            var container = builder.BuildWithAutofac();

            var filters = container.ResolveMany(typeof(IFilter));

            Assert.AreEqual(3, filters.Count());
            Assert.IsTrue(filters.OfType<OneFilter>().Any());
            Assert.IsTrue(filters.OfType<TwoFilter>().Any());
        }

        [Test]
        public void GetService_various_same_contract_registrations()
        {
            var container = this.CreateServicesBuilder()
                .WithServiceInfoProviders(new MultiFilterAppServiceInfoProvider())
                .BuildWithAutofac();

            var rawFilters = container.GetService(typeof(IEnumerable<IFilter>));
            var filters = rawFilters as IEnumerable<IFilter>;

            Assert.AreEqual(3, filters.Count());
            Assert.IsTrue(filters.OfType<OneFilter>().Any());
            Assert.IsTrue(filters.OfType<TwoFilter>().Any());
        }

        [Test]
        public void Dispose()
        {
            var container = this.CreateServiceProvider();
            container.Dispose();
            Assert.Throws<ObjectDisposedException>(() => container.TryResolve<IList<string>>());
        }

        [Test]
        public void Dispose_multiple()
        {
            var container = this.CreateServiceProvider();
            container.Dispose();
            container.Dispose();
        }

        [Test]
        public void CreateScopedInjector_ScopeExportedClass()
        {
            var builder = this.CreateServicesBuilder();
            builder.AppServices.Add(
                new AppServiceInfo(
                    typeof(ScopeExportedClass),
                    typeof(ScopeExportedClass),
                    AppServiceLifetime.Scoped));
            var container = builder.BuildWithAutofac();

            using var scopedContext = container.CreateScope();
            using var otherScopedContext = container.CreateScope();
            var scopedInstance1 = scopedContext.ServiceProvider.Resolve<ScopeExportedClass>();
            var scopedInstance2 = scopedContext.ServiceProvider.Resolve<ScopeExportedClass>();

            Assert.AreSame(scopedInstance1, scopedInstance2);

            var otherScopedInstance = otherScopedContext.ServiceProvider.Resolve<ScopeExportedClass>();
            Assert.AreNotSame(scopedInstance1, otherScopedInstance);
        }

        [Test]
        public void Resolve_ambient_services()
        {
            var appServices = this.CreateAppServices();
            var service = Substitute.For<IAsyncInitializable>();
            appServices.Add(typeof(IAsyncInitializable), service);

            var container = this.CreateServicesBuilder(appServices).BuildWithAutofac();

            var actualService = container.Resolve<IAsyncInitializable>();
            Assert.AreSame(service, actualService);
        }

        [Test]
        public void Resolve_ambient_services_factory()
        {
            var appServices = this.CreateAppServices();
            appServices.Add(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>(), b => b.Transient());

            var container = this.CreateServicesBuilder(appServices).BuildWithAutofac();

            var service1 = container.Resolve<IAsyncInitializable>();
            var service2 = container.Resolve<IAsyncInitializable>();
            Assert.AreNotSame(service1, service2);
        }

        [Test]
        public void Resolve_ambient_services_not_available_after_first_failed_request()
        {
            var appServices = this.CreateAppServices();
            var container = this.CreateServicesBuilder(appServices).BuildWithAutofac();

            var service = container.TryResolve<IAsyncInitializable>();
            Assert.IsNull(service);

            appServices.Add(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>());

            // This is null because the injector caches the export providers, and after a first request
            // when the export was not available, will cache the empty export providers.
            service = container.TryResolve<IAsyncInitializable>();
            Assert.IsNull(service);
        }

        public class ScopeExportedClass
        {
            public IServiceProvider ServiceProvider { get; }

            public ScopeExportedClass(IServiceProvider serviceProvider)
            {
                this.ServiceProvider = serviceProvider;
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

        public class MultiFilterAppServiceInfoProvider : IAppServiceInfoProvider
        {
            public IEnumerable<ContractDeclaration> GetAppServiceContracts()
            {
                yield return new ContractDeclaration(typeof(IFilter), new AppServiceInfo(typeof(IFilter), typeof(OneFilter), AppServiceLifetime.Singleton) { AllowMultiple = true });
                yield return new ContractDeclaration(typeof(IFilter), new AppServiceInfo(typeof(IFilter), typeof(TwoFilter), AppServiceLifetime.Transient) { AllowMultiple = true });
                yield return new ContractDeclaration(typeof(IFilter), new AppServiceInfo(typeof(IFilter), Substitute.For<IFilter>()) { AllowMultiple = true });
            }
        }

        public interface IFilter { }

        public class OneFilter : IFilter { }

        public class TwoFilter : IFilter { }
    }
}