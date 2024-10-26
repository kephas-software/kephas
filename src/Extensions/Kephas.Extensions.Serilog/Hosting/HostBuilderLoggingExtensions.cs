// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostBuilderLoggingExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.Logging;
using Kephas.Extensions.Serilog;
using Kephas.Runtime;
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
        where TBuilder : IHostBuilder =>
        UseLogging<TBuilder>(builder, (_, _, _) => { });

    /// <summary>
    /// Configures the Serilog with default options.
    /// </summary>
    /// <param name="builder">The host builder.</param>
    /// <param name="configureLogger">The action to configure the logger additionally.</param>
    /// <typeparam name="TBuilder">The builder type.</typeparam>
    /// <returns>The provided host builder.</returns>
    public static TBuilder UseLogging<TBuilder>(
        this TBuilder builder,
        Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> configureLogger)
        where TBuilder : IHostBuilder
    {
        builder = builder ?? throw new ArgumentNullException(nameof(builder));
        configureLogger = configureLogger ?? throw new ArgumentNullException(nameof(configureLogger));

        builder.ConfigureHostConfiguration(configBuilder => configBuilder.AddSerilog());

        builder.ConfigureServices((_, services) =>
        {
            ServiceCollectionExtensions.AddSingleton<ILogLevelSwitch, SerilogLevelSwitch>(services);
        });

        builder.UseSerilog((context, services, loggerConfiguration) =>
            {
                var appRuntime = services.GetService<IAppRuntime>();
                var levelSwitch = (services.GetService<ILogLevelSwitch>() as ISerilogLevelSwitch)?.LevelSwitch;
                loggerConfiguration.ConfigureSerilog(context.Configuration, appRuntime, levelSwitch);
                
                configureLogger(context, services, loggerConfiguration);
            });

        return builder;
    }
}
