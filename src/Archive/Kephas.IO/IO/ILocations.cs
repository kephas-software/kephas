// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocations.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.IO;

/// <summary>
/// Interface for providing a named enumeration of locations.
/// </summary>
public interface ILocations : IEnumerable<string>
{
    /// <summary>
    /// Gets the logical name of the location.
    /// </summary>
    public string Name { get; }
}