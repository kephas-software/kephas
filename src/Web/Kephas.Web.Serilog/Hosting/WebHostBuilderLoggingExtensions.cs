// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostBuilderLoggingExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.Hosting;
using Kephas.Web.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Kephas.Web.Hosting;

/// <summary>
/// Logging related extension methods for <see cref="IHostBuilder"/> for use in ASP.NET Core applications.
/// </summary>
public static class WebHostBuilderLoggingExtensions
{
    /// <summary>
    /// Configures the Serilog with default options and without additional configuration.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <returns>The provided host builder.</returns>
    public static TBuilder UseWebLogging<TBuilder>(
        this TBuilder builder)
        where TBuilder : IHostBuilder =>
        UseWebLogging<TBuilder>(builder, (_, _, _) => { });

    /// <summary>
    /// Configures the Serilog with default options.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <param name="configureLogger">The action to configure the logger additionally.</param>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <returns>The provided host builder.</returns>
    public static TBuilder UseWebLogging<TBuilder>(
        this TBuilder builder,
        Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> configureLogger)
        where TBuilder : IHostBuilder
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        configureLogger = configureLogger ?? throw new ArgumentNullException(nameof(configureLogger));

        builder.ConfigureServices((context, services) =>
        {
            services.AddTransient<HttpRequestEnricher>();
        });
        
        builder.UseLogging((context, services, configuration) =>
            {
                configuration
                    .Enrich.With(
                        services.GetRequiredService<HttpRequestEnricher>());
                
                configureLogger(context, services, configuration);
            });

        return builder;
    }
}
