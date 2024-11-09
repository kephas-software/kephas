// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSourceGeneratorIntegrationTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using FluentAssertions;
using Kephas.Analyzers.DependencyInjection;
using Kephas.Analyzers.TestAssembly;
using Kephas.DependencyInjection.ServiceProviders;
using Kephas.Testing.Analyzers;
using Xunit;

namespace Kephas.Analyzers.Tests.DependencyInjection;

public class DeclareServicesSourceGeneratorIntegrationTest : SourceGeneratorTestBase<DeclareServicesSourceGenerator>
{
    [Fact]
    public void Execute_generated_DeclareServicesAttribute()
    {
        var testAssembly = typeof(NoService).Assembly;

        var appServicesAttrs = testAssembly.GetCustomAttributes<Attribute>().OfType<IServiceInfoProvider>().ToList();
        appServicesAttrs.Should().ContainSingle();

        var serviceProviderType = appServicesAttrs[0].GetType().GenericTypeArguments[0];
        serviceProviderType.FullName.Should().Be("Kephas.DependencyInjection.ServiceProviders.Generated.DeclareServices_Kephas_Analyzers_TestAssembly");
    }

    [Fact]
    public void Execute_generated_ServiceInfoProvider_class()
    {
        var testAssembly = typeof(NoService).Assembly;

        var appServicesAttr = testAssembly.GetCustomAttributes<Attribute>().OfType<IServiceInfoProvider>().Single();

        var serviceProviderType = appServicesAttr.GetType().GenericTypeArguments[0];
        var provider = Activator.CreateInstance(serviceProviderType)!;
        var typesProvider = (IServiceInfoProvider)provider;
        var services = typesProvider.GetImplementationDeclarations([]);

        var expectedServices = new (Type implementationType, Type serviceDeclarationType)[]
        {
            (typeof(StringService), typeof(IGenericContract<string>)),
            (typeof(IntService), typeof(IGenericContract<int>)),
            (typeof(NonGenericService), typeof(IGenericContractDeclaration<,>)),
            (typeof(OpenGenericService<>), typeof(IOpenGenericContract<>)),
            (typeof(ServiceAndImplementation), typeof(ServiceAndImplementation)),
            (typeof(DerivedService), typeof(ServiceBase)),
            (typeof(SingletonService), typeof(ISingletonServiceContract)),
            (typeof(FileScopedNsSingletonService), typeof(IFileScopedNsSingletonServiceContract)),
        };

        expectedServices.Should()
            .AllSatisfy(service => services.Any(
                s => s.ImplementationType == service.implementationType &&
                     s.ServiceDeclarationType == service.serviceDeclarationType)
                    .Should().BeTrue());

        var serviceInfoProvider = (IServiceInfoProvider)provider;
        serviceInfoProvider.GetServiceDeclarations()
            .Should()
            .BeEquivalentTo(
                new[]
                {
                    new ServiceDeclaration(typeof(ISingletonServiceContract), null),
                    new ServiceDeclaration(typeof(IFileScopedNsSingletonServiceContract), null),
                    new ServiceDeclaration(typeof(ServiceAndImplementation), null),
                    new ServiceDeclaration(typeof(ServiceBase), null),
                    new ServiceDeclaration(typeof(IOpenGenericContract<>), null),
                    new ServiceDeclaration(typeof(IGenericContract<>), null),
                    new ServiceDeclaration(typeof(IGenericContractDeclaration<,>), null),
                    new ServiceDeclaration(typeof(IMetaService), typeof(MetaServiceMetadata)),
                });
    }
}