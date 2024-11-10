// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Test.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using Kephas.DependencyInjection;
using Kephas.DependencyInjection.Builder;
using Kephas.DependencyInjection.Builder.Configuration;
using Kephas.Extensions;
using Kephas.Extensions.Hosting;
using Kephas.Extensions.Logging;
using Kephas.Runtime;
using Kephas.Runtime.Configuration;
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
    /// Builds a service provider based on the provided parameters.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="appRuntimeConfig">Optional application runtime configurator.</param>
    /// <param name="builderConfig">Optional service builder configurator.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildEmptyTestServiceProvider(
        this IServiceCollection services,
        IConfiguration? configuration = null,
        Action<AppRuntimeSettings>? appRuntimeConfig = null,
        Action<ServiceInfoCollectionBuilder>? builderConfig = null) =>
        BuildTestServiceProvider(
            services,
            configuration,
            settings =>
            {
                settings.AppAssemblyNamePattern = "^1$"; // invalid name, should not return any assemblies
                appRuntimeConfig?.Invoke(settings);
            },
            settings =>
            {
                settings.GetAppAssemblies = () => [];
                settings.ConfigureBuilder = builderConfig;
            });

    /// <summary>
    /// Builds a service provider based on the provided parameters.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="appRuntimeConfig">Optional application runtime configurator.</param>
    /// <param name="builderConfig">Optional service builder configurator.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildEmptyTestServiceProvider(
        IConfiguration? configuration = null,
        Action<AppRuntimeSettings>? appRuntimeConfig = null,
        Action<ServiceInfoCollectionBuilder>? builderConfig = null) =>
        BuildEmptyTestServiceProvider(
            new ServiceCollection(),
            configuration,
            appRuntimeConfig,
            builderConfig);

    /// <summary>
    /// Builds a service provider based on the provided parameters.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="appRuntimeConfig">Optional application runtime configurator.</param>
    /// <param name="builderConfig">Optional service builder configurator.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildTestServiceProvider(
        IConfiguration? configuration = null,
        Action<AppRuntimeSettings>? appRuntimeConfig = null,
        Action<ServiceInfosBuilderSettings>? builderConfig = null) =>
        BuildTestServiceProvider(new ServiceCollection(), configuration, appRuntimeConfig, builderConfig);

    /// <summary>
    /// Builds a service provider based on the provided parameters.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="appRuntimeConfig">Optional application runtime configurator.</param>
    /// <param name="builderConfig">Optional service builder configurator.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildTestServiceProvider(
        this IServiceCollection services,
        IConfiguration? configuration = null,
        Action<AppRuntimeSettings>? appRuntimeConfig = null,
        Action<ServiceInfosBuilderSettings>? builderConfig = null)
    {
        configuration ??= GetIntegrationConfig();
        return services
            .AddApplicationServices(configuration, appRuntimeConfig, builderConfig)
            .BuildServiceProvider();
    }


    /// <summary>
    /// Builds a service provider based on the provided parameters.
    /// </summary>
    /// <typeparam name="T">The entry class. This type will provide the entry assembly.</typeparam>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="appRuntimeConfig">Optional application runtime configurator.</param>
    /// <param name="builderConfig">Optional service builder configurator.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildTestServiceProvider<T>(
        IConfiguration? configuration = null,
        Action<AppRuntimeSettings>? appRuntimeConfig = null,
        Action<ServiceInfosBuilderSettings>? builderConfig = null) =>
        BuildTestServiceProvider<T>(new ServiceCollection(), configuration, appRuntimeConfig, builderConfig);

    /// <summary>
    /// Builds a service provider based on the provided parameters.
    /// </summary>
    /// <typeparam name="T">The entry class. This type will provide the entry assembly.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="appRuntimeConfig">Optional application runtime configurator.</param>
    /// <param name="builderConfig">Optional service builder configurator.</param>
    /// <returns>A <see cref="IServiceProvider"/>.</returns>
    public static IServiceProvider BuildTestServiceProvider<T>(
        this IServiceCollection services,
        IConfiguration? configuration = null,
        Action<AppRuntimeSettings>? appRuntimeConfig = null,
        Action<ServiceInfosBuilderSettings>? builderConfig = null)
    {
        configuration ??= GetIntegrationConfig();
        return services
            .AddApplicationServices(
                configuration,
                settings => settings.ConfigureAppRuntimeSettings<T>(appRuntimeConfig),
                builderConfig)
            .BuildServiceProvider();
    }

    /// <summary>
    /// Gets the test lazy enumerator.
    /// </summary>
    /// <param name="services">The services</param>
    /// <typeparam name="T">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <returns>The test <see cref="ILazyEnumerable{T,TMetadata}"/>.</returns>
    public static ILazyEnumerable<T, TMetadata> GetTestLazyEnumerable<T, TMetadata>(params T[] services) =>
        new LazyEnumerable<T, TMetadata>(
            services
                .Select(s => new Lazy<T, TMetadata>(() => s, ServiceHelper.GetServiceMetadata<TMetadata>(null, typeof(T)))));

    /// <summary>
    /// Gets the test factory enumerator.
    /// </summary>
    /// <param name="services">The services</param>
    /// <typeparam name="T">The service type.</typeparam>
    /// <typeparam name="TMetadata">The metadata type.</typeparam>
    /// <returns>The test <see cref="IFactoryEnumerable{T,TMetadata}"/>.</returns>
    public static IFactoryEnumerable<T, TMetadata> GetTestFactoryEnumerable<T, TMetadata>(params T[] services)
        where T : notnull =>
        new FactoryEnumerable<T, TMetadata>(
            services
                .Select(s => new Factory<T, TMetadata>(() => s, ServiceHelper.GetServiceMetadata<TMetadata>(null, typeof(T)))));

    /// <summary>
    /// Gets a test <see cref="IAppRuntime"/> service.
    /// </summary>
    /// <remarks>The <see cref="IAppRuntime"/> it returns is a mock providing the specified assemblies.</remarks>
    /// <typeparam name="T">The entry class. This type will provide the entry assembly.</typeparam>
    /// <returns>The test <see cref="IAppRuntime"/> service.</returns>
    public static IAppRuntime GetTestAppRuntime<T>(IEnumerable<Assembly> assemblies)
    {
        var appRuntime = GetTestAppRuntime(assemblies);
        appRuntime.GetEntryAssembly().Returns(typeof(T).Assembly);

        return appRuntime;
    }

    /// <summary>
    /// Gets a test <see cref="IAppRuntime"/> service.
    /// </summary>
    /// <remarks>The <see cref="IAppRuntime"/> it returns is a mock providing the specified assemblies.</remarks>
    /// <returns>The test <see cref="IAppRuntime"/> service.</returns>
    public static IAppRuntime GetTestAppRuntime(IEnumerable<Assembly> assemblies)
    {
        var appRuntime = Substitute.For<IAppRuntime>();
        appRuntime.GetAppName().Returns("TestApp");
        appRuntime.GetAppAssemblies().Returns(assemblies ?? []);
        appRuntime.GetEnvironment().Returns("TEST");

        return appRuntime;
    }

    /// <summary>
    /// Gets a test <see cref="IAppRuntime"/> service.
    /// </summary>
    /// <typeparam name="T">The entry class. This type will provide the entry assembly.</typeparam>
    /// <returns>The test <see cref="IAppRuntime"/> service.</returns>
    public static IAppRuntime GetTestAppRuntime<T>(Action<AppRuntimeSettings>? appRuntimeConfig = null)
    {
        var appRuntimeSettings = new AppRuntimeSettings();
        appRuntimeSettings.ConfigureAppRuntimeSettings<T>(appRuntimeConfig);
        var appRuntime = new AppRuntime(appRuntimeSettings);
        return appRuntime;
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
        
        LoggingHelper.SetAppRuntime(services.GetAppRuntime(defaultIfNotFound: true));

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

    /// <summary>
    /// Executes the provided operation within the provided culture.
    /// </summary>
    /// <param name="cultureName">The culture name, for example "en-US" or "de-DE".</param>
    /// <param name="operation">The operation to execute.</param>
    public static void UsingCulture(string cultureName, Action operation)
    {
        _ = operation ?? throw new ArgumentNullException(nameof(operation));

        UsingCulture(cultureName, () =>
        {
            operation?.Invoke();
            return true;
        });
    }

    /// <summary>
    /// Executes the provided operation within the provided culture.
    /// </summary>
    /// <param name="cultureName">The culture name, for example "en-US" or "de-DE".</param>
    /// <param name="operation">The operation to execute.</param>
    public static T UsingCulture<T>(string cultureName, Func<T> operation)
    {
        _ = operation ?? throw new ArgumentNullException(nameof(operation));

        var currentCulture = CultureInfo.CurrentCulture;
        var currentUICulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            return operation();
        }
        finally
        {
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentUICulture;
        }
    }

    /// <summary>
    /// Executes the provided operation within the provided culture asynchronously.
    /// </summary>
    /// <param name="culture">The culture name, for example "en-US" or "de-DE".</param>
    /// <param name="operation">The operation to execute.</param>
    public static async Task UsingCultureAsync(string culture, Func<Task> operation)
    {
        _ = operation ?? throw new ArgumentNullException(nameof(operation));

        await UsingCultureAsync(culture, async () =>
        {
            await operation();
            return true;
        });
    }

    /// <summary>
    /// Executes the provided operation within the provided culture.
    /// </summary>
    /// <param name="culture">The culture name, for example "en-US" or "de-DE".</param>
    /// <param name="operation">The operation to execute.</param>
    public static async Task<T> UsingCultureAsync<T>(string culture, Func<Task<T>> operation)
    {
        _ = operation ?? throw new ArgumentNullException(nameof(operation));

        var currentCulture = CultureInfo.CurrentCulture;
        var currentUICulture = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            return await operation();
        }
        finally
        {
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentUICulture;
        }
    }

    private static void ConfigureAppRuntimeSettings<T>(this AppRuntimeSettings settings, Action<AppRuntimeSettings>? appRuntimeConfig = null)
    {
        settings.EntryAssembly = typeof(T).Assembly;
        var assemblyName = typeof(T).Assembly.GetName().Name ?? string.Empty;
        settings.AppAssemblyNamePattern = assemblyName.EndsWith("Test") || assemblyName.EndsWith("Tests")
            ? AppRuntimeSettings.TestsIncludedAppAssemblyNamePattern
            : AppRuntimeSettings.DefaultAppAssemblyNamePattern;
        appRuntimeConfig?.Invoke(settings);
    }
}