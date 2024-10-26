using Kephas.Hosting;
using Kephas.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;

namespace Kephas.Testing;

/// <summary>
/// Helper class for integration tests.
/// </summary>
public static class IntegrationTest
{
    private static readonly int DefaultMaxDestructuringDepth = 5;
    private static readonly int DefaultMaxCollectionCount = 10;
    
    /// <summary>
    /// Adds integration logging using Serilog.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The provided service collection.</returns>
    public static IServiceCollection AddIntegrationLogging(this IServiceCollection services, IConfiguration configuration)
    {
        _ = services ?? throw new ArgumentNullException(nameof(services));
        _ = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        LoggingHelper.SetAppRuntime(services.GetAppRuntime());

        services
            .AddLogging(builder =>
            {
                var loggerConfiguration = new LoggerConfiguration()
                    .Destructure.ToMaximumDepth(DefaultMaxDestructuringDepth)
                    .Destructure.ToMaximumCollectionCount(DefaultMaxCollectionCount)
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithThreadId()
                    .Enrich.WithProcessId()
                    .Enrich.WithMachineName()
                    .Enrich.WithExceptionDetails();

                global::Serilog.Debugging.SelfLog.Enable(TextWriter.Synchronized(Console.Error));

                builder.AddSerilog(loggerConfiguration.CreateLogger());
            });

        return services;
    }

    /// <summary>
    /// Gets the configuration based on the provided <paramref name="settingsFile"/> and its 'Development' flavor.
    /// </summary>
    /// <param name="settingsFile">The settings file. If not provided, 'appsettings.json' is considered.</param>
    /// <returns>The configuration.</returns>
    public static IConfigurationRoot GetIntegrationConfig(string? settingsFile = null)
    {
        settingsFile ??= "appsettings.json";
        var devSettingsFile = $"{settingsFile[0..^4]}Development.json"; 
        
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile(settingsFile, optional: true);
        configurationBuilder.AddJsonFile(devSettingsFile, optional: true);
        configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Serilog:Using:0", typeof(HostBuilderLoggingExtensions).Assembly.GetName()!.Name }
        });

        return configurationBuilder.Build();
    }
}
