// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;
using Kephas.Services.Builder;
using Kephas.Services.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using Kephas.Services.Metadata;
using Kephas.Testing;

namespace Kephas.Tests.Extensions.DependencyInjection;

/// <summary>
/// Tests for <see cref="ServiceProvider"/>.
/// </summary>
public class DependencyInjectionTest
{
    public IServiceProvider CreateServiceProvider(params Type[] types)
    {
        var services = new ServiceCollection();
        foreach (var type in types)
        {
            services.AddSingleton(type, type);
        }

        return services.BuildServiceProvider();
    }

    [Fact]
    public void Resolve_from_ambient_services_self()
    {
        var appServices = new AppServiceCollection();
        var container = Test.CreateTestServicesBuilder(appServices).BuildServiceProvider();
        var containerExport = container.GetRequiredService(typeof(IAppServiceCollection));
        var ambientExport = container.GetRequiredService(typeof(IAppServiceCollection));

        Assert.NotNull(containerExport);
        Assert.Same(ambientExport, containerExport);
        Assert.Same(appServices, containerExport);
    }

    [Fact]
    public void Resolve_from_ambient_services_instance()
    {
        var appServices = new AppServiceCollection();
        var container = Test.CreateTestServicesBuilder(appServices).BuildServiceProvider();
        var containerExport = container.GetRequiredService(typeof(DependencyInjectionExtensionsIntegrationTest.ILogManager));
        var ambientExport = container.GetRequiredService(typeof(DependencyInjectionExtensionsIntegrationTest.ILogManager));

        Assert.NotNull(containerExport);
        Assert.Same(ambientExport, containerExport);
    }

    [Fact]
    public void Resolve_from_ambient_services_instance_type()
    {
        var appServices = new AppServiceCollection()
            .Add<ITestTypeLoader, TestTypeLoader>(b => b.Singleton());
        var container = Test.CreateTestServicesBuilder(appServices).BuildServiceProvider();
        var containerExport = container.GetRequiredService(typeof(ITestTypeLoader));
        var ambientExport = container.GetRequiredService(typeof(ITestTypeLoader));

        Assert.NotNull(containerExport);
        Assert.Same(ambientExport, containerExport);
    }

    [Fact]
    public void Resolve_success()
    {
        var container = CreateServiceProvider(typeof(ExportedClass));
        var exported = container.GetRequiredService(typeof(ExportedClass));

        Assert.NotNull(exported);
        Assert.IsType<ExportedClass>(exported);
    }

    [Fact]
    public void TryResolve_success()
    {
        var container = CreateServiceProvider(typeof(ExportedClass));
        var exported = container.GetService(typeof(ExportedClass));

        Assert.NotNull(exported);
        Assert.IsType<ExportedClass>(exported);
    }

    [Fact]
    public void TryResolve_failure()
    {
        var container = CreateServiceProvider();
        var exported = container.GetService(typeof(ExportedClass));

        Assert.Null(exported);
    }

    [Fact]
    public void Resolve_generic_success()
    {
        var container = CreateServiceProvider(typeof(ExportedClass));
        var exported = container.GetRequiredService<ExportedClass>();

        Assert.NotNull(exported);
        Assert.IsType<ExportedClass>(exported);
    }

    [Fact]
    public void TryResolve_generic_success()
    {
        var container = CreateServiceProvider(typeof(ExportedClass));
        var exported = container.GetService<ExportedClass>();

        Assert.NotNull(exported);
        Assert.IsType<ExportedClass>(exported);
    }

    [Fact]
    public void TryResolve_generic_failure()
    {
        var container = CreateServiceProvider();
        var exported = container.GetService<ExportedClass>();

        Assert.Null(exported);
    }

    [Fact]
    public void Resolve_failure()
    {
        var container = CreateServiceProvider();
        Assert.Throws<InvalidOperationException>(() => container.GetRequiredService(typeof(ExportedClass)));
    }

    [Fact]
    public void ResolveMany_success()
    {
        var container = CreateServiceProvider(typeof(ExportedClass));
        var exported = (IEnumerable<object>)container.GetRequiredService(typeof(IEnumerable<ExportedClass>));

        Assert.NotNull(exported);
        var exportedList = exported.ToList();
        Assert.Equal(1, exportedList.Count);
        Assert.IsType<ExportedClass>(exportedList[0]);
    }

    [Fact]
    public void ResolveMany_empty()
    {
        var container = CreateServiceProvider(typeof(ExportedClass));
        var exported = (IEnumerable<object>)container.GetRequiredService(typeof(IEnumerable<string>));

        Assert.NotNull(exported);
        var exportedList = exported.ToList();
        Assert.Equal(0, exportedList.Count);
    }

    [Fact]
    public void ResolveMany_various_same_contract_registrations()
    {
        var builder = Test.CreateTestServicesBuilder()
            .AddProviders(new MultiFilterAppServiceInfoProvider());
        var container = builder.BuildServiceProvider();

        var filters = (IEnumerable<object>)container.GetRequiredService(typeof(IEnumerable<IFilter>));

        Assert.Equal(3, filters.Count());
        Assert.True(filters.OfType<OneFilter>().Any());
        Assert.True(filters.OfType<TwoFilter>().Any());
    }

    [Fact]
    public void GetService_various_same_contract_registrations()
    {
        var container = Test.CreateTestServicesBuilder()
            .AddProviders(new MultiFilterAppServiceInfoProvider())
            .BuildServiceProvider();

        var filters = container.GetRequiredService<IEnumerable<IFilter>>();

        Assert.Equal(3, filters.Count());
        Assert.True(filters.OfType<OneFilter>().Any());
        Assert.True(filters.OfType<TwoFilter>().Any());
    }

    [Fact]
    public void Dispose()
    {
        var container = CreateServiceProvider();
        (container as IDisposable)!.Dispose();
        Assert.Throws<ObjectDisposedException>(() => container.GetService<IList<string>>());
    }

    [Fact]
    public void Dispose_multiple()
    {
        var container = CreateServiceProvider();
        (container as IDisposable)!.Dispose();
        (container as IDisposable)!.Dispose();
    }

    [Fact]
    public void CreateScopedInjector_ScopeExportedClass()
    {
        var appServices = new AppServiceCollection();
        appServices.Add(new AppServiceInfo(
            typeof(ScopeExportedClass),
            typeof(ScopeExportedClass),
            AppServiceLifetime.Scoped));
        var builder = Test.CreateTestServicesBuilder(appServices);
        var container = builder.BuildServiceProvider();

        using var scopedContext = container.CreateScope();
        using var otherScopedContext = container.CreateScope();
        var scopedInstance1 = scopedContext.ServiceProvider.GetRequiredService<ScopeExportedClass>();
        var scopedInstance2 = scopedContext.ServiceProvider.GetRequiredService<ScopeExportedClass>();

        Assert.Same(scopedInstance1, scopedInstance2);

        var otherScopedInstance = otherScopedContext.ServiceProvider.GetRequiredService<ScopeExportedClass>();
        Assert.NotSame(scopedInstance1, otherScopedInstance);
    }

    [Fact]
    public void Resolve_ambient_services()
    {
        var appServices = new AppServiceCollection();
        var service = Substitute.For<IAsyncInitializable>();
        appServices.Add(typeof(IAsyncInitializable), service);

        var container = Test.CreateTestServicesBuilder(appServices).BuildServiceProvider();

        var actualService = container.GetRequiredService<IAsyncInitializable>();
        Assert.Same(service, actualService);
    }

    [Fact]
    public void Resolve_ambient_services_factory()
    {
        var appServices = new AppServiceCollection();
        appServices.Add(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>(), b => b.Transient());

        var container = Test.CreateTestServicesBuilder(appServices).BuildServiceProvider();

        var service1 = container.GetRequiredService<IAsyncInitializable>();
        var service2 = container.GetRequiredService<IAsyncInitializable>();
        Assert.NotSame(service1, service2);
    }

    [Fact]
    public void Resolve_ambient_services_not_available_after_first_failed_request()
    {
        var appServices = new AppServiceCollection();
        var container = Test.CreateTestServicesBuilder(appServices).BuildServiceProvider();

        var service = container.GetService<IAsyncInitializable>();
        Assert.Null(service);

        appServices.Add(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>());

        // This is null because the injector caches the export providers, and after a first request
        // when the export was not available, will cache the empty export providers.
        service = container.GetService<IAsyncInitializable>();
        Assert.Null(service);
    }

    public class ScopeExportedClass
    {
        public IServiceProvider ServiceProvider { get; }

        public ScopeExportedClass(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
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

    public interface IAsyncInitializable {}

    public interface ITestTypeLoader {}

    public class TestTypeLoader : ITestTypeLoader {}
}
