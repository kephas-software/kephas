// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json;
using Kephas.DependencyInjection;

namespace Kephas.Serialization.Json;

/// <summary>
/// Service for JSON serialization and deserialization of objects.
/// </summary>
[SingletonService]
public interface IJsonService
{
    /// <summary>
    /// Deserializes the object.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>The deserialized object.</returns>
    T Deserialize<T>(string serializedObj, Action<JsonSerializerOptions>? optionsConfig = null);

    /// <summary>
    /// Deserializes the object.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="objType">The object type.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <returns>The deserialized object.</returns>
    object? Deserialize(string serializedObj, Type objType, Action<JsonSerializerOptions>? optionsConfig = null);

    /// <summary>
    /// Deserializes the object asynchronously.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>A task yielding the deserialized object.</returns>
    async Task<T> DeserializeAsync<T>(string serializedObj, Action<JsonSerializerOptions>? optionsConfig = null, CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        cancellationToken.ThrowIfCancellationRequested();
        return Deserialize<T>(serializedObj, optionsConfig);
    }

    /// <summary>
    /// Deserializes the object asynchronously.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="objType">The object type.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task yielding the deserialized object.</returns>
    async Task<object?> DeserializeAsync(string serializedObj, Type objType, Action<JsonSerializerOptions>? optionsConfig = null, CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        cancellationToken.ThrowIfCancellationRequested();
        return Deserialize(serializedObj, objType, optionsConfig);
    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>The serialized object.</returns>
    string Serialize<T>(T obj, Action<JsonSerializerOptions>? optionsConfig = null);

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="objType">The object type.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <returns>The serialized object.</returns>
    string Serialize(object obj, Type? objType, Action<JsonSerializerOptions>? optionsConfig = null);

    /// <summary>
    /// Serializes the object asynchronously.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>A task yielding the deserialized object.</returns>
    async Task<string> SerializeAsync<T>(T obj, Action<JsonSerializerOptions>? optionsConfig = null, CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        cancellationToken.ThrowIfCancellationRequested();
        return Serialize(obj, optionsConfig);
    }

    /// <summary>
    /// Serializes the object asynchronously.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="objType">The object type.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task yielding the deserialized object.</returns>
    async Task<string> SerializeAsync(object obj, Type? objType, Action<JsonSerializerOptions>? optionsConfig = null, CancellationToken cancellationToken = default)
    {
        await Task.Yield();

        cancellationToken.ThrowIfCancellationRequested();
        return Serialize(obj, objType, optionsConfig);
    }
}
