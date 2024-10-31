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
    public IEnumerable<ServiceDeclarationInfo> GetServiceInfos()
    {
        yield return new ServiceDeclarationInfo(typeof(IFilter),
            new ServiceInfo(typeof(IFilter), typeof(OneFilter), AppServiceLifetime.Singleton));
        yield return new ServiceDeclarationInfo(typeof(IFilter),
            new ServiceInfo(typeof(IFilter), typeof(TwoFilter), AppServiceLifetime.Transient));
        yield return new ServiceDeclarationInfo(typeof(IFilter),
            new ServiceInfo(typeof(IFilter), Substitute.For<IFilter>()));
    }
}

public interface IFilter
{
}

public class OneFilter : IFilter
{
}

public class TwoFilter : IFilter
{
}

public interface IAsyncInitializable
{
}

public interface ITestTypeLoader
{
}

public class TestTypeLoader : ITestTypeLoader
{
}