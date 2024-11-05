// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonSerializerOptionsProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using Kephas.DependencyInjection;

namespace Kephas.Serialization.Json;

/// <summary>
/// Provides the <see cref="JsonSerializerOptions"/>. 
/// </summary>
[SingletonService]
public interface IJsonSerializerOptionsProvider
{
    /// <summary>
    /// Gets the <see cref="JsonSerializerOptions"/>.
    /// </summary>
    /// <returns>The <see cref="JsonSerializerOptions"/>.</returns>
    JsonSerializerOptions GetOptions();

    /// <summary>
    /// Configures the provided options and returns them.
    /// </summary>
    /// <param name="options">The options to configure.</param>
    /// <returns>The <paramref name="options"/>.</returns>
    JsonSerializerOptions ConfigureOptions(JsonSerializerOptions options);
}
