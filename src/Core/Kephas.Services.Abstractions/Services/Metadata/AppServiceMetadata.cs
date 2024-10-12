// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Metadata for application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Collections;

namespace Kephas.Services.Metadata;

/// <summary>
/// Metadata for application services.
/// </summary>
public class AppServiceMetadata : ServiceMetadataBase, IAppServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppServiceMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public AppServiceMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata == null)
        {
            return;
        }

        Priority = (Priority)Convert.ChangeType(metadata.TryGetValue(nameof(Priority), Priority.Normal)!, typeof(int));
        ServiceName = (string?)metadata.TryGetValue(nameof(ServiceName));
        IsOverride = (bool)metadata.TryGetValue(nameof(IsOverride), false)!;
        ServiceType = (Type?)metadata.TryGetValue(nameof(ServiceType));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppServiceMetadata" /> class.
    /// </summary>
    /// <param name="priority">Optional. The processing priority.</param>
    /// <param name="serviceName">Optional. The name of the service.</param>
    /// <param name="isOverride">Optional. Indicates whether the service overrides its base.</param>
    public AppServiceMetadata(Priority priority = 0, string? serviceName = null, bool isOverride = false)
        : base(new Dictionary<string, object?>())
    {
        Priority = priority;
        ServiceName = serviceName;
        IsOverride = isOverride;
    }

    /// <summary>
    /// Gets the order in which the services should be processed.
    /// </summary>
    /// <value>
    /// The processing priority.
    /// </value>
    public Priority Priority { get; }

    /// <summary>
    /// Gets a value indicating whether the service overrides the
    /// service it specializes.
    /// </summary>
    /// <value>
    /// True if the service overrides the service it specializes, false otherwise.
    /// </value>
    public bool IsOverride { get; }

    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    /// <value>
    /// The name of the service.
    /// </value>
    public string? ServiceName { get; set; }

    /// <summary>
    /// Gets or sets the concrete service type implementing the service contract.
    /// </summary>
    /// <value>
    /// The type of the service.
    /// </value>
    public Type? ServiceType { get; set; }

    /// <summary>
    /// Gets or sets the service dependencies.
    /// </summary>
    /// <value>
    /// The service dependencies.
    /// </value>
    public IEnumerable<Type> Dependencies { get; set; } = Enumerable.Empty<Type>();

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>
    /// A string that represents the current object.
    /// </returns>
    public override string ToString()
    {
        var serviceName = string.IsNullOrEmpty(ServiceName) ? string.Empty : $"Name: {ServiceName}, ";
        return $"Priority: {Priority}, {serviceName}Impl: {ServiceType}";
    }
}
