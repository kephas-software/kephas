// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainBaseIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection;
using Kephas.Ontology;
using Kephas.Testing;
using Kephas.Tests.TestServices;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kephas.Tests.Ontology;

public class DomainBaseIntegrationTest() : IntegrationTestBase<DomainBaseIntegrationTest>(includeTestAssembly: true)
{
    [Theory]
    [InlineData(typeof(CoreDomain), "Core")]
    [InlineData(typeof(CrmDomain), "Crm")]
    [InlineData(typeof(ContractingDomain), "Contracting")]
    [InlineData(typeof(BIDomain), "BI")]
    public void Name_RetrievedFromMetadata(Type domainType, string expectedName)
    {
        var domain = ServiceProvider.GetRequiredService<ILazyEnumerable<IDomain, DomainMetadata>>()
            .GetRequiredService(m => m.ImplementationType == domainType);
        domain.Name.Should().Be(expectedName);
    }
    
    [Theory]
    [InlineData(typeof(CoreDomain), new Type[0])]
    [InlineData(typeof(CrmDomain), new[] { typeof(CoreDomain) })]
    [InlineData(typeof(ContractingDomain), new[] { typeof(CrmDomain), typeof(CoreDomain) })]
    [InlineData(typeof(BIDomain), new[] { typeof(ContractingDomain), typeof(CrmDomain), typeof(CoreDomain) })]
    public void Dependencies_RetrievedFromMetadata(Type domainType, Type[] expectedDependencies)
    {
        var domain = ServiceProvider.GetRequiredService<ILazyEnumerable<IDomain, DomainMetadata>>()
            .GetRequiredService(m => m.ImplementationType == domainType);
        domain.Dependencies.Select(d => d.GetType()).Should().BeEquivalentTo(expectedDependencies);
    }
}