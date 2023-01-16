// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Text;

using Kephas.Services;
using Kephas.Testing;
using NUnit.Framework;

/// <summary>
/// Test class for <see cref="AppServiceCollectionExtensions"/>.
/// </summary>
[TestFixture]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class AppServiceCollectionExtensionsTest : TestBase
{
    [Test]
    public void Constructor_register_ambient_services()
    {
        var appServices = this.CreateAppServices();

        Assert.AreSame(appServices, appServices.GetServiceInstance(typeof(IAppServiceCollection)));
    }

    [Test]
    public void TryAdd_TContract_existing_instance()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd(new Service());
        appServices.TryAdd(service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreNotSame(service, serviceInfo.Instance);
        Assert.IsNull(serviceInfo.Metadata?["test"]);
    }

    [Test]
    public void TryAdd_TContract_non_existing_instance()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd(service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreSame(service, serviceInfo.Instance);
        Assert.AreEqual("value", serviceInfo.Metadata?["test"]);
        Assert.AreEqual(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Test]
    public void TryAdd_existing_instance()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd(typeof(IService), new Service());
        appServices.TryAdd(typeof(IService), service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreNotSame(service, serviceInfo.Instance);
        Assert.IsNull(serviceInfo.Metadata?["test"]);
    }

    [Test]
    public void TryAdd_non_existing_instance()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd(typeof(IService), service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreSame(service, serviceInfo.Instance);
        Assert.AreEqual("value", serviceInfo.Metadata?["test"]);
        Assert.AreEqual(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Test]
    public void TryAdd_TContract_TService_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd<IService, Service>();
        appServices.TryAdd<IService, OtherService>(b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreSame(typeof(Service), serviceInfo.InstanceType);
        Assert.IsNull(serviceInfo.Metadata?["test"]);
    }

    [Test]
    public void TryAdd_TContract_TService_non_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd<IService, Service>(b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreSame(typeof(Service), serviceInfo.InstanceType);
        Assert.AreEqual("value", serviceInfo.Metadata?["test"]);
        Assert.AreEqual(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Test]
    public void TryAdd_TContract_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd<IService>(typeof(Service));
        appServices.TryAdd<IService>(typeof(OtherService), b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreSame(typeof(Service), serviceInfo.InstanceType);
        Assert.IsNull(serviceInfo.Metadata?["test"]);
    }

    [Test]
    public void TryAdd_TContract_non_existing_type()
    {
        var appServices = new AppServiceCollection();
        appServices.TryAdd<IService, Service>(b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreSame(typeof(Service), serviceInfo.InstanceType);
        Assert.AreEqual("value", serviceInfo.Metadata?["test"]);
        Assert.AreEqual(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    [Test]
    public void TryAdd_existing_type()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd(typeof(IService), type);
        appServices.TryAdd(typeof(StringBuilder), service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreNotSame(service, serviceInfo.Instance);
        Assert.IsNull(serviceInfo.Metadata?["test"]);
    }

    [Test]
    public void TryAdd_non_existing_type()
    {
        var appServices = new AppServiceCollection();
        var service = new Service();
        appServices.TryAdd(typeof(StringBuilder), service, b => b.AddMetadata("test", "value"));

        var serviceInfo = appServices.Single(r => r.ContractType == typeof(IService));
        Assert.AreSame(service, serviceInfo.Instance);
        Assert.AreEqual("value", serviceInfo.Metadata?["test"]);
        Assert.AreEqual(AppServiceLifetime.Singleton, serviceInfo.Lifetime);
    }

    private interface IService {}

    private class Service : IService {}

    private class OtherService : IService {}
}