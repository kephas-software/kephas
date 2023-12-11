// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization;

/// <summary>
/// Interface used to extract the information contained in this object to a serialization friendly representation.
/// </summary>
public interface ISerializable
{
    /// <summary>
    /// Converts this object to a serialization friendly representation.
    /// </summary>
    /// <returns>A serialization friendly object representing this object.</returns>
    object? GetObjectData();
}
