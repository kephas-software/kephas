// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeclareServicesSourceGeneratorTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentAssertions;
using Kephas.Analyzers.DependencyInjection;
using Kephas.DependencyInjection;
using Kephas.Testing.Analyzers;
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
                    
                    [assembly: DeclareServices<DeclareServicesSourceGeneratorTest.ServiceProviders.Generated.DeclareServices_DeclareServicesSourceGeneratorTest>()]
                    
                    namespace DeclareServicesSourceGeneratorTest.ServiceProviders.Generated
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

    [Fact]
    public void Generate_DeclareServices_ImplementationOfClosedGeneric()
    {
        var sourceCode =
            """
            using System;
            using Kephas.DependencyInjection;
            
            namespace Kephas.Sample;

            public interface ITestGenericWithNonGenericExport;
            
            [TransientService(ServiceType = typeof(ITestGenericWithNonGenericExport))]
            public interface ITestGenericWithNonGenericExport<T> : ITestGenericWithNonGenericExport;
            
            public class TestGenericWithNonGenericExport : ITestGenericWithNonGenericExport<string>;
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
                    
                    [assembly: DeclareServices<DeclareServicesSourceGeneratorTest.ServiceProviders.Generated.DeclareServices_DeclareServicesSourceGeneratorTest>()]
                    
                    namespace DeclareServicesSourceGeneratorTest.ServiceProviders.Generated
                    {
                       [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
                       internal partial class DeclareServices_DeclareServicesSourceGeneratorTest: IServiceInfoProvider
                       {
                           IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
                           {
                                yield return new ServiceDeclaration(typeof(ITestGenericWithNonGenericExport<>), null);
                           }
                    
                           IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
                           {
                                yield return new ServiceImplementationDeclaration(typeof(TestGenericWithNonGenericExport), typeof(Kephas.Sample.ITestGenericWithNonGenericExport<System.String>));
                           }
                       }
                    }
                    
                    """);
            });
    }

    [Fact]
    public void Generate_DeclareServices_MetadataSupport()
    {
        var sourceCode =
            """
            using System;
            using Kephas.DependencyInjection;
            
            namespace Kephas.Sample;

            [SingletonService]
            public interface ITransformer;
            
            public record TransformerMetadata : ServiceMetadata;
            
            internal class InternalTransformer : ITransformer;

            [ExcludeFromDependencyInjection]
            public class ExcludedTransformer : ITransformer;
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
                    
                    [assembly: DeclareServices<DeclareServicesSourceGeneratorTest.ServiceProviders.Generated.DeclareServices_DeclareServicesSourceGeneratorTest>()]
                    
                    namespace DeclareServicesSourceGeneratorTest.ServiceProviders.Generated
                    {
                       [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
                       internal partial class DeclareServices_DeclareServicesSourceGeneratorTest: IServiceInfoProvider
                       {
                           IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
                           {
                                yield return new ServiceDeclaration(typeof(ITransformer), typeof(TransformerMetadata));
                           }
                    
                           IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
                           {
                                yield return new ServiceImplementationDeclaration(typeof(InternalTransformer), typeof(Kephas.Sample.ITransformer));
                           }
                       }
                    }
                    
                    """);
            });
    }

    [Fact]
    public void Generate_DeclareServices_OpenGenericOverride()
    {
        var sourceCode =
            """
            using System;
            using Kephas.DependencyInjection;
            
            namespace Kephas.Sample;

            [SingletonService]
            public interface IOpenGeneric<T>;
            
            internal class OpenGeneric<T> : IOpenGeneric<T>;

            [OverrideBaseImplementation]
            internal class OverrideOpenGeneric<T> : OpenGeneric<T>;
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
                    
                    [assembly: DeclareServices<DeclareServicesSourceGeneratorTest.ServiceProviders.Generated.DeclareServices_DeclareServicesSourceGeneratorTest>()]
                    
                    namespace DeclareServicesSourceGeneratorTest.ServiceProviders.Generated
                    {
                       [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
                       internal partial class DeclareServices_DeclareServicesSourceGeneratorTest: IServiceInfoProvider
                       {
                           IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
                           {
                                yield return new ServiceDeclaration(typeof(IOpenGeneric<>), null);
                           }
                    
                           IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
                           {
                                yield return new ServiceImplementationDeclaration(typeof(OpenGeneric<>), typeof(Kephas.Sample.IOpenGeneric<>));
                                yield return new ServiceImplementationDeclaration(typeof(OverrideOpenGeneric<>), typeof(Kephas.Sample.IOpenGeneric<>));
                           }
                       }
                    }
                    
                    """);
            });
    }

    [Fact]
    public void Generate_DeclareServices_OverrideBaseImplementation()
    {
        var sourceCode =
            """
            using System;
            using Kephas.DependencyInjection;
            
            namespace Kephas.Sample;
            
            [Priority(Priority.BelowNormal)]
            [OverrideBaseImplementation]
            public class FactoryServiceEnumerable<TService, TMetadata> : FactoryEnumerable<TService, TMetadata>
                where TService : class
            {
                /// <summary>
                /// Initializes a new instance of the <see cref="FactoryServiceEnumerable{TService}"/> class.
                /// </summary>
                public FactoryServiceEnumerable(IEnumerable<IFactory<TService, TMetadata>>? serviceFactories = null)
                    : base(serviceFactories)
                {
                }
            }
            """;

        var result = GetRunResult(sourceCode, [typeof(IFactoryEnumerable<,>).Assembly]);

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
                    
                    [assembly: DeclareServices<DeclareServicesSourceGeneratorTest.ServiceProviders.Generated.DeclareServices_DeclareServicesSourceGeneratorTest>()]
                    
                    namespace DeclareServicesSourceGeneratorTest.ServiceProviders.Generated
                    {
                       [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
                       internal partial class DeclareServices_DeclareServicesSourceGeneratorTest: IServiceInfoProvider
                       {
                           IEnumerable<ServiceDeclaration>? IServiceInfoProvider.GetServiceDeclarations()
                           {
                                yield break;
                           }
                    
                           IEnumerable<ServiceImplementationDeclaration> IServiceInfoProvider.GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
                           {
                                yield return new ServiceImplementationDeclaration(typeof(FactoryServiceEnumerable<,>), typeof(Kephas.DependencyInjection.IFactoryEnumerable<, >));
                           }
                       }
                    }
                    
                    """);
            });
    }
}