using Kephas.Extensions.Logging;
using Kephas.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Destructurers;

namespace Kephas.Extensions.Serilog;

/// <summary>
/// Extension methods for logging with Serilog.
/// </summary>
public static class SerilogExtensions
{
    /// <summary>
    /// Adds the serilog configuration to the builder.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder.</param>
    /// <returns>The provided configuration builder.</returns>
    public static IConfigurationBuilder AddSerilogLogging(this IConfigurationBuilder configurationBuilder)
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
    /// Adds Serilog logging to the services when used from within a non-hosted application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Optional callback used to configure the Serilog options.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddSerilogLogging(
        this IServiceCollection services,
        Action<SerilogOptions>? configureOptions = null)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));

        services
            .AddSingleton<ILogLevelSwitch, SerilogLevelSwitch>()
#if NET6_0
            .AddLogging(builder =>
            {
                var serviceProvider = builder.Services.BuildServiceProvider();
                var loggerConfiguration = new LoggerConfiguration();
                loggerConfiguration.ConfigureSerilog(serviceProvider, configureOptions);

                builder.AddSerilog(loggerConfiguration.CreateLogger());
            });
#else
            .AddSerilog((serviceProvider, loggerConfiguration) =>
            {
                loggerConfiguration.ConfigureSerilog(serviceProvider, configureOptions);
            });
#endif
        
        return services;
    }

    internal static void ConfigureSerilog(this LoggerConfiguration loggerConfiguration,
        IServiceProvider serviceProvider,
        Action<SerilogOptions>? configureOptions)
    {
        var options = new SerilogOptions
        {
            AppRuntime = serviceProvider.GetService<IAppRuntime>(),
            LevelSwitch = (serviceProvider.GetService<ILogLevelSwitch>() as ISerilogLevelSwitch)?.LevelSwitch,
            Configuration = serviceProvider.GetService<IConfiguration>(),
        };
        configureOptions?.Invoke(options);
        loggerConfiguration.ConfigureSerilogCore(serviceProvider, options);
    }

    /// <summary>
    /// Configures Serilog from the application configuration with default settings and enrichers.
    /// </summary>
    /// <remarks>
    /// The following enrichers are provided: LogContext, ThreadId, ProcessId, MachineName, ExceptionDetails.
    /// Also, the application runtime is set and the self log.
    /// </remarks>
    /// <param name="loggerConfiguration">The logger configuration.</param>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="options">The options to configure Serilog.</param>
    private static LoggerConfiguration ConfigureSerilogCore(
        this LoggerConfiguration loggerConfiguration,
        IServiceProvider serviceProvider,
        SerilogOptions? options = null)
    {
        _ = loggerConfiguration ?? throw new ArgumentNullException(nameof(loggerConfiguration));
        options ??= new SerilogOptions();

        if (options.AppRuntime is not null)
        {
            LoggingHelper.SetAppRuntime(options.AppRuntime);
        }
        var configuration = options.Configuration ?? new ConfigurationBuilder().Build();
        
        options.PreConfigure?.Invoke(serviceProvider, loggerConfiguration);

        loggerConfiguration
            .Destructure.ToMaximumDepth(options.MaxDestructuringDepth)
            .Destructure.ToMaximumCollectionCount(options.MaxCollectionCount)
            .Destructure.With(GetDestructuringPolicies(serviceProvider))
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithMachineName()
            .UseConfiguredExceptionDetailsEnricher(options, serviceProvider)
            .UseConfiguredLoggingLevelSwitch(options);

        var selfLog = options.SelfLog
                      ?? (message => LoggingHelper.WriteFallbackLogError(message, options.AppRuntime));
        global::Serilog.Debugging.SelfLog.Enable(selfLog);

        options.PostConfigure?.Invoke(serviceProvider, loggerConfiguration);

        return loggerConfiguration;
    }

    private static IDestructuringPolicy[] GetDestructuringPolicies(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetServices<IDestructuringPolicy>().ToArray();
    }

    private static LoggerConfiguration UseConfiguredLoggingLevelSwitch(this LoggerConfiguration loggerConfiguration, SerilogOptions options)
    {
        if (options.LevelSwitch is not null)
        {
            loggerConfiguration
                .MinimumLevel.ControlledBy(options.LevelSwitch);
        }

        return loggerConfiguration;
    }

    private static LoggerConfiguration UseConfiguredExceptionDetailsEnricher(
        this LoggerConfiguration loggerConfiguration,
        SerilogOptions options,
        IServiceProvider serviceProvider)
    {
        if (!options.UseExceptionDetailsEnricher)
        {
            return loggerConfiguration;
        }

        var exceptionOptions = options.ExceptionDestructuringOptions
                               ?? new DestructuringOptionsBuilder()
                                   .WithDestructurers(GetExceptionDestructurers(serviceProvider))
                                   .WithIgnoreStackTraceAndTargetSiteExceptionFilter()
                                   .WithDestructuringDepth(options.MaxDestructuringDepth)
                                   .WithoutReflectionBasedDestructurer();
        loggerConfiguration.Enrich.WithExceptionDetails(exceptionOptions);
        loggerConfiguration.Enrich.With(new FallbackExceptionEnricher(exceptionOptions));

        return loggerConfiguration;
    }

    private static IEnumerable<IExceptionDestructurer> GetExceptionDestructurers(IServiceProvider serviceProvider) =>
        DestructuringOptionsBuilder.DefaultDestructurers
            .Union(serviceProvider.GetServices<IExceptionDestructurer>());
}