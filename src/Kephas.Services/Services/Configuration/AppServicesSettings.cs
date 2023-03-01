// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServicesSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

using Kephas.Dynamic;

namespace Kephas.Services.Configuration;

/// <summary>
/// Strategy for resolving ambiguous service registrations.
/// </summary>
public enum AmbiguousServiceResolutionStrategy
{
    /// <summary>
    /// Uses the last registered service.
    /// </summary>
    UseLast = 0,

    /// <summary>
    /// Uses the first registered service.
    /// </summary>
    UseFirst = 1,

    /// <summary>
    /// Forces the use of priority order. If the priority indicate the same value, an exception occurs.
    /// </summary>
    ForcePriority = 0x10,
}

/// <summary>
/// Stores application services related settings.
/// </summary>
public class AppServicesSettings : Expando
{
    /// <summary>
    /// Gets or sets the assembly file name pattern.
    /// </summary>
    /// <value>
    /// The assembly file name pattern.
    /// </value>
    public string? AssemblyFileNamePattern { get; set; }

    /// <summary>
    /// Gets or sets a function to retrieve the application assemblies containing service registrations.
    /// </summary>
    public Func<IEnumerable<Assembly>>? GetAppAssemblies { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the resolution strategy for ambiguous registered services.
    /// </summary>
    public AmbiguousServiceResolutionStrategy AmbiguousResolutionStrategy { get; set; } =
        AmbiguousServiceResolutionStrategy.UseLast;
}