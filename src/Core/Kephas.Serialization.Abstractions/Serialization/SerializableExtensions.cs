// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializableExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Kephas.ExceptionHandling;

namespace Kephas.Serialization;

/// <summary>
/// Extension methods for data formatting.
/// </summary>
public static class SerializableExtensions
{
    /// <summary>
    /// Converts the object to a serialization friendly representation.
    /// </summary>
    /// <param name="obj">The object to format.</param>
    /// <returns>A serialization friendly object representing this object.</returns>
    public static object? GetObjectData(this object? obj) =>
        obj switch
        {
            null => null,
            ISerializable serializable => serializable.GetObjectData(),
            Exception ex => GetObjectData(ex),
            _ => obj.ToString(),
        };

    /// <summary>
    /// Converts the object to a serialization friendly representation.
    /// </summary>
    /// <param name="exception">The object to format.</param>
    /// <returns>A serialization friendly object representing this object.</returns>
    [return: NotNullIfNotNull(nameof(exception))]
    public static object? GetObjectData(this Exception? exception) =>
        exception is null ? null : new ExceptionData(exception);
}