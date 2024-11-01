// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServicesSourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Analyzers.DependencyInjection;
using Xunit;

namespace Kephas.Analyzers.Tests.DependencyInjection;

public class DeclareServicesSourceGeneratorTest : SourceGeneratorTestBase<DeclareServicesSourceGenerator>
{
    [Fact]
    public void Generate_DeclareServices()
    {
        var sourceCode =
            """
            using Kephas.DependencyInjection;
            
            namespace Kephas.Sample;

            [TransientService]
            public interface IMetaService;
            [ScopedService]
            public interface IScopedMetaService;
            [SingletonService]
            public interface ISingletonMetaService;
            
            """;

        var result = GetRunResult(sourceCode);

        // Assert the driver use the cached result from AssemblyName and Syntax
        Assert.Collection(
            result.GeneratedSources,
            source =>
            {
                source.HintName.Should().Be("DeclaredServices.g.cs");
                var stringSource = source.SourceText.ToString();
                stringSource.Should().Contain(
                    """
                    #nullable enable
                    
                    using System;
                    using System.Collections.Generic;
                    using System.Diagnostics.CodeAnalysis;
                    using System.Reflection;
                    
                    using Kephas.DependencyInjection;
                    using Kephas.DependencyInjection.ServiceProviders;
                    
                    [assembly: DeclareServices<Kephas.DependencyInjection.ServiceProviders.Generated.DeclareServices_DeclareServicesSourceGeneratorTest>()]
                    
                    namespace Kephas.DependencyInjection.ServiceProviders.Generated
                    {
                       [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
                       internal partial class DeclareServices_DeclareServicesSourceGeneratorTest: IServiceInfoProvider
                       {
                           IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
                           {
                                yield return new ServiceDeclaration(typeof(IMetaService), null);
                                yield return new ServiceDeclaration(typeof(IScopedMetaService), null);
                                yield return new ServiceDeclaration(typeof(ISingletonMetaService), null);
                           }
                    
                           IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
                           {
                           }
                       }
                    }
                    
                    """);
            });
    }
}