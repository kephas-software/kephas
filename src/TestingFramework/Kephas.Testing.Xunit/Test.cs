// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Test.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Reflection;
using Kephas.Runtime;
using Kephas.Testing.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Kephas.Testing;

/// <summary>
/// Provides testing primitives.
/// </summary>
public static class Test
{
    /// <summary>
    /// Gets the test <see cref="ILogger{T}"/>.
    /// </summary>
    /// <returns>The test logger.</returns>
    public static ILogger<T> GetTestLogger<T>(LogLevel? minimumLevel = null) => new TestLogger<T>(minimumLevel: minimumLevel);

    /// <summary>
    /// Gets the test <see cref="ILogger{T}"/>.
    /// </summary>
    /// <returns>The test logger.</returns>
    public static ILogger<T> GetTestLogger<T>(StringBuilder? stringBuilder, LogLevel? minimumLevel = null) => new TestLogger<T>(stringBuilder, minimumLevel: minimumLevel);

    /// <summary>
    /// Creates am empty list of <see cref="IServiceInfo"/>.
    /// </summary>
    public static IServiceInfoCollection CreateTestServices()
    {
        return new ServiceInfoCollection();
    }

    /// <summary>
    /// Creates a test <see cref="ServiceInfoCollectionBuilder"/>.
    /// </summary>
    /// <param name="services">Optional. The service collection used to initialize the builder with.</param>
    /// <returns>The <see cref="ServiceInfoCollectionBuilder"/>.</returns>
    public static ServiceInfoCollectionBuilder CreateTestServiceInfoCollectionBuilder(IEnumerable<IServiceInfo>? services = null)
    {
        var appRuntime = new AppRuntime();
        return new ServiceInfoCollectionBuilder(services ?? CreateTestServices(), () => appRuntime.GetAppAssemblies());
    }

    /// <summary>
    /// Builds a service provider based on <see cref="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ServiceInfoCollectionBuilder"/>.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildServiceProvider(this ServiceInfoCollectionBuilder builder) =>
        new ServiceCollection().AddApplicationServices(builder.Build()).BuildServiceProvider();


    /// <summary>
    /// Builds a service provider based on <see cref="services"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceInfoCollection"/>.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildServiceProvider(this IServiceInfoCollection services) =>
        new ServiceCollection().AddApplicationServices(services).BuildServiceProvider();

    /// <summary>
    /// Gets the test lazy enumerator.
    /// </summary>
    /// <param name="services">The services</param>
    /// <typeparam name="T">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <returns>The test <see cref="ILazyEnumerable{TContract,TMetadata}"/>.</returns>
    public static ILazyEnumerable<T, TMetadata> GetTestLazyEnumerable<T, TMetadata>(params T[] services)
    {
        var lazyEnumerable = Substitute.For<ILazyEnumerable<T, TMetadata>>();

        lazyEnumerable.GetEnumerator()
            .Returns(services
                .Select(s => new Lazy<T, TMetadata>(() => s, CreateMetadata<TMetadata>(null)))
                .ToList()
                .GetEnumerator());

        return lazyEnumerable;
    }

    /// <summary>
    /// Gets a test <see cref="IAppRuntime"/> service.
    /// </summary>
    /// <returns>The test <see cref="IAppRuntime"/> service.</returns>
    public static IAppRuntime GetTestAppRuntime(string? environment = null, string? appName = null)
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        appRuntime.GetEnvironment().Returns(environment);
        appRuntime.GetAppName().Returns(appName);

        return appRuntime;
    }

    private static TMetadata CreateMetadata<TMetadata>(IDictionary<string, object?>? metadata)
    {
        var typedMetadata = Activator.CreateInstance(typeof(TMetadata), metadata);
        return (TMetadata)typedMetadata!;
    }
}