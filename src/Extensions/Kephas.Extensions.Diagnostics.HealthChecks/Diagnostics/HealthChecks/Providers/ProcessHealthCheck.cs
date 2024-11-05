// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessHealthCheck.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.InteropServices;
using Kephas.Runtime;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kephas.Extensions.Diagnostics.HealthChecks.Providers;

/// <summary>
/// Health checker for the process.
/// </summary>
[FailureStatus(HealthStatus.Unhealthy)]
[Tags("process")]
public class ProcessHealthCheck(IAppRuntime appRuntime) : IHealthCheck
{
    /// <summary>
    /// Runs the health check, returning the status of the component being checked.
    /// </summary>
    /// <param name="context">A context object associated with the current execution.</param>
    /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> that can be used to cancel the health check.</param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task`1" /> that completes when the health check has finished, yielding the status of the component being checked.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        const int mbyte = 1024 * 1024;

        try
        {
            var processInfo = GetCurrentProcessInfo();
            var data = new Dictionary<string, object>
            {
                { "AppName", appRuntime.GetAppName() },
                { "AppVersion", appRuntime.GetAppVersion() },
                { "AppInstanceId", appRuntime.GetAppInstanceId() },
                { "Environment", appRuntime.GetEnvironment() },
                { "MachineName", appRuntime.GetMachineName() },

                { "ProcessName", processInfo.ProcessName },
                { "ProcessId", processInfo.Id },
                { "HandleCount", processInfo.HandleCount },
                { "ThreadCount", processInfo.ThreadCount },
                { "PrivateBytesMB", processInfo.PrivateMemorySize64 / mbyte },
                { "PeakWorkingSetMB", processInfo.PeakWorkingSet64 / mbyte },
                { "TotalProcessorTime", processInfo.TotalProcessorTime.ToString("g") },

                { "OSDescription", RuntimeInformation.OSDescription },
                { "OSArchitecture", RuntimeInformation.OSArchitecture },
                { "ProcessArchitecture", RuntimeInformation.ProcessArchitecture },
                { "FrameworkDescription", RuntimeInformation.FrameworkDescription },
                { "RuntimeIdentifier", RuntimeInformation.RuntimeIdentifier },
            };

            return Task.FromResult(
                new HealthCheckResult(
                    HealthStatus.Healthy,
                    description: "Reports process information.",
                    exception: null,
                    data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                new HealthCheckResult(
                    HealthStatus.Unhealthy,
                    description: "Process information could not be retrieved.",
                    exception: ex));
        }
    }
    
    /// <summary>
    /// Gets the current process information.
    /// </summary>
    /// <returns>The current process information.</returns>
    protected internal virtual ProcessInfo GetCurrentProcessInfo()
    {
        using var process = Process.GetCurrentProcess();
        return new ProcessInfo(
            process.ProcessName,
            process.Id,
            process.HandleCount,
            process.Threads.Count,
            process.PrivateMemorySize64,
            process.PeakWorkingSet64,
            process.TotalProcessorTime);
    }

    /// <summary>
    /// Provides process information. 
    /// </summary>
    public record ProcessInfo(string ProcessName, int Id, int HandleCount, int ThreadCount, long PrivateMemorySize64, long PeakWorkingSet64, TimeSpan TotalProcessorTime);
}
