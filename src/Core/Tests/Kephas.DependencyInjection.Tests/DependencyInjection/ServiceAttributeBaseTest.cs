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
using Xunit;

namespace Kephas.Tests.DependencyInjection;

/// <summary>
/// An application service contract attribute test.
/// </summary>
public class ServiceAttributeBaseTest
{
    [Fact]
    public void SingletonServiceAttribute()
    {
        var attr = new SingletonServiceAttribute();
        attr.Lifetime.Should().Be(AppServiceLifetime.Singleton);

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Interface | AttributeTargets.Class);
        attrUsage!.AllowMultiple.Should().BeFalse();
    }

    [Fact]
    public void ScopedServiceAttribute()
    {
        var attr = new ScopedServiceAttribute();
        attr.Lifetime.Should().Be(AppServiceLifetime.Scoped);

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Interface | AttributeTargets.Class);
        attrUsage!.AllowMultiple.Should().BeFalse();
    }

    [Fact]
    public void TransientServiceAttribute()
    {
        var attr = new TransientServiceAttribute();
        attr.Lifetime.Should().Be(AppServiceLifetime.Transient);

        var attrUsage = attr.GetType().GetCustomAttribute<AttributeUsageAttribute>();
        attrUsage!.ValidOn.Should().Be(AttributeTargets.Interface | AttributeTargets.Class);
        attrUsage!.AllowMultiple.Should().BeFalse();
    }
}