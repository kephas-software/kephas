// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidAsStringSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB.Serializers;

using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Serializers;

/// <summary>
/// Serializes GUIDs as strings.
/// </summary>
public class GuidAsStringSerializer : GuidSerializer, IMongoSerializer<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GuidAsStringSerializer"/> class.
    /// </summary>
    public GuidAsStringSerializer()
        : base(BsonType.String)
    {
    }
}