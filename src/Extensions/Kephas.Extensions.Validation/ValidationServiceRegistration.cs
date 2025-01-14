// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationServiceRegistration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FluentValidation;
using Kephas.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Extensions;

/// <summary>
/// Service registration for validation.
/// </summary>
public class ValidationServiceRegistration : IServiceRegistration
{
    /// <summary>
    /// Registers services in the provided collection, using the given configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        var appRuntime = services.GetAppRuntime();
        services.AddValidatorsFromAssemblies(appRuntime.GetAppAssemblies(), ServiceLifetime.Scoped);
    }
}
