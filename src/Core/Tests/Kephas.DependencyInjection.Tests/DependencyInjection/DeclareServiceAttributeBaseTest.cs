// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceContractAttributeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application service contract attribute test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using Xunit;

namespace Kephas.Tests.DependencyInjection;

/// <summary>
/// An application service contract attribute test.
/// </summary>
public class DeclareServiceAttributeBaseTest
{
    [Fact]
    public void DeclareSingletonServiceAttribute_non_generic()
    {
        var attr = new DeclareSingletonServiceAttribute(typeof(IComparable), typeof(string));
        attr.Lifetime.Should().Be(AppServiceLifetime.Singleton);
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.InstancingStrategy.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();

        CheckIServiceInfoProviderInterface((IServiceInfoProvider)attr);
    }

    [Fact]
    public void DeclareSingletonServiceAttribute_generic()
    {
        var attr = new DeclareSingletonServiceAttribute<IComparable, string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Singleton);
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.InstancingStrategy.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();

        CheckIServiceInfoProviderInterface((IServiceInfoProvider)attr);
    }

    [Fact]
    public void DeclareSingletonServiceAttribute_Implementation()
    {
        IServiceInfo attr = new DeclareSingletonServiceAttribute<IComparable, string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Singleton);
        attr.ServiceDeclarationType.Should().Be(typeof(IComparable));
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.InstancingStrategy.Should().Be(typeof(string));
        attr.ImplementationType.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();

        CheckIServiceInfoProviderInterface((IServiceInfoProvider)attr);
    }

    [Fact]
    public void DeclareScopedServiceAttribute_non_generic()
    {
        var attr = new DeclareScopedServiceAttribute(typeof(IComparable), typeof(string));
        attr.Lifetime.Should().Be(AppServiceLifetime.Scoped);
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.InstancingStrategy.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();

        CheckIServiceInfoProviderInterface((IServiceInfoProvider)attr);
    }

    [Fact]
    public void DeclareScopedServiceAttribute_generic()
    {
        var attr = new DeclareScopedServiceAttribute<IComparable, string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Scoped);
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.InstancingStrategy.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();

        CheckIServiceInfoProviderInterface((IServiceInfoProvider)attr);
    }

    [Fact]
    public void DeclareScopedServiceAttribute_Implementation()
    {
        IServiceInfo attr = new DeclareScopedServiceAttribute<IComparable, string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Scoped);
        attr.ServiceDeclarationType.Should().Be(typeof(IComparable));
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.InstancingStrategy.Should().Be(typeof(string));
        attr.ImplementationType.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();

        CheckIServiceInfoProviderInterface((IServiceInfoProvider)attr);
    }

    [Fact]
    public void DeclareTransientServiceAttribute_non_generic()
    {
        var attr = new DeclareTransientServiceAttribute(typeof(IComparable), typeof(string));
        attr.Lifetime.Should().Be(AppServiceLifetime.Transient);
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.InstancingStrategy.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();

        CheckIServiceInfoProviderInterface((IServiceInfoProvider)attr);
    }

    [Fact]
    public void DeclareTransientServiceAttribute_generic()
    {
        var attr = new DeclareTransientServiceAttribute<IComparable, string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Transient);
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.InstancingStrategy.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();

        CheckIServiceInfoProviderInterface((IServiceInfoProvider)attr);
    }

    [Fact]
    public void DeclareTransientServiceAttribute_Implementation()
    {
        IServiceInfo attr = new DeclareTransientServiceAttribute<IComparable, string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Transient);
        attr.ServiceDeclarationType.Should().Be(typeof(IComparable));
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.InstancingStrategy.Should().Be(typeof(string));
        attr.ImplementationType.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();

        CheckIServiceInfoProviderInterface((IServiceInfoProvider)attr);
    }

    private void CheckIServiceInfoProviderInterface(IServiceInfoProvider attr)
    {
        attr.GetServiceDeclarationInfos().Should().HaveCount(1);
        attr.GetServiceDeclarationInfos().Should().Satisfy(
            e => e.ServiceInfo.InstancingStrategy == null && e.ServiceInfo.ServiceType == e.ServiceDeclarationType);
    }
}