// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorHandlingServiceRegistration.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.DependencyInjection;
using Kephas.Web.ExceptionHandling;
using Kephas.Web.ExceptionHandling.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kephas.Web;

/// <summary>
/// Service registration for error handling.
/// </summary>
public class ErrorHandlingServiceRegistration : IServiceRegistration
{
    /// <summary>
    /// Registers services in the provided collection, using the given configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services.Configure<ExceptionHandlerSettings>(settings => configuration.GetSection("ExceptionHandler").Bind(settings));
        services.Configure<ExceptionHandlerOptions>(settings => configuration.GetSection("ExceptionHandler").Bind(settings));
        services.ConfigureOptions(typeof(RequestExceptionHandlerConfigurator));
    }
}