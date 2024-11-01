// --------------------------------------------------------------------------------------------------------------------
// <copyright file="C.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Metadata;
using Kephas.DependencyInjection.Reflection;
using Kephas.DependencyInjection.ServiceProviders;
using NSubstitute;

namespace Kephas.Extensions.Tests.TestServices;

public class TestScopedService(IServiceProvider serviceProvider)
{
    public IServiceProvider ServiceProvider { get; } = serviceProvider;
}

public class ExportedClass
{
}

public record TestMetadata : ServiceMetadata
{
    public TestMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
    }
}

public class MultiFilterServiceInfoProvider : IServiceInfoProvider
{
    public IEnumerable<ServiceDeclaration>? GetServiceDeclarations()
    {
        yield return new(typeof(IFilter), null);
    }

    public IEnumerable<ServiceImplementationDeclaration> GetImplementationDeclarations(IEnumerable<Type> eligibleImplementationTypes)
    {
        yield return new ServiceImplementationDeclaration(typeof(OneFilter), typeof(IFilter));
        yield return new ServiceImplementationDeclaration(typeof(TwoFilter), typeof(IFilter));
    }
}

public interface IFilter;

public class OneFilter : IFilter;

public class TwoFilter : IFilter;

public interface IAsyncInitializable;

public interface ITestTypeLoader;

public class TestTypeLoader : ITestTypeLoader;