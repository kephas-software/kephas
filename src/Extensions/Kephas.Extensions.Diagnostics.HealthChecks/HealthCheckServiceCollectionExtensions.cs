using System.Reflection;
using Kephas.Collections;
using Kephas.DependencyInjection;
using Kephas.Extensions.Diagnostics.HealthChecks;
using Kephas.Extensions.Diagnostics.HealthChecks.Configuration;
using Kephas.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kephas.Extensions;

/// <summary>
/// Extension methods for the health check.
/// </summary>
public static class HealthCheckServiceCollectionExtensions
{
    /// <summary>
    /// The section name for the health check.
    /// </summary>
    public static readonly string HealthChecksSection = "HealthChecks";

    /// <summary>
    /// The section name for the memory health check.
    /// </summary>
    public static readonly string MemoryHealthCheckSection = $"{HealthChecksSection}:Memory";

    private static readonly MethodInfo AddCheckMethod = ReflectionHelper.GetGenericMethodOf(_ =>
        ((IHealthChecksBuilder) null!).AddCheck<IHealthCheck>(string.Empty, null, null, null));

    /// <summary>
    /// Add the health checks to the services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The provided service collection, for chaining the calls.</returns>
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services,
        IConfiguration configuration)
        => AddHealthCheckServices(
            services,
            options => configuration.GetSection(HealthChecksSection)?.Bind(options),
            options => configuration.GetSection(MemoryHealthCheckSection)?.Bind(options)
            );

    /// <summary>
    /// Add the health checks to the services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="config">Optional. The configuration callback.</param>
    /// <param name="memoryConfig">Optional. The memory check configuration callback.</param>
    /// <returns>The provided service collection, for chaining the calls.</returns>
    public static IServiceCollection AddHealthCheckServices(
        this IServiceCollection services,
        Action<HealthChecksSettings>? config = null,
        Action<MemoryCheckSettings>? memoryConfig = null)
    {
        var builder = services.AddHealthChecks();

        var hcOptions = new HealthChecksSettings();
        config?.Invoke(hcOptions);
        services.Configure<HealthChecksSettings>(options => config?.Invoke(options));
        services.Configure<MemoryCheckSettings>(options => memoryConfig?.Invoke(options));

        foreach (var registration in services.Where(r => r.ServiceType == typeof(Lazy<IHealthCheck, HealthCheckMetadata>)).ToList())
        {
            // get metadata
            if ((registration.ImplementationFactory!(null!) as IHasServiceMetadata)?.GetMetadata() is not HealthCheckMetadata metadata)
            {
                continue;
            }

            // add the health check
            var addCheck = AddCheckMethod.MakeGenericMethod(metadata.ImplementationType!);
            addCheck.Call(null, builder, metadata.ServiceKey, metadata.FailureStatus, GetHealthCheckTags(metadata), metadata.Timeout);
        }

        services.Configure<HealthCheckPublisherOptions>(
            options =>
            {
                options.Delay = hcOptions.Publisher.Delay;
                options.Timeout = hcOptions.Publisher.Timeout;
                options.Period = hcOptions.Publisher.Period;
            });
        
        return services;
    }

    private static IEnumerable<string> GetHealthCheckTags(HealthCheckMetadata metadata)
    {
        var tags = new HashSet<string> { metadata.ServiceKey!.ToLower() };
        if (metadata.Tags is not null)
        {
            tags.AddRange(metadata.Tags.Select(t => t.ToLower()));
        }

        return tags;
    }
}
