// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryCheckSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kephas.Extensions.Diagnostics.HealthChecks.Configuration;

/// <summary>
/// The memory check options.
/// </summary>
public record MemoryCheckSettings
{
    private const long DefaultThreshold = 1024L;

    /// <summary>
    /// Gets or sets failure threshold (in megabytes). By default it is 1 GB.
    /// </summary>
    public long Threshold { get; set; } = DefaultThreshold;

    /// <summary>
    /// Gets or sets the health status when the threshold is exceeded.
    /// By default it is <see cref="HealthStatus.Healthy"/>, meaning this health check is only informative.
    /// </summary>
    public HealthStatus ThresholdExceededStatus { get; set; } = HealthStatus.Healthy;
}