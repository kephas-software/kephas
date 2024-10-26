// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.DependencyInjection;

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
            ServiceCollectionExtensions.AddSingleton(services, type, type);
        }

        return services.BuildServiceProvider();
    }

    [Fact]
    public void Resolve_from_ambient_services_self()
    {
        var container = Test.CreateTestServiceInfoCollectionBuilder().BuildServiceProvider();
        var serviceInfoCollection = container.GetRequiredService(typeof(IServiceInfoCollection));
        var sameServiceInfoCollection = container.GetRequiredService(typeof(IServiceInfoCollection));

        Assert.NotNull(serviceInfoCollection);
        serviceInfoCollection.Should().BeSameAs(sameServiceInfoCollection);
    }

    [Fact]
    public void Resolve_from_ambient_services_instance()
    {
        var appServices = new ServiceInfoCollection();
        var container = Test.CreateTestServiceInfoCollectionBuilder(appServices).BuildServiceProvider();
        var containerExport = container.GetRequiredService(typeof(DependencyInjectionExtensionsIntegrationTest.ILogManager));
        var ambientExport = container.GetRequiredService(typeof(DependencyInjectionExtensionsIntegrationTest.ILogManager));

        Assert.NotNull(containerExport);
        Assert.Same(ambientExport, containerExport);
    }

    [Fact]
    public void Resolve_from_ambient_services_instance_type()
    {
        var appServices = new ServiceInfoCollection()
            .Add<ITestTypeLoader, TestTypeLoader>(b => b.Singleton());
        var container = Test.CreateTestServiceInfoCollectionBuilder(appServices).BuildServiceProvider();
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
        var builder = Test.CreateTestServiceInfoCollectionBuilder()
            .AddProviders(new MultiFilterServiceInfoProvider());
        var container = builder.BuildServiceProvider();

        var filters = (IEnumerable<object>)container.GetRequiredService(typeof(IEnumerable<IFilter>));

        Assert.Equal(3, filters.Count());
        Assert.True(filters.OfType<OneFilter>().Any());
        Assert.True(filters.OfType<TwoFilter>().Any());
    }

    [Fact]
    public void GetService_various_same_contract_registrations()
    {
        var container = Test.CreateTestServiceInfoCollectionBuilder()
            .AddProviders(new MultiFilterServiceInfoProvider())
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
        var appServices = new ServiceInfoCollection();
        appServices.Add(new ServiceInfo(
            typeof(ScopeExportedClass),
            typeof(ScopeExportedClass),
            AppServiceLifetime.Scoped));
        var builder = Test.CreateTestServiceInfoCollectionBuilder(appServices);
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
        var appServices = new ServiceInfoCollection();
        var service = Substitute.For<IAsyncInitializable>();
        appServices.Add(typeof(IAsyncInitializable), service);

        var container = Test.CreateTestServiceInfoCollectionBuilder(appServices).BuildServiceProvider();

        var actualService = container.GetRequiredService<IAsyncInitializable>();
        Assert.Same(service, actualService);
    }

    [Fact]
    public void Resolve_ambient_services_factory()
    {
        var appServices = new ServiceInfoCollection();
        appServices.Add(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>(), b => b.Transient());

        var container = Test.CreateTestServiceInfoCollectionBuilder(appServices).BuildServiceProvider();

        var service1 = container.GetRequiredService<IAsyncInitializable>();
        var service2 = container.GetRequiredService<IAsyncInitializable>();
        Assert.NotSame(service1, service2);
    }

    [Fact]
    public void Resolve_ambient_services_not_available_after_first_failed_request()
    {
        var appServices = new ServiceInfoCollection();
        var container = Test.CreateTestServiceInfoCollectionBuilder(appServices).BuildServiceProvider();

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
        public IFactory<ExportedClass> ExportedClassFactory { get; set; }
    }

    public class ExportedClassImplicitManyFactoryImporter
    {
        public ICollection<IFactory<ExportedClass>> ExportedClassFactoryCollection { get; set; }
        public IList<IFactory<ExportedClass>> ExportedClassFactoryList { get; set; }
        public IEnumerable<IFactory<ExportedClass>> ExportedClassFactoryEnumerable { get; set; }
        public IFactory<ExportedClass>[] ExportedClassFactoryArray { get; set; }
    }

    public class TestMetadata : ServiceMetadata
    {
        public TestMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
        }
    }

    public class MultiFilterServiceInfoProvider : IServiceInfoProvider
    {
        public IEnumerable<ServiceDeclarationInfo> GetServiceInfos()
        {
            yield return new ServiceDeclarationInfo(typeof(IFilter), new ServiceInfo(typeof(IFilter), typeof(OneFilter), AppServiceLifetime.Singleton));
            yield return new ServiceDeclarationInfo(typeof(IFilter), new ServiceInfo(typeof(IFilter), typeof(TwoFilter), AppServiceLifetime.Transient));
            yield return new ServiceDeclarationInfo(typeof(IFilter), new ServiceInfo(typeof(IFilter), Substitute.For<IFilter>()));
        }
    }

    public interface IFilter { }

    public class OneFilter : IFilter { }

    public class TwoFilter : IFilter { }

    public interface IAsyncInitializable {}

    public interface ITestTypeLoader {}

    public class TestTypeLoader : ITestTypeLoader {}
}
