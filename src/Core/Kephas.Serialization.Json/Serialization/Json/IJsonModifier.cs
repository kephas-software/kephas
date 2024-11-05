// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonModifier.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization.Metadata;
using Kephas.DependencyInjection;

namespace Kephas.Serialization.Json;

/// <summary>
/// Application service used to modify the JsonTypeInfo settings created by default.
/// </summary>
[TransientService]
public interface IJsonModifier
{
    /// <summary>
    /// Modifies the <paramref name="jsonTypeInfo"/> created by default.
    /// </summary>
    /// <param name="jsonTypeInfo">The JSON type information.</param>
    void Modify(JsonTypeInfo jsonTypeInfo);
}
