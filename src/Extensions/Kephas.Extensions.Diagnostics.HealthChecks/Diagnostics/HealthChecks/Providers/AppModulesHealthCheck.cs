// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppModulesHealthCheck.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
using System.Text.RegularExpressions;
using Kephas.DependencyInjection.Metadata;
using Kephas.Extensions.Diagnostics.HealthChecks.Configuration;
using Kephas.Runtime;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Kephas.Extensions.Diagnostics.HealthChecks.Providers;

/// <summary>
/// Health checker for the infrastructure.
/// </summary>
[FailureStatus(HealthStatus.Degraded)]
[Tags("app-modules")]
[Priority(Priority.Low)]
public class AppModulesHealthCheck(IAppRuntime appRuntime, IOptions<HealthChecksSettings> options) : IHealthCheck
{
    /// <summary>
    /// Runs the health check, returning the status of the component being checked.
    /// </summary>
    /// <param name="context">A context object associated with the current execution.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> that can be used to cancel the health check.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that completes when the health check has finished, yielding the status of the component being checked.</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        await Task.Yield();

        try
        {
            var allResults = new List<(string name, HealthCheckResult result)>();
            foreach (var (name, pattern) in options.Value.ModulePatterns)
            {
                try
                {
                    allResults.Add((name, CheckAppModulesHealth(pattern)));
                }
                catch (Exception ex)
                {
                    allResults.Add((name, new HealthCheckResult(HealthStatus.Unhealthy, ex.Message, ex)));
                }
            }

            return allResults.Count == 0
                ? new HealthCheckResult(HealthStatus.Healthy, "No modules are available.")
                : new HealthCheckResult(
                    allResults.Min(r => r.result.Status),
                    allResults.Any(r => r.result.Exception is not null)
                        ? $"At least one application module has errors."
                        : "Provides information about the application modules.",
                    allResults.Any(r => r.result.Exception is not null)
                        ? new AggregateException(
                            "At least one application module has errors.",
                            allResults.Where(r => r.result.Exception is not null).Select(r => r.result.Exception!))
                        : null,
                    allResults.ToDictionary(r => r.name, r => (object)r.result.Data));
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(
                HealthStatus.Unhealthy,
                $"Internal error while performing the health checks on application modules.",
                ex);
        }
    }

    /// <summary>
    /// Checks the health of the application modules matching the specified pattern.
    /// </summary>
    /// <param name="pattern">The pattern to match module names.</param>
    /// <returns>The health check result for the matched modules.</returns>
    protected internal virtual HealthCheckResult CheckAppModulesHealth(string pattern) =>
        new(HealthStatus.Healthy, GetVersionDescription(), data: GetInfrastructureModules(pattern));

    private string GetVersionDescription() =>
        $"Version {Assembly.GetExecutingAssembly().GetName().Version}.";

    private IReadOnlyDictionary<string, object> GetInfrastructureModules(string pattern)
    {
        var regex = new Regex(pattern);
        return appRuntime.GetAppAssemblies()
            .Select(a => (assemblyName: a.GetName(), fileVersion: a.GetCustomAttribute<AssemblyFileVersionAttribute>()))
            .Where(an => regex.IsMatch(an.assemblyName.Name ?? string.Empty))
            .OrderBy(an => an.assemblyName.Name)
            .ToDictionary(an => an.assemblyName.Name!, an => GetModuleVersion(an.assemblyName, an.fileVersion));
    }

    private static object GetModuleVersion(AssemblyName assemblyName, AssemblyFileVersionAttribute? fileVersion)
    {
        return $"{assemblyName.Version?.ToString() ?? "<no-version>"}, file version: {fileVersion?.Version ?? "<no-version>"}";
    }
}
