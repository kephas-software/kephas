using Kephas.Extensions.Logging;
using Kephas.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;

namespace Kephas.Extensions.Serilog;

/// <summary>
/// Extension methods for logging with Serilog.
/// </summary>
public static class SerilogExtensions
{
    private static readonly int DefaultMaxDestructuringDepth = 5;
    private static readonly int DefaultMaxCollectionCount = 10;
    
    /// <summary>
    /// Adds the serilog configuration to the builder.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder.</param>
    /// <returns>The provided configuration builder.</returns>
    public static IConfigurationBuilder AddSerilog(this IConfigurationBuilder configurationBuilder)
    {
        _ = configurationBuilder ?? throw new ArgumentNullException(nameof(configurationBuilder));

        // add this assembly to Using configuration
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Serilog:Using:0", typeof(SerilogExtensions).Assembly.GetName().Name }
        });

        return configurationBuilder;
    }

    /// <summary>
    /// Configures Serilog from the application configuration with default settings and enrichers.
    /// </summary>
    /// <remarks>
    /// The following enrichers are provided: LogContext, ThreadId, ProcessId, MachineName, ExceptionDetails.
    /// Also, the application runtime is set and the self log.
    /// </remarks>
    /// <param name="loggerConfiguration">The logger configuration.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="appRuntime">The application runtime.</param>
    /// <param name="loggingLevelSwitch">The switch to control the logging level at runtime.</param>
    public static LoggerConfiguration ConfigureSerilog(this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration,
        IAppRuntime? appRuntime = null,
        LoggingLevelSwitch? loggingLevelSwitch = null)
    {
        _ = loggerConfiguration ?? throw new ArgumentNullException(nameof(loggerConfiguration));

        if (appRuntime is not null)
        {
            LoggingHelper.SetAppRuntime(appRuntime);
        }

        loggerConfiguration
            .Destructure.ToMaximumDepth(DefaultMaxDestructuringDepth)
            .Destructure.ToMaximumCollectionCount(DefaultMaxCollectionCount)
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithMachineName()
            .Enrich.WithExceptionDetails();

        if (loggingLevelSwitch is not null)
        {
            loggerConfiguration
                .MinimumLevel.ControlledBy(loggingLevelSwitch);
        }

        global::Serilog.Debugging.SelfLog.Enable(message => LoggingHelper.WriteFallbackLogError(message, appRuntime));

        return loggerConfiguration;
    }

    /// <summary>
    /// Adds Serilog logging to the services when used from within a non-hosted application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<ILogLevelSwitch>(new SerilogLevelSwitch(configuration))
            .AddLogging(builder =>
            {
                var appRuntime = services.GetAppRuntime(defaultIfNotFound: true);
                var levelSwitch = services.GetRegisteredSingletonService<ILogLevelSwitch>(
                    () => throw new InvalidOperationException($"Must register the {nameof(SerilogLevelSwitch)} service prior to adding logging services."));
                var loggerConfiguration = new LoggerConfiguration()
                    .ConfigureSerilog(configuration, appRuntime, (levelSwitch as ISerilogLevelSwitch)?.LevelSwitch);

                builder.AddSerilog(loggerConfiguration.CreateLogger());
            });
        
        return services;
    }
}