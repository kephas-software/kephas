// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthCheckMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kephas.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Metadata for health checks.
/// </summary>
public record HealthCheckMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public HealthCheckMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        FailureStatus = (HealthStatus)Convert.ChangeType(TryGetValue(metadata, nameof(FailureStatus), HealthStatus.Healthy)!, typeof(int));
        Timeout = (TimeSpan?)TryGetValue(metadata, nameof(Timeout));
        Tags = (IEnumerable<string>)TryGetValue(metadata, nameof(Tags), null)!;
    }

    /// <summary>
    /// Gets the failure status.
    /// </summary>
    public HealthStatus? FailureStatus { get; }

    /// <summary>
    /// Gets the timeout.
    /// </summary>
    public TimeSpan? Timeout { get; }
    
    /// <summary>
    /// Gets the tags.
    /// </summary>
    public IEnumerable<string>? Tags { get; }
}