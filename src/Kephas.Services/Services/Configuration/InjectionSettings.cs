// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InjectionSettings.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Configuration
{
    using Kephas.Dynamic;

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
    /// Stores injection settings.
    /// </summary>
    public class InjectionSettings : Expando
    {
        /// <summary>
        /// Gets or sets the assembly file name pattern.
        /// </summary>
        /// <value>
        /// The assembly file name pattern.
        /// </value>
        public string? AssemblyFileNamePattern { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the resolution strategy for ambiguous registered services.
        /// </summary>
        public AmbiguousServiceResolutionStrategy AmbiguousResolutionStrategy { get; set; } =
            AmbiguousServiceResolutionStrategy.UseLast;
    }
}