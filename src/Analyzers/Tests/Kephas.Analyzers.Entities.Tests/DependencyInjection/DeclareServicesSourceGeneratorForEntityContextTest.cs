// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServicesSourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Analyzers.DependencyInjection;
using Kephas.Entities;
using Kephas.Testing.Analyzers;
using Xunit;

namespace Kephas.Analyzers.Entities.Tests.DependencyInjection;

public class DeclareServicesSourceGeneratorForEntityContextTest : SourceGeneratorTestBase<DeclareServicesSourceGenerator>
{
    [Fact]
    public void Generate_DeclareServices_for_entity_context()
    {
        var sourceCode =
            """
            
            using System.CodeDom.Compiler;
            using Microsoft.EntityFrameworkCore;
            using Kephas.DependencyInjection;
            using Kephas.DependencyInjection.Metadata;
            using Kephas.Entities;
            using Kephas.Entities.Builder;
            
            namespace Kephas.Entities.EntityContexts.Generated;
            
            [Priority(Priority.Normal)]
            [ConnectionName("SqliteCrm")]
            [GeneratedCode("EntityContextSourceGenerator", "1.0.0.0")]
            public partial class SqliteCrmEntityContext : EntityContextBase<SqliteCrmEntityContext>
            {
                public SqliteCrmEntityContext(DbContextOptions<SqliteCrmEntityContext> options, IDbContextBuilder builder)
                    : base(options, builder)
                {
                }
            }
            """;

        var result = GetRunResult(sourceCode,[
            typeof(IEntityContext).Assembly,
            typeof(SqlWriteRepository).Assembly
        ]);

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
                    
                    [assembly: DeclareServices<Kephas.DependencyInjection.ServiceProviders.Generated.DeclareServices_DeclareServicesSourceGeneratorForEntityContextTest>()]
                    
                    namespace Kephas.DependencyInjection.ServiceProviders.Generated
                    {
                       [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
                       internal partial class DeclareServices_DeclareServicesSourceGeneratorForEntityContextTest: IServiceInfoProvider
                       {
                           IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
                           {
                                yield break;
                           }
                    
                           IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
                           {
                                yield return new ServiceImplementationDeclaration(typeof(SqliteCrmEntityContext), typeof(Kephas.Entities.IEntityContext<Kephas.Entities.EntityContexts.Generated.SqliteCrmEntityContext>));
                           }
                       }
                    }
                    
                    """);
            });
    }
}