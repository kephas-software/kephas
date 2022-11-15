// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoSerializerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB.Serializers;

using Kephas.Collections;
using Kephas.Services;

/// <summary>
/// Metadata for <see cref="IMongoSerializer{TValue}"/>.
/// </summary>
public class MongoSerializerMetadata : AppServiceMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoSerializerMetadata"/> class.
    /// </summary>
    /// <param name="metadata">The metadata.</param>
    public MongoSerializerMetadata(IDictionary<string, object?>? metadata)
        : base(metadata)
    {
        if (metadata is null)
        {
            return;
        }

        this.ValueType = (Type?)metadata.TryGetValue(nameof(this.ValueType));
    }

    /// <summary>
    /// Gets the value type.
    /// </summary>
    public Type? ValueType { get; }
}