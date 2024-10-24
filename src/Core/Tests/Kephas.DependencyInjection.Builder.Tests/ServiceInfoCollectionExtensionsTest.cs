// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Reflection;
using Xunit;

namespace Kephas.Tests;

/// <summary>
/// Test class for <see cref="ServiceInfoCollectionExtensions"/>.
/// </summary>
public class ServiceInfoCollectionExtensionsTest
{
    [Fact]
    public void TryAdd_TContract_existing_instance()
    {
        var appServices = new ServiceInfoCollection();
        var service = new Service();
        appServices.TryAdd<IService>(new Service());
        appServices.TryAdd<IService>(service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.NotSame(service, serviceInfo.Instance);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_TContract_non_existing_instance()
    {
        var appServices = new ServiceInfoCollection();
        var service = new Service();
        appServices.TryAdd<IService>(service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.Same(service, serviceInfo.Instance);
        Assert.Equal("value", serviceInfo.Metadata?["test"]);
        Assert.Equal(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Fact]
    public void TryAdd_existing_instance()
    {
        var appServices = new ServiceInfoCollection();
        var service = new Service();
        appServices.TryAdd(typeof(IService), new Service());
        appServices.TryAdd(typeof(IService), service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.NotSame(service, serviceInfo.Instance);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_non_existing_instance()
    {
        var appServices = new ServiceInfoCollection();
        var service = new Service();
        appServices.TryAdd(typeof(IService), service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.Same(service, serviceInfo.Instance);
        Assert.Equal("value", serviceInfo.Metadata?["test"]);
        Assert.Equal(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Fact]
    public void TryAdd_TContract_TService_existing_type()
    {
        var appServices = new ServiceInfoCollection();
        appServices.TryAdd<IService, Service>();
        appServices.TryAdd<IService, OtherService>(b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.ImplementationType);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_TContract_TService_non_existing_type()
    {
        var appServices = new ServiceInfoCollection();
        appServices.TryAdd<IService, Service>(b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.ImplementationType);
        Assert.Equal("value", serviceInfo.Metadata?["test"]);
        Assert.Equal(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Fact]
    public void TryAdd_TContract_existing_type()
    {
        var appServices = new ServiceInfoCollection();
        appServices.TryAdd<IService>(typeof(Service));
        appServices.TryAdd<IService>(typeof(OtherService), b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.ImplementationType);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_TContract_non_existing_type()
    {
        var appServices = new ServiceInfoCollection();
        appServices.TryAdd<IService, Service>(b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.ImplementationType);
        Assert.Equal("value", serviceInfo.Metadata?["test"]);
        Assert.Equal(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Fact]
    public void TryAdd_existing_type()
    {
        var appServices = new ServiceInfoCollection();
        appServices.TryAdd(typeof(IService), typeof(Service));
        appServices.TryAdd(typeof(IService), typeof(OtherService), b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.ImplementationType);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    [Fact]
    public void TryAdd_non_existing_type()
    {
        var appServices = new ServiceInfoCollection();
        appServices.TryAdd(typeof(IService), typeof(Service));

        var serviceInfo = appServices.Single(r => r.ServiceType == typeof(IService));
        Assert.Same(typeof(Service), serviceInfo.ImplementationType);
        Assert.Null(serviceInfo.Metadata?["test"]);
    }

    private interface IService {}

    private class Service : IService {}

    private class OtherService : IService {}
}
