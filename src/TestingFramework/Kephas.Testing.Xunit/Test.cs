// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Test.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Reflection;
using Kephas.Hosting;
using Kephas.Logging;
using Kephas.Runtime;
using Kephas.Testing.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Serilog;
using Serilog.Exceptions;

namespace Kephas.Testing;

/// <summary>
/// Provides testing primitives.
/// </summary>
public static class Test
{
    private static readonly int DefaultMaxDestructuringDepth = 5;
    private static readonly int DefaultMaxCollectionCount = 10;
    
    /// <summary>
    /// Gets the test <see cref="ILogger{T}"/>.
    /// </summary>
    /// <returns>The test logger.</returns>
    public static ILogger<T> GetTestLogger<T>(LogLevel? minimumLevel = null) => new TestLogger<T>(minimumLevel: minimumLevel);

    /// <summary>
    /// Gets the test <see cref="ILogger{T}"/>.
    /// </summary>
    /// <returns>The test logger.</returns>
    public static ILogger<T> GetTestLogger<T>(StringBuilder? stringBuilder, LogLevel? minimumLevel = null) => new TestLogger<T>(stringBuilder, minimumLevel: minimumLevel);

    /// <summary>
    /// Creates am empty list of <see cref="IServiceInfo"/>.
    /// </summary>
    public static IServiceInfoCollection CreateTestServices()
    {
        return new ServiceInfoCollection();
    }

    /// <summary>
    /// Creates a test <see cref="ServiceInfoCollectionBuilder"/>.
    /// </summary>
    /// <param name="services">Optional. The service collection used to initialize the builder with.</param>
    /// <returns>The <see cref="ServiceInfoCollectionBuilder"/>.</returns>
    public static ServiceInfoCollectionBuilder CreateTestServiceInfoCollectionBuilder(IServiceInfoCollection? services = null)
    {
        var appRuntime = new AppRuntime();
        return ServiceInfoCollectionBuilder.Create(services ?? CreateTestServices(), () => appRuntime.GetAppAssemblies());
    }

    /// <summary>
    /// Builds a service provider based on <see cref="builder"/>.
    /// </summary>
    /// <param name="builder">The <see cref="ServiceInfoCollectionBuilder"/>.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildServiceProvider(this ServiceInfoCollectionBuilder builder, IConfiguration? configuration = null) =>
        new ServiceCollection().AddApplicationServices(configuration ?? GetIntegrationConfig(), builder.Build()).BuildServiceProvider();


    /// <summary>
    /// Builds a service provider based on <see cref="services"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceInfoCollection"/>.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildServiceProvider(this IServiceInfoCollection services, IConfiguration? configuration = null) =>
        new ServiceCollection().AddApplicationServices(configuration ?? GetIntegrationConfig(), services).BuildServiceProvider();

    /// <summary>
    /// Gets the test lazy enumerator.
    /// </summary>
    /// <param name="services">The services</param>
    /// <typeparam name="T">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <returns>The test <see cref="ILazyEnumerable{TContract,TMetadata}"/>.</returns>
    public static ILazyEnumerable<T, TMetadata> GetTestLazyEnumerable<T, TMetadata>(params T[] services)
    {
        var lazyEnumerable = Substitute.For<ILazyEnumerable<T, TMetadata>>();

        lazyEnumerable.GetEnumerator()
            .Returns(services
                .Select(s => new Lazy<T, TMetadata>(() => s, CreateMetadata<TMetadata>(null)))
                .ToList()
                .GetEnumerator());

        return lazyEnumerable;
    }

    /// <summary>
    /// Gets a test <see cref="IAppRuntime"/> service.
    /// </summary>
    /// <returns>The test <see cref="IAppRuntime"/> service.</returns>
    public static IAppRuntime GetTestAppRuntime(string? environment = null, string? appName = null)
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        appRuntime.GetEnvironment().Returns(environment);
        appRuntime.GetAppName().Returns(appName);

        return appRuntime;
    }

    private static TMetadata CreateMetadata<TMetadata>(IDictionary<string, object?>? metadata)
    {
        var typedMetadata = Activator.CreateInstance(typeof(TMetadata), metadata);
        return (TMetadata)typedMetadata!;
    }
    
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