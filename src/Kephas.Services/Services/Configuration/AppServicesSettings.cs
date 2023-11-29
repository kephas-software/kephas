// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

namespace Kephas.Services.Configuration;

/// <summary>
/// Stores application services related settings.
/// </summary>
public record AppServicesSettings
{
    /// <summary>
    /// Gets or sets the service assembly file name pattern.
    /// </summary>
    /// <value>
    /// The service assembly file name pattern.
    /// </value>
    public string? ServiceAssemblyFileNamePattern { get; set; }

    /// <summary>
    /// Gets or sets a function to retrieve the assemblies containing service registrations.
    /// </summary>
    public Func<IEnumerable<Assembly>>? GetServiceAssemblies { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the resolution strategy for ambiguous registered services.
    /// </summary>
    public AmbiguousServiceResolutionStrategy AmbiguousResolutionStrategy { get; set; } =
        AmbiguousServiceResolutionStrategy.UseLast;
}
