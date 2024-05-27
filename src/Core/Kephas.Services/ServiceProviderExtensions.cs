// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Kephas.Services;
using Kephas.Services.Metadata;
using Kephas.Services.Resources;

namespace Kephas;

/// <summary>
/// Extension methods for <see cref="IServiceProvider"/>.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Tries to resolve the service with the provided name.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns>
    /// The named service.
    /// </returns>
    public static T? GetServiceByName<T>(this IServiceProvider serviceProvider, string serviceName)
    {
        var exportFactories = ((IEnumerable<Lazy<T, AppServiceMetadata>>)(
                serviceProvider.GetService(typeof(ILazyEnumerable<T, AppServiceMetadata>))
                    ?? Array.Empty<IEnumerable<Lazy<T, AppServiceMetadata>>>()))
            .Where(f => f.Metadata.ServiceName == serviceName)
            .ToList();
        return exportFactories.Count switch
        {
            0 => default,
            > 1 => throw new AmbiguousMatchException(
                string.Format(
                    Strings.DefaultNamedServiceProvider_GetNamedService_AmbiguousMatch_Exception,
                    serviceName,
                    typeof(T))),
            _ => exportFactories[0].Value
        };
    }

    /// <summary>
    /// Resolves the service with the provided name.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="serviceName">Name of the service.</param>
    /// <returns>
    /// The named service.
    /// </returns>
    [return: NotNull]
    public static T GetRequiredServiceByName<T>(this IServiceProvider serviceProvider, string serviceName)
    {
        var service = GetServiceByName<T>(serviceProvider, serviceName);
        return service is null
            ? throw new ServiceException(
                string.Format(
                    Strings.DefaultNamedServiceProvider_GetNamedService_NoServiceFound_Exception,
                    serviceName,
                    typeof(T)))
            : service;
    }

    /// <summary>
    /// Resolves the service with the provided service type.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="serviceType">The service type.</param>
    /// <returns>
    /// The service.
    /// </returns>
    public static object GetRequiredService(this IServiceProvider serviceProvider, Type serviceType) =>
        serviceProvider.GetService(serviceType)
        ?? throw new InvalidOperationException($"Service {serviceType} not found.");
}
