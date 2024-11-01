// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthChecksSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kephas.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// The aggregated health checks result.
/// </summary>
[ExcludeFromCodeCoverage]
public record HealthChecksOverallResult
{
    /// <summary>
    /// Gets or sets the overall status.
    /// </summary>
    public HealthStatus OverallStatus { get; set; }

    /// <summary>
    /// Gets or sets the checks.
    /// </summary>
    public IEnumerable<HealthCheckResult> Checks { get; set; } = [];
}