// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizedHostedServiceMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;
using Microsoft.Extensions.Hosting;

namespace Kephas.Extensions.Hosting;

/// <summary>
/// Metadata for <see cref="IHostedService"/>.
/// </summary>
public record SynchronizedHostedServiceMetadata : ServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SynchronizedHostedServiceMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public SynchronizedHostedServiceMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        RequireSyncTokens = (IEnumerable<string>?)TryGetValue(metadata, nameof(RequireSyncTokens), Enumerable.Empty<string>()) ?? [];
        GrantSyncTokens = (IEnumerable<string>?)TryGetValue(metadata, nameof(GrantSyncTokens), Enumerable.Empty<string>()) ?? [];
    }

    /// <summary>
    /// Enumerates the synchronization tokens required by this hosted service to start successfully.
    /// </summary>
    public IEnumerable<string> RequireSyncTokens { get; } = [];

    /// <summary>
    /// Enumerates the synchronization tokens granted by this hosted service after it started successfully.
    /// </summary>
    public IEnumerable<string> GrantSyncTokens { get; } = [];
}