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

public class FactoryServiceEnumerableTest()
    : IntegrationTestBase<FactoryServiceEnumerableTest>(includeTestAssembly: true)
{
    [Fact]
    public void FactoryServiceEnumerable_with_metadata()
    {
        var enumerable = ServiceProvider.GetRequiredService<IFactoryEnumerable<IFilter, ServiceMetadata>>();
        enumerable.Should().BeOfType<FactoryServiceEnumerable<IFilter, ServiceMetadata>>();
        enumerable.Should().Satisfy(
            e => 
                e.Metadata.ServiceKey == "One" &&
                e.Metadata.ImplementationType == typeof(OneFilter),
            e =>
                e.Metadata.ServiceKey == "Two" &&
                e.Metadata.ImplementationType == typeof(TwoFilter));
    }
    
    [Fact]
    public void FactoryServiceEnumerable_no_metadata()
    {
        var enumerable = ServiceProvider.GetRequiredService<IFactoryEnumerable<IFilter>>();
        enumerable.Should().BeOfType<FactoryServiceEnumerable<IFilter>>();
        enumerable.Should().Satisfy(
            e => 
                e.Metadata.ServiceKey == "One" &&
                e.Metadata.ImplementationType == typeof(OneFilter),
            e =>
                e.Metadata.ServiceKey == "Two" &&
                e.Metadata.ImplementationType == typeof(TwoFilter));
    }
}