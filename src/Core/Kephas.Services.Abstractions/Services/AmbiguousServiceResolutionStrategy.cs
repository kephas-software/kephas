// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AmbiguousServiceResolutionStrategy.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services;

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