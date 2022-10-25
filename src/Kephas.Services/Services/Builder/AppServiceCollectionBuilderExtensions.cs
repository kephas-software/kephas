// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceCollectionBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Builder;

using System.Reflection;
using Kephas.Collections;

/// <summary>
/// Extension methods for <see cref="IAppServiceCollectionBuilder"/>.
/// </summary>
public static class AppServiceCollectionBuilderExtensions
{
    /// <summary>
    /// Add the assemblies to the builder.
    /// </summary>
    /// <param name="builder">The <see cref="IAppServiceCollectionBuilder"/> instance.</param>
    /// <param name="assemblies">The assemblies to add.</param>
    /// <returns>The provided <see cref="IAppServiceCollectionBuilder"/> instance.</returns>
    public static IAppServiceCollectionBuilder WithAssemblies(this IAppServiceCollectionBuilder builder, IEnumerable<Assembly> assemblies)
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));

        builder.Assemblies.AddRange(assemblies);

        return builder;
    }

    /// <summary>
    /// Add the assemblies to the builder.
    /// </summary>
    /// <param name="builder">The <see cref="IAppServiceCollectionBuilder"/> instance.</param>
    /// <param name="assemblies">The assemblies to add.</param>
    /// <returns>The provided <see cref="IAppServiceCollectionBuilder"/> instance.</returns>
    public static IAppServiceCollectionBuilder WithAssemblies(this IAppServiceCollectionBuilder builder, params Assembly[] assemblies) =>
        WithAssemblies(builder, (IEnumerable<Assembly>)assemblies);

    /// <summary>
    /// Add the service providers to the builder.
    /// </summary>
    /// <param name="builder">The <see cref="IAppServiceCollectionBuilder"/> instance.</param>
    /// <param name="serviceProviders">The service providers.</param>
    /// <returns>The provided <see cref="IAppServiceCollectionBuilder"/> instance.</returns>
    public static IAppServiceCollectionBuilder WithServiceInfoProviders(this IAppServiceCollectionBuilder builder, IEnumerable<IAppServiceInfoProvider> serviceProviders)
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));

        builder.ServiceInfoProviders.AddRange(serviceProviders);

        return builder;
    }

    /// <summary>
    /// Add the service providers to the builder.
    /// </summary>
    /// <param name="builder">The <see cref="IAppServiceCollectionBuilder"/> instance.</param>
    /// <param name="serviceProviders">The service providers.</param>
    /// <returns>The provided <see cref="IAppServiceCollectionBuilder"/> instance.</returns>
    public static IAppServiceCollectionBuilder WithServiceInfoProviders(this IAppServiceCollectionBuilder builder, params IAppServiceInfoProvider[] serviceProviders) =>
        WithServiceInfoProviders(builder, (IEnumerable<IAppServiceInfoProvider>)serviceProviders);
}