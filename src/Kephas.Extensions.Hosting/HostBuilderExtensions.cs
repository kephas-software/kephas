// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Extensions.Hosting;

using System;

using Kephas.Application;
using Kephas.Services.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// A host builder factory.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// Integrates the ambient services into the host builder and configures it.
    /// </summary>
    /// <remarks>
    /// For ASP.NET Core applications, do not create the container here, but instead in the Startup.ConfigureAppServices method.
    /// </remarks>
    /// <param name="hostBuilder">The host builder.</param>
    /// <param name="servicesBuilder">The services builder.</param>
    /// <param name="appArgs">The application arguments.</param>
    /// <param name="setupAction">Optional. Callback to setup the ambient services.</param>
    /// <returns>The provided host builder.</returns>
    public static IHostBuilder ConfigureAppServices(this IHostBuilder hostBuilder, IAppServiceCollectionBuilder servicesBuilder, IAppArgs appArgs, Action<HostBuilderContext, IServiceCollection, IAppServiceCollectionBuilder>? setupAction = null)
    {
        hostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
        servicesBuilder = servicesBuilder ?? throw new ArgumentNullException(nameof(servicesBuilder));

        var ambientServices = servicesBuilder.AmbientServices;

        hostBuilder
            .ConfigureServices((context, services) =>
            {
                setupAction?.Invoke(context, services, servicesBuilder);
                services.UseAmbientServices(ambientServices);
            })
            .ConfigureAppConfiguration(
                (ctx, cfg) =>
                {
                    ambientServices.AddAppArgs(appArgs);
                });

        hostBuilder.Properties[nameof(IAmbientServices)] = ambientServices;
        return hostBuilder;
    }
}