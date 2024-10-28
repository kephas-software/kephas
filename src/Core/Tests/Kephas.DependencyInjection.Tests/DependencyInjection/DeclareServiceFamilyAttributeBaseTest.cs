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
using Xunit;

namespace Kephas.Tests.DependencyInjection;

/// <summary>
/// An application service contract attribute test.
/// </summary>
public class DeclareServiceFamilyAttributeBaseTest
{
    [Fact]
    public void DeclareSingletonServiceFamilyAttribute()
    {
        var attr = new DeclareSingletonServiceFamilyAttribute<string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Singleton);
        attr.ServiceType.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareSingletonServiceFamilyAttribute_Implementation()
    {
        IServiceInfo attr = new DeclareSingletonServiceFamilyAttribute<IComparable, string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Singleton);
        attr.ServiceDeclarationType.Should().Be(typeof(IComparable));
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.MetadataType.Should().Be(typeof(string));
        attr.ImplementationType.Should().Be(null);
        attr.InstancingStrategy.Should().Be(null);

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareScopedServiceFamilyAttribute()
    {
        var attr = new DeclareScopedServiceFamilyAttribute<string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Scoped);
        attr.ServiceType.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareScopedServiceFamilyAttribute_Implementation()
    {
        IServiceInfo attr = new DeclareScopedServiceFamilyAttribute<IComparable, string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Scoped);
        attr.ServiceDeclarationType.Should().Be(typeof(IComparable));
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.MetadataType.Should().Be(typeof(string));
        attr.ImplementationType.Should().Be(null);
        attr.InstancingStrategy.Should().Be(null);

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareTransientServiceFamilyAttribute()
    {
        var attr = new DeclareTransientServiceFamilyAttribute<string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Transient);
        attr.ServiceType.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareTransientServiceFamilyAttribute_Implementation()
    {
        IServiceInfo attr = new DeclareTransientServiceFamilyAttribute<IComparable, string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Transient);
        attr.ServiceDeclarationType.Should().Be(typeof(IComparable));
        attr.ServiceType.Should().Be(typeof(IComparable));
        attr.MetadataType.Should().Be(typeof(string));
        attr.ImplementationType.Should().Be(null);
        attr.InstancingStrategy.Should().Be(null);

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }
}