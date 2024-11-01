// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection.Reflection;
using Kephas.Extensions.Tests.TestServices;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kephas.Extensions.Tests.DependencyInjection;

/// <summary>
/// Tests for <see cref="ServiceProvider"/>.
/// </summary>
public class ServiceCollectionExtensionsIntegrationTest : TestDependencyInjectionBase
{
    [Fact]
    public void Resolve_AppService_generic_export_with_non_generic_contract()
    {
        var sp = BuildTestServiceProvider(
            typeof(ITestGenericWithNonGenericExport), typeof(ITestGenericWithNonGenericExport<>),
            typeof(TestGenericWithNonGenericExport));

        var service = sp.GetRequiredService<ITestGenericWithNonGenericExport>();
        Assert.IsType<TestGenericWithNonGenericExport>(service);
    }

    [Fact]
    public void ServiceInfoCollectionBuilder_AddTypes_generic_export()
    {
        var sp = BuildTestServiceProvider(
            typeof(ITestGenericExport<>), typeof(TestGenericExport));

        var service = sp.GetRequiredService<ITestGenericExport<string>>();
        Assert.IsType<TestGenericExport>(service);
    }

    [Fact]
    public void ServiceInfoCollectionBuilder_AddTypes_IExportFactory_Success()
    {
        var sp = BuildTestServiceProvider(
            typeof(ITestMultiAppService), typeof(ITestMultiAppServiceConsumer),
            typeof(TestMultiAppService1), typeof(TestMultiAppService2), typeof(TestMultiAppServiceConsumer));

        var export = (TestMultiAppServiceConsumer)sp.GetRequiredService<ITestMultiAppServiceConsumer>();

        Assert.Equal(2, export.Factories.Count());
        Assert.Equal(2, export.MetadataFactories.Count());
    }

    [Fact]
    public void ServiceInfoCollection_multiple_preserve_order_mixed()
    {
        var serviceInfos = new ServiceInfoCollection();
        serviceInfos.Add(typeof(ITestMultiAppService), _ => new TestMultiAppService1(), b => b.Transient());
        serviceInfos.Add(Substitute.For<ITestMultiAppService>());
        serviceInfos.Add(typeof(ITestMultiAppService), typeof(TestMultiAppService2));
        var sp = BuildTestServiceProvider(serviceInfos);

        var services = sp.GetRequiredService<IReadOnlyList<ITestMultiAppService>>();

        services.Should().HaveCount(3);
        services[0].Should().BeOfType<TestMultiAppService1>();
        services[1].GetType().Name.Should().Contain("Proxy");
        services[2].Should().BeOfType<TestMultiAppService2>();
    }

    [Fact]
    public void ServiceInfoCollectionBuilder_AddTypes_enumeration_preserve_order_types()
    {
        var sp = BuildTestServiceProvider(
            typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2));

        var exports = sp.GetRequiredService<IList<ITestMultiAppService>>();

        exports.Should().HaveCount(2);
        exports[0].Should().BeOfType<TestMultiAppService1>();
        exports[1].Should().BeOfType<TestMultiAppService2>();
    }

    [Fact]
    public void ServiceInfoCollectionBuilder_AddTypes_enumeration()
    {
        var sp = BuildTestServiceProvider(
            typeof(ITestMultiAppService), typeof(TestMultiAppService1), typeof(TestMultiAppService2));

        var enumeration1 = sp.GetRequiredService<IEnumerable<ITestMultiAppService>>().ToList();
        var enumeration2 = sp.GetRequiredService<IEnumerable<ITestMultiAppService>>().ToList();

        enumeration1.Should().Equal(enumeration2);
    }

    [Fact]
    public void ServiceInfoCollectionBuilder_AddTypes_with_override()
    {
        var sp = BuildTestServiceProvider(
            typeof(ITestAppService), typeof(TestAppService), typeof(TestOverrideAppService));

        var service = sp.GetRequiredService<ITestAppService>();
        service.Should().BeOfType<TestOverrideAppService>();
    }

    [Fact]
    public void ServiceInfoCollection_AddTransient()
    {
        var appServices = new ServiceInfoCollection()
            .Add<ITestTypeLoader, TestTypeLoader>(b => b.Transient());
        var sp = BuildTestServiceProvider(appServices);
        var instance1 = sp.GetRequiredService(typeof(ITestTypeLoader));
        var instance2 = sp.GetRequiredService(typeof(ITestTypeLoader));

        Assert.NotNull(instance1);
        Assert.NotSame(instance2, instance1);
    }

    [Fact]
    public void ServiceInfoCollection_AddSingleton()
    {
        var serviceInfos = new ServiceInfoCollection()
            .Add(typeof(ExportedClass), typeof(ExportedClass), b => b.Singleton());
        var sp = BuildTestServiceProvider(serviceInfos);
        var enumerable = (IEnumerable<object>)sp.GetRequiredService(typeof(IEnumerable<ExportedClass>));
        var singleton = sp.GetRequiredService<ExportedClass>();

        enumerable.Should().NotBeNull();
        enumerable.Should().HaveCount(1);
        enumerable.Should().Satisfy(e => object.ReferenceEquals(e, singleton));
    }

    [Fact]
    public void ServiceInfoCollectionBuilder_AddProvider()
    {
        var thirdFilter = Substitute.For<IFilter>();
        var sp = Test.BuildEmptyTestServiceProvider(
            builderConfig: b => b
                .AddProviders(new MultiFilterServiceInfoProvider())
                .AddServiceInfos(new ServiceInfo(typeof(IFilter), thirdFilter)));

        var filters = sp.GetRequiredService<IEnumerable<IFilter>>();

        filters.Should().Contain(f => f is OneFilter);
        filters.Should().Contain(f => f is TwoFilter);
        filters.Should().Contain(thirdFilter);
        filters.Should().HaveCount(3);
    }

    [Fact]
    public void ServiceInfoCollection_AddScoped()
    {
        var serviceInfos = new ServiceInfoCollection()
            .Add(typeof(TestScopedService), typeof(TestScopedService), b => b.Scoped());
        var container = BuildTestServiceProvider(serviceInfos);

        using var scope = container.CreateScope();
        using var otherScope = container.CreateScope();
        var scopedInstance1 = scope.ServiceProvider.GetRequiredService<TestScopedService>();
        var scopedInstance2 = scope.ServiceProvider.GetRequiredService<TestScopedService>();

        Assert.Same(scopedInstance1, scopedInstance2);

        var otherScopedInstance = otherScope.ServiceProvider.GetRequiredService<TestScopedService>();
        Assert.NotSame(scopedInstance1, otherScopedInstance);
    }

    [Fact]
    public void ServiceInfoCollection_AddInstance()
    {
        var scopedService = Substitute.For<IAsyncInitializable>();
        var serviceInfos = new ServiceInfoCollection()
            .Add(typeof(IAsyncInitializable), scopedService);

        var container = BuildTestServiceProvider(serviceInfos);

        var actualService = container.GetRequiredService<IAsyncInitializable>();
        Assert.Same(scopedService, actualService);
    }

    [Fact]
    public void ServiceInfoCollection_AddFactory()
    {
        var serviceInfos = new ServiceInfoCollection()
            .Add(typeof(IAsyncInitializable), () => Substitute.For<IAsyncInitializable>(), b => b.Transient());

        var container = BuildTestServiceProvider(serviceInfos);

        var service1 = container.GetRequiredService<IAsyncInitializable>();
        var service2 = container.GetRequiredService<IAsyncInitializable>();
        Assert.NotSame(service1, service2);
    }
}
