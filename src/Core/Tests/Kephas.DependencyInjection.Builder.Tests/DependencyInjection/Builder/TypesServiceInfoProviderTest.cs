// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypesServiceInfoProviderTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Tests.DependencyInjection.TestServices;
using Xunit;

namespace Kephas.Tests.DependencyInjection.Builder;

public class TypesServiceInfoProviderTest
{
    [Fact]
    public void GetServiceDeclarations_ReturnsServiceDeclarations()
    {
        var types = new[]
        {
            typeof(string),
            typeof(ITransientService), typeof(IScopedService), typeof(ISingletonService),
            typeof(IMyService), typeof(MyAbstractClass),
            typeof(IMyLogger<>), typeof(MyLogger<>),
        };
        var provider = new TypesServiceInfoProvider(types, includeInternal: true);

        var declarations = ((IServiceInfoProvider)provider).GetServiceDeclarations();

        declarations.Should().NotBeNull();
        declarations.Should().Satisfy(
            d => d.ServiceDeclarationType == typeof(ITransientService) && d.MetadataType == null,
            d => d.ServiceDeclarationType == typeof(IScopedService) && d.MetadataType == null,
            d => d.ServiceDeclarationType == typeof(ISingletonService) && d.MetadataType == null,
            d => d.ServiceDeclarationType == typeof(IMyService) && d.MetadataType == null,
            d => d.ServiceDeclarationType == typeof(MyAbstractClass) && d.MetadataType == null,
            d => d.ServiceDeclarationType == typeof(IMyLogger<>) && d.MetadataType == null);
    }

    [Fact]
    public void GetImplementationDeclarations_ReturnsImplementationDeclarations()
    {
        var types = new[]
        {
            typeof(string),
            typeof(IMyService), typeof(MyService), typeof(MyPrivateService),
            typeof(IMyLogger<>), typeof(MyLogger<>),
        };
        var provider = new TypesServiceInfoProvider(types, includeInternal: true);

        var declarations = provider.GetImplementationDeclarations(types);

        declarations.Should().NotBeNull();
        declarations.Should().Satisfy(
            d => d.ServiceDeclarationType == typeof(IMyService) && d.ImplementationType == typeof(MyService),
            d => d.ServiceDeclarationType == typeof(IMyLogger<>) && d.ImplementationType == typeof(MyLogger<>));
    }

    private class MyPrivateService : IMyService;
}