// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostBuilderLoggingExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.Hosting;
using Kephas.Extensions.Serilog;
using Kephas.Web.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        where TBuilder :
        IHostApplicationBuilder =>
            UseWebLogging<TBuilder>(builder, _ => { });

    /// <summary>
    /// Configures the Serilog with default options.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <param name="configureOptions">The action to configure the logger additionally.</param>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <returns>The provided host builder.</returns>
    public static TBuilder UseWebLogging<TBuilder>(
        this TBuilder builder,
        Action<SerilogOptions> configureOptions)
        where TBuilder :
        IHostApplicationBuilder
    {
        configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));
        builder = builder ?? throw new ArgumentNullException(nameof(builder));

        builder.Services
            .AddTransient<HttpRequestEnricher>()
            .AddTransient<CorrelationIdEnricher>();
        
        builder.UseLogging(options =>
        {
            configureOptions(options);
            var postConfigure = options.PostConfigure;
            options.PostConfigure = (services, loggerConfiguration) =>
            {
                postConfigure?.Invoke(services, loggerConfiguration);
                loggerConfiguration
                    .Enrich.With(
                        services.GetRequiredService<HttpRequestEnricher>(),
                        services.GetRequiredService<CorrelationIdEnricher>());
            };
        });

        return builder;
    }
}
