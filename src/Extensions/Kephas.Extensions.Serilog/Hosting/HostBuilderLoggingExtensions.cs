// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostBuilderLoggingExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.Logging;
using Kephas.Extensions.Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Kephas.Extensions.Hosting;

/// <summary>
/// Logging related extension methods for <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderLoggingExtensions
{
    /// <summary>
    /// Configures the Serilog with default options and without additional configuration.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <returns>The provided host builder.</returns>
    public static TBuilder UseLogging<TBuilder>(this TBuilder builder)
        where TBuilder : 
#if NET6_0
        IHostBuilder =>
#else
        IHostApplicationBuilder =>
#endif
        UseLogging<TBuilder>(builder, _ => { });

    /// <summary>
    /// Configures the Serilog with default options.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <param name="configureOptions">The action to configure additionally the logger.</param>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <returns>The provided host builder.</returns>
    public static TBuilder UseLogging<TBuilder>(
        this TBuilder builder,
        Action<SerilogOptions> configureOptions)
        where TBuilder :
#if NET6_0
        IHostBuilder
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));

        builder.ConfigureHostConfiguration(configBuilder => configBuilder.AddSerilogLogging());

        builder.ConfigureServices((_, services) =>
        {
            services.AddSingleton<ILogLevelSwitch, SerilogLevelSwitch>();
        });

        builder.UseSerilog((_, serviceProvider, loggerConfiguration) =>
        {
            loggerConfiguration.ConfigureSerilog(serviceProvider, configureOptions);
        });

        return builder;
    }
#else
            IHostApplicationBuilder
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        configureOptions = configureOptions ?? throw new ArgumentNullException(nameof(configureOptions));

        builder.Configuration.AddSerilogLogging();
        builder.Services.AddSerilogLogging(configureOptions);

        return builder;
    }
#endif
}
