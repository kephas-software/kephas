// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryHealthCheck.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Extensions.Diagnostics.HealthChecks.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Kephas.Extensions.Diagnostics.HealthChecks.Providers;

/// <summary>
/// Health checker for the memory.
/// </summary>
[FailureStatus(HealthStatus.Unhealthy)]
[Tags("memory")]
public class MemoryHealthCheck(IOptions<MemoryCheckSettings> options) : IHealthCheck
{
    /// <summary>
    /// Runs the health check, returning the status of the component being checked.
    /// </summary>
    /// <param name="context">A context object associated with the current execution.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> that can be used to cancel the health check.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that completes when the health check has finished, yielding the status of the component being checked.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;

        const int mbyte = 1024 * 1024; 

        // Include GC information in the reported diagnostics.
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        var allocatedMbytes = (double)allocated / mbyte;
        var data = new Dictionary<string, object>
        {
            { "AllocatedMB", allocatedMbytes.ToString("n2") },
            { "AllocatedBytes", ((double)allocated).ToString("n2") },
            { "Gen0Collections", GC.CollectionCount(0) },
            { "Gen1Collections", GC.CollectionCount(1) },
            { "Gen2Collections", GC.CollectionCount(2) },
        };

        var status = (allocated < settings.Threshold * mbyte) ? HealthStatus.Healthy : settings.ThresholdExceededStatus;

        return Task.FromResult(
            new HealthCheckResult(
                status,
                description: $"Reports degraded status if allocated bytes exceeds {settings.Threshold} MBytes.",
                exception: null,
                data: data));
    }
}