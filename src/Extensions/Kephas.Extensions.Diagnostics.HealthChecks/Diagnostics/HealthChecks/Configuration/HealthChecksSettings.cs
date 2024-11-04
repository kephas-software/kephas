// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthChecksSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kephas.Extensions.Diagnostics.HealthChecks.Configuration;

/// <summary>
/// Settings for the health checks infrastructure.
/// </summary>
public record HealthChecksSettings
{
    /// <summary>
    /// Gets or sets the URL path for invoking the health checks. Default is "/health-report".
    /// </summary>
    public string ReportUrlPath { get; set; } = "/health-report";

    /// <summary>
    /// Enumerates the patterns used to provide information about the application modules.
    /// </summary>
    public IDictionary<string, string> ModulePatterns { get; set; }
        = new Dictionary<string, string> { { "Kephas", "^Kephas\\." } };

    /// <summary>
    /// Gets or sets the health check publisher options.
    /// </summary>
    public HealthCheckPublisherOptions Publisher { get; set; } =
        new()
        {
            Delay = TimeSpan.FromSeconds(2),
        };
}
