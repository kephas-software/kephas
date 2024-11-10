// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityFrameworkCoreServiceRegistration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using Kephas.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Entities;

/// <summary>
/// Service registration for the Entity Framework Core services.
/// </summary>
/// <remarks>
/// Make sure all the other services are registered, leave the entities at the end, therefore <see cref="Priority.Low"/>.
/// </remarks>
[Priority(Priority.Low)]
public class EntityFrameworkCoreServiceRegistration : IServiceRegistration
{
    /// <summary>
    /// Registers services in the provided collection, using the given configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddEntityServices(configuration);
    }
}