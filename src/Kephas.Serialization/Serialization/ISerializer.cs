// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base contract for serializers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Net.Mime;
using Kephas.Services;

namespace Kephas.Serialization;

/// <summary>
/// Base contract for serializers.
/// </summary>
/// <remarks>
/// Serializers handle a single media type and are aggregated by the <see cref="ISerializationService"/>.
/// </remarks>
[AppServiceContract(AllowMultiple = true)]
public interface ISerializer
{
    /// <summary>
    /// Serializes the provided object asynchronously.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
    /// <param name="settingsProvider">Optional. The provider for serialization settings.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result.
    /// </returns>
    Task SerializeAsync(
        object? obj,
        TextWriter textWriter,
        ISerializationSettingsProvider? settingsProvider = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Serializes the provided object asynchronously.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="settingsProvider">Optional. The provider for serialization settings.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result that yields the serialized object.
    /// </returns>
    Task<string> SerializeAsync(
        object? obj,
        ISerializationSettingsProvider? settingsProvider = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deserializes an object asynchronously.
    /// </summary>
    /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
    /// <param name="settingsProvider">Optional. The provider for serialization settings.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result that yields the deserialized object.
    /// </returns>
    Task<object?> DeserializeAsync(
        TextReader textReader,
        ISerializationSettingsProvider? settingsProvider = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deserializes an object asynchronously.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="settingsProvider">Optional. The provider for serialization settings.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>
    /// An asynchronous result that yields the deserialized object.
    /// </returns>
    Task<object?> DeserializeAsync(
        string serializedObj,
        ISerializationSettingsProvider? settingsProvider = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Serializes the provided object.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="textWriter">The <see cref="TextWriter"/> used to write the object content.</param>
    /// <param name="settingsProvider">Optional. The provider for serialization settings.</param>
    void Serialize(
        object? obj,
        TextWriter textWriter,
        ISerializationSettingsProvider? settingsProvider = null);

    /// <summary>
    /// Serializes the provided object.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <param name="settingsProvider">Optional. The provider for serialization settings.</param>
    /// <returns>
    /// The serialized object.
    /// </returns>
    string Serialize(
        object? obj,
        ISerializationSettingsProvider? settingsProvider = null);

    /// <summary>
    /// Deserializes an object.
    /// </summary>
    /// <param name="textReader">The <see cref="TextReader"/> containing the serialized object.</param>
    /// <param name="settingsProvider">Optional. The provider for serialization settings.</param>
    /// <returns>
    /// The deserialized object.
    /// </returns>
    object? Deserialize(
        TextReader textReader,
        ISerializationSettingsProvider? settingsProvider = null);

    /// <summary>
    /// Deserializes an object.
    /// </summary>
    /// <param name="serializedObj">The serialized object.</param>
    /// <param name="settingsProvider">Optional. The provider for serialization settings.</param>
    /// <returns>
    /// The deserialized object.
    /// </returns>
    object? Deserialize(
        string serializedObj,
        ISerializationSettingsProvider? settingsProvider = null);
}
