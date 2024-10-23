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
using Kephas.DependencyInjection.Metadata;
using Xunit;

namespace Kephas.Tests.DependencyInjection;

/// <summary>
/// An application service contract attribute test.
/// </summary>
public class DeclareServiceAttributeBaseTest
{
    [Fact]
    public void DeclareSingletonServiceAttribute()
    {
        var attr = new DeclareSingletonServiceAttribute<string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Singleton);
        attr.ServiceType.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareSingletonServiceAttribute_Metadata()
    {
        var attr = new DeclareSingletonServiceAttribute<string, ServiceMetadata>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Singleton);
        attr.ServiceType.Should().Be(typeof(string));
        attr.MetadataType.Should().Be(typeof(ServiceMetadata));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareScopedServiceAttribute()
    {
        var attr = new DeclareScopedServiceAttribute<string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Scoped);
        attr.ServiceType.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareScopedServiceAttribute_Metadata()
    {
        var attr = new DeclareScopedServiceAttribute<string, ServiceMetadata>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Scoped);
        attr.ServiceType.Should().Be(typeof(string));
        attr.MetadataType.Should().Be(typeof(ServiceMetadata));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareTransientServiceAttribute()
    {
        var attr = new DeclareTransientServiceAttribute<string>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Transient);
        attr.ServiceType.Should().Be(typeof(string));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }

    [Fact]
    public void DeclareTransientServiceAttribute_Metadata()
    {
        var attr = new DeclareTransientServiceAttribute<string, ServiceMetadata>();
        attr.Lifetime.Should().Be(AppServiceLifetime.Transient);
        attr.ServiceType.Should().Be(typeof(string));
        attr.MetadataType.Should().Be(typeof(ServiceMetadata));

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Assembly);
        attrUsage!.AllowMultiple.Should().BeTrue();
    }
}