// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediatorServiceRegistration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions.Mediator;

/// <summary>
/// Service registration for the MediatR infrastructure.
/// </summary>
public class MediatorServiceRegistration : IServiceRegistration
{
    /// <summary>
    /// Registers services in the provided collection, using the given configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMediatR(config =>
        {
            config.MediatorImplementationType = typeof(MediatorService);
            // Disable automatic scanning, the MediatorServiceProvider will resolve the services.
            config.RegisterServicesFromAssemblies(GetType().Assembly);
            config.TypeEvaluator = _ => false;
        });
    }
}