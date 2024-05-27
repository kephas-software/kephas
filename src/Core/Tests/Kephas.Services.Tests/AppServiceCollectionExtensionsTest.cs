// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Tests;

/// <summary>
/// Test class for <see cref="AppServiceCollectionExtensions"/>.
/// </summary>
public class AppServiceCollectionExtensionsTest
{
    [Fact]
    public void Constructor_register_ambient_services()
    {
        var appServices = new AppServiceCollection();

        Assert.Same(appServices, appServices.GetServiceInstance(typeof(IAppServiceCollection)));
    }

    [Fact]
    public void TryAdd_TContract_existing_instance()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd<IService>(new Service());
        appServices.TryAdd<IService>(service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.NotSame(service, serviceInfo.Instance);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_TContract_non_existing_instance()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd<IService>(service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.Same(service, serviceInfo.Instance);
        Assert.Equal("value", serviceInfo.Metadata?["test"]);
        Assert.Equal(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Fact]
    public void TryAdd_existing_instance()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd(typeof(IService), new Service());
        appServices.TryAdd(typeof(IService), service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.NotSame(service, serviceInfo.Instance);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_non_existing_instance()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd(typeof(IService), service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.Same(service, serviceInfo.Instance);
        Assert.Equal("value", serviceInfo.Metadata?["test"]);
        Assert.Equal(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Fact]
    public void TryAdd_TContract_TService_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd<IService, Service>();
        appServices.TryAdd<IService, OtherService>(b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.InstanceType);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_TContract_TService_non_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd<IService, Service>(b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.InstanceType);
        Assert.Equal("value", serviceInfo.Metadata?["test"]);
        Assert.Equal(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Fact]
    public void TryAdd_TContract_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd<IService>(typeof(Service));
        appServices.TryAdd<IService>(typeof(OtherService), b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.InstanceType);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_TContract_non_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd<IService, Service>(b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.InstanceType);
        Assert.Equal("value", serviceInfo.Metadata?["test"]);
        Assert.Equal(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Fact]
    public void TryAdd_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd(typeof(IService), typeof(Service));
        appServices.TryAdd(typeof(IService), typeof(OtherService), b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.InstanceType);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_non_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd(typeof(IService), typeof(Service));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.InstanceType);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    private interface IService {}

    private class Service : IService {}

    private class OtherService : IService {}
}
