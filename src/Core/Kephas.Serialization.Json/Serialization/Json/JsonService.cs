// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonService.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text;
using System.Text.Json;

namespace Kephas.Serialization.Json;

/// <summary>
/// The default implementation of the <see cref="IJsonService"/>.
/// </summary>
public class JsonService(IJsonSerializerOptionsProvider optionsProvider) : IJsonService
{
    /// <summary>
    /// Deserializes the object.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>The deserialized object.</returns>
    public T Deserialize<T>(string serializedObj, Action<JsonSerializerOptions>? optionsConfig = null)
    {
        var options = GetSerializerOptions(optionsConfig);
        return JsonSerializer.Deserialize<T>(serializedObj, options)
               ?? throw new InvalidOperationException($"Could not deserialize properly '{serializedObj}'.");
    }

    /// <summary>
    /// Deserializes the object.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="objType">The object type.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <returns>The deserialized object.</returns>
    public object? Deserialize(string serializedObj, Type objType, Action<JsonSerializerOptions>? optionsConfig = null)
    {
        var options = GetSerializerOptions(optionsConfig);
        return JsonSerializer.Deserialize(serializedObj, objType, options)
               ?? throw new InvalidOperationException($"Could not deserialize properly '{serializedObj}'.");
    }

    /// <summary>
    /// Deserializes the object asynchronously.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>A task yielding the deserialized object.</returns>
    public async Task<T> DeserializeAsync<T>(string serializedObj, Action<JsonSerializerOptions>? optionsConfig = null,
        CancellationToken cancellationToken = default)
    {
        var options = GetSerializerOptions(optionsConfig);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedObj));
        return await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken)
               ?? throw new InvalidOperationException($"Could not deserialize properly '{serializedObj}'.");
    }

    /// <summary>
    /// Deserializes the object asynchronously.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="objType">The object type.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task yielding the deserialized object.</returns>
    public async Task<object?> DeserializeAsync(string serializedObj, Type objType, Action<JsonSerializerOptions>? optionsConfig = null, CancellationToken cancellationToken = default)
    {
        var options = GetSerializerOptions(optionsConfig);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(serializedObj));
        return await JsonSerializer.DeserializeAsync(stream, objType, options, cancellationToken)
               ?? throw new InvalidOperationException($"Could not deserialize properly '{serializedObj}'.");
    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>The serialized object.</returns>
    public string Serialize<T>(T obj, Action<JsonSerializerOptions>? optionsConfig = null)
    {
        var options = GetSerializerOptions(optionsConfig);
        return JsonSerializer.Serialize(obj, obj?.GetType() ?? typeof(object), options)
               ?? throw new InvalidOperationException($"Could not serialize properly '{obj}'.");
    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="objType">The object type.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <returns>The serialized object.</returns>
    public string Serialize(object obj, Type? objType, Action<JsonSerializerOptions>? optionsConfig = null)
    {
        var options = GetSerializerOptions(optionsConfig);
        return JsonSerializer.Serialize(obj, objType ?? obj?.GetType() ?? typeof(object), options)
               ?? throw new InvalidOperationException($"Could not serialize properly '{obj}'.");
    }

    /// <summary>
    /// Serializes the object asynchronously.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <typeparam name="T">The object type.</typeparam>
    /// <returns>A task yielding the deserialized object.</returns>
    public async Task<string> SerializeAsync<T>(T obj, Action<JsonSerializerOptions>? optionsConfig = null,
        CancellationToken cancellationToken = default)
    {
        var options = GetSerializerOptions(optionsConfig);
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, obj, obj?.GetType() ?? typeof(object), options, cancellationToken);
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Serializes the object asynchronously.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="objType">The object type.</param>
    /// <param name="optionsConfig">Optional. Callback to configure serialization options.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A task yielding the deserialized object.</returns>
    public async Task<string> SerializeAsync(object obj, Type? objType, Action<JsonSerializerOptions>? optionsConfig = null,
        CancellationToken cancellationToken = default)
    {
        var options = GetSerializerOptions(optionsConfig);
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, obj, objType ?? obj?.GetType() ?? typeof(object), options, cancellationToken);
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    private JsonSerializerOptions GetSerializerOptions(Action<JsonSerializerOptions>? optionsConfig)
    {
        var options = optionsProvider.GetOptions();

        if (optionsConfig is not null)
        {
            options = new JsonSerializerOptions(options);
            optionsConfig(options);
        }

        return options;
    }
}
