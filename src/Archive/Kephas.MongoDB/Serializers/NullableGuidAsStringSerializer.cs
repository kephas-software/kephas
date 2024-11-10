// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullableGuidAsStringSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.MongoDB.Serializers;

using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Serializers;

/// <summary>
/// Serializes nullable GUIDs as strings.
/// </summary>
public class NullableGuidAsStringSerializer : NullableSerializer<Guid>, IMongoSerializer<Guid?>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullableGuidAsStringSerializer"/> class.
    /// </summary>
    public NullableGuidAsStringSerializer()
        : base(new GuidSerializer(BsonType.String))
    {
    }
}