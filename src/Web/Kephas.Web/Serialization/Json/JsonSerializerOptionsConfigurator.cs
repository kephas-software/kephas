// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonSerializerOptionsConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using Kephas.Serialization.Json;
using Microsoft.Extensions.Options;

namespace Kephas.Web.Serialization.Json;

/// <summary>
/// Configurator for <see cref="JsonSerializerOptions"/>.
/// </summary>
public class JsonSerializerOptionsConfigurator(IJsonSerializerOptionsProvider optionsProvider) : IConfigureOptions<JsonSerializerOptions>
{
    /// <summary>
    /// Invoked to configure an <see cref="JsonSerializerOptions"/> instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        optionsProvider.ConfigureOptions(options);
    }
}