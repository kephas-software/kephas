// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceInfoCollectionBuilderIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Reflection;
using Kephas.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Tests.DependencyInjection.Builder;

public class ServiceInfoCollectionBuilderIntegrationTest : IntegrationTestBase<ServiceInfoCollectionBuilderIntegrationTest>
{
    [Fact]
    public void ShouldNotDuplicateServices()
    {
        var serviceInfos = ServiceProvider.GetRequiredService<IServiceInfoCollection>();
        serviceInfos.Where(si => si.ServiceType == typeof(TimeProvider)).Should().HaveCount(1);
        serviceInfos.Where(si => si.ServiceType == typeof(ILazyEnumerable<>)).Should().HaveCount(1);
        serviceInfos.Where(si => si.ServiceType == typeof(ILazyEnumerable<,>)).Should().HaveCount(1);
        serviceInfos.Where(si => si.ServiceType == typeof(IFactoryEnumerable<>)).Should().HaveCount(1);
        serviceInfos.Where(si => si.ServiceType == typeof(IFactoryEnumerable<,>)).Should().HaveCount(1);
    }
}