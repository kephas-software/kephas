// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryServiceEnumerableTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.Extensions.DependencyInjection;
using Kephas.Extensions.Tests.TestServices;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Extensions.Tests.DependencyInjection;

public class LazyServiceEnumerableTest()
    : IntegrationTestBase<LazyServiceEnumerableTest>(includeTestAssembly: true)
{
    [Fact]
    public void LazyServiceEnumerable_with_metadata()
    {
        var enumerable = ServiceProvider.GetRequiredService<ILazyEnumerable<IFilter, ServiceMetadata>>();
        enumerable.Should().BeOfType<LazyServiceEnumerable<IFilter, ServiceMetadata>>();
        enumerable.Should().Satisfy(
            e => 
                e.Metadata.ServiceKey == "One" &&
                e.Metadata.ImplementationType == typeof(OneFilter),
            e =>
                e.Metadata.ServiceKey == "Two" &&
                e.Metadata.ImplementationType == typeof(TwoFilter));
    }
    
    [Fact]
    public void LazyServiceEnumerable_no_metadata()
    {
        var enumerable = ServiceProvider.GetRequiredService<ILazyEnumerable<IFilter>>();
        enumerable.Should().BeOfType<LazyServiceEnumerable<IFilter>>();
        enumerable.Should().Satisfy(
            e => 
                e.Metadata.ServiceKey == "One" &&
                e.Metadata.ImplementationType == typeof(OneFilter),
            e =>
                e.Metadata.ServiceKey == "Two" &&
                e.Metadata.ImplementationType == typeof(TwoFilter));
    }
}