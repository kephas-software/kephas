// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonHelper.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;

namespace Kephas.Serialization.Json;

/// <summary>
/// Helper class for static JSON serialization/deserialization.
/// </summary>
public static class JsonHelper
{
    /// <summary>
    /// Serializes the object, optional with customized serialization options.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <returns>The serialized string.</returns>
    public static string Serialize<T>(this T obj, Action<JsonSerializerOptions>? optionsConfig = null)
    {
        var serializationService = new JsonService(new JsonSerializerOptionsProvider());
        return serializationService.Serialize(obj, optionsConfig);
    }

    /// <summary>
    /// Deserializes the object, optional with customized serialization options.
    /// </summary>
    /// <param name="str">The string from which the object should be deserialized.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>The deserialized object.</returns>
    public static T Deserialize<T>(this string str, Action<JsonSerializerOptions>? optionsConfig = null)
    {
        var serializationService = new JsonService(new JsonSerializerOptionsProvider());
        return serializationService.Deserialize<T>(str, optionsConfig);
    }
}
