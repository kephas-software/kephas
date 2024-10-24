// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceProviderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas;

/// <summary>
/// Extension methods for <see cref="IServiceProvider"/>.
/// </summary>
public static class ServiceProviderExtensions
{
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
