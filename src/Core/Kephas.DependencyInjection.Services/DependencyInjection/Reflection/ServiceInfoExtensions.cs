// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceInfoExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Kephas.DependencyInjection.Reflection;

/// <summary>
/// Extension methods for <see cref="IServiceInfo"/>.
/// </summary>
public static class ServiceInfoExtensions
{
    /// <summary>
    /// Gets a value indicating whether the application service is a singleton.
    /// </summary>
    /// <param name="serviceInfo">The application service contract information.</param>
    /// <returns>
    /// <c>true</c> if shared; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSingleton(this IServiceInfo serviceInfo)
    {
        serviceInfo = serviceInfo ?? throw new ArgumentNullException(nameof(serviceInfo));

        return serviceInfo.Lifetime == AppServiceLifetime.Singleton;
    }

    /// <summary>
    /// Gets a value indicating whether the application service is shared within a scope.
    /// </summary>
    /// <param name="serviceInfo">The application service contract information.</param>
    /// <returns>
    /// <c>true</c> if shared within a scope; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsScoped(this IServiceInfo serviceInfo)
    {
        serviceInfo = serviceInfo ?? throw new ArgumentNullException(nameof(serviceInfo));

        return serviceInfo.Lifetime == AppServiceLifetime.Scoped;
    }

    /// <summary>
    /// Gets a value indicating whether the application service is instanced per request.
    /// </summary>
    /// <param name="serviceInfo">The application service contract information.</param>
    /// <returns>
    /// <c>true</c> if instanced per request; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsTransient(this IServiceInfo serviceInfo)
    {
        serviceInfo = serviceInfo ?? throw new ArgumentNullException(nameof(serviceInfo));

        return serviceInfo.Lifetime == AppServiceLifetime.Transient;
    }

    /// <summary>
    /// Sets a value indicating whether the application service is a singleton.
    /// </summary>
    /// <param name="serviceInfo">The application service contract information.</param>
    /// <returns>
    /// The <paramref name="serviceInfo"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ServiceInfo Singleton(this ServiceInfo serviceInfo)
    {
        serviceInfo = serviceInfo ?? throw new ArgumentNullException(nameof(serviceInfo));

        serviceInfo.Lifetime = AppServiceLifetime.Singleton;

        return serviceInfo;
    }

    /// <summary>
    /// Sets a value indicating whether the application service is shared within a scope.
    /// </summary>
    /// <param name="serviceInfo">The application service contract information.</param>
    /// <returns>
    /// The <paramref name="serviceInfo"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ServiceInfo Scoped(this ServiceInfo serviceInfo)
    {
        serviceInfo = serviceInfo ?? throw new ArgumentNullException(nameof(serviceInfo));

        serviceInfo.Lifetime = AppServiceLifetime.Scoped;

        return serviceInfo;
    }

    /// <summary>
    /// Sets a value indicating whether the application service is instanced per request.
    /// </summary>
    /// <param name="serviceInfo">The application service contract information.</param>
    /// <returns>
    /// The <paramref name="serviceInfo"/>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ServiceInfo Transient(this ServiceInfo serviceInfo)
    {
        serviceInfo = serviceInfo ?? throw new ArgumentNullException(nameof(serviceInfo));

        serviceInfo.Lifetime = AppServiceLifetime.Transient;

        return serviceInfo;
    }
}