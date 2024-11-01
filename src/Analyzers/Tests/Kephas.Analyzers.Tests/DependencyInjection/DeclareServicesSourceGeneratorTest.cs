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
            using System;
            using Kephas.DependencyInjection;
            
            namespace Kephas.Sample;

            [TransientService]
            public interface IMetaService;
            [ScopedService]
            public interface IScopedMetaService;
            [SingletonService]
            public interface ISingletonMetaService;

            [TransientService]
            public interface IMetaService<T>;
            [TransientService]
            public interface IMetaService<T,V>;
            
            public class MetaService : IMetaService {}
            public class ScopedMetaService : IScopedMetaService {}
            public class SingletonMetaService : ISingletonMetaService {}
            
            public class MetaService<T> : IComparable, IMetaService<T> {}
            public class MetaService<T,V> : IComparable, IMetaService<T,V> {}
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
                                yield return new ServiceDeclaration(typeof(IMetaService<>), null);
                                yield return new ServiceDeclaration(typeof(IMetaService<,>), null);
                           }
                    
                           IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
                           {
                                yield return new ServiceImplementationDeclaration(typeof(MetaService), typeof(Kephas.Sample.IMetaService));
                                yield return new ServiceImplementationDeclaration(typeof(ScopedMetaService), typeof(Kephas.Sample.IScopedMetaService));
                                yield return new ServiceImplementationDeclaration(typeof(SingletonMetaService), typeof(Kephas.Sample.ISingletonMetaService));
                                yield return new ServiceImplementationDeclaration(typeof(MetaService<>), typeof(Kephas.Sample.IMetaService<>));
                                yield return new ServiceImplementationDeclaration(typeof(MetaService<,>), typeof(Kephas.Sample.IMetaService<, >));
                           }
                       }
                    }
                    
                    """);
            });
    }
}