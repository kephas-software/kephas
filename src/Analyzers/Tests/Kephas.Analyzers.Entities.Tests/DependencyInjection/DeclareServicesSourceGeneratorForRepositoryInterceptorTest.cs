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

public class DeclareServicesSourceGeneratorForRepositoryInterceptorTest : SourceGeneratorTestBase<DeclareServicesSourceGenerator>
{
    [Fact]
    public void Generate_DeclareServices_for_repository_interceptor()
    {
        var sourceCode =
            """
            using Kephas.DependencyInjection.Metadata;
            using Kephas.Validation;
            using Microsoft.EntityFrameworkCore;
            
            namespace Kephas.Entities.Interceptors;
            
            [Priority(Priority.Low)]
            public class ValidationSaveChangesRepositoryInterceptor(IValidationService validationService)
                : ISaveChangesRepositoryInterceptor
            {
                public async Task<int> HandleAsync(IWriteRepository repository, IEntityContext entityContext, Func<Task<int>> next,
                    CancellationToken cancellationToken)
                {
                    var dbContext = (DbContext)entityContext;
                    foreach (var entry in dbContext.ChangeTracker.Entries())
                    {
                        await validationService.EnsureValidAsync(entry.Entity, cancellationToken);
                    }
                    
                    return await next();
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
                    
                    [assembly: DeclareServices<DeclareServicesSourceGeneratorForRepositoryInterceptorTest.ServiceProviders.Generated.DeclareServices_DeclareServicesSourceGeneratorForRepositoryInterceptorTest>()]
                    
                    namespace DeclareServicesSourceGeneratorForRepositoryInterceptorTest.ServiceProviders.Generated
                    {
                       [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
                       internal partial class DeclareServices_DeclareServicesSourceGeneratorForRepositoryInterceptorTest: IServiceInfoProvider
                       {
                           IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
                           {
                                yield break;
                           }
                    
                           IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
                           {
                                yield return new ServiceImplementationDeclaration(typeof(ValidationSaveChangesRepositoryInterceptor), typeof(Kephas.Entities.Interceptors.IRepositoryInterceptor<Kephas.Entities.Interceptors.ISaveChangesOperation, System.Int32>));
                           }
                       }
                    }
                    
                    """);
            });
    }
}