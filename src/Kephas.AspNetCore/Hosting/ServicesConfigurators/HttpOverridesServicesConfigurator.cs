// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpOverridesServicesConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.AspNetCore.Hosting.ServicesConfigurators;

using Kephas.Extensions.DependencyInjection;
using Kephas.Logging;
using Kephas.Services;
using Kephas.Services.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Services configurator for when the app is hosted in an Azure Linux App Service/Docker.
/// Set the <c>ASPNETCORE_FORWARDEDHEADERS_ENABLED</c> environment variable to <c>true</c> or set the
/// <c>ForwardedHeaders_Enabled</c> value in the application settings to true to clear the restriction
/// that only loopback proxies are allowed by default. Reason: forwarders are enabled by explicit configuration.
/// </summary>
/// <remarks>
/// Check:
/// https://stackoverflow.com/questions/51143761/asp-net-core-docker-https-on-azure-app-service-containers
/// and
/// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-6.0#forward-the-scheme-for-linux-and-non-iis-reverse-proxies.
/// </remarks>
[ProcessingPriority(Priority.Highest)]
public class HttpOverridesServicesConfigurator : Loggable, IServicesConfigurator
{
    /// <summary>
    /// Configure the services.
    /// </summary>
    /// <param name="context">The host builder context.</param>
    /// <param name="services">The service collection.</param>
    /// <param name="servicesBuilder">The services builder.</param>
    public void ConfigureServices(
        HostBuilderContext context,
        IServiceCollection services,
        IAppServiceCollectionBuilder servicesBuilder)
    {
        var configuration = context.Configuration;
        var forwardedHeadersEnabled = configuration.GetValue<bool>("ForwardedHeaders_Enabled", false);
        if (forwardedHeadersEnabled)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.RequireHeaderSymmetry = false;

                // Only loopback proxies are allowed by default.
                // Clear that restriction because forwarders are enabled by explicit
                // configuration.
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();

                this.Logger.Info($"{typeof(ForwardedHeadersOptions)} is configured for container hosting.");
            });

            this.Logger.Info($"{typeof(ForwardedHeadersOptions)} will be configured for container hosting.");
        }
    }
}