// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the type serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Quartz.Spi.MongoDbJobStore.Serializers
{
    using System;

    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    internal class TypeSerializer : SerializerBase<Type>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Type value)
        {
            context.Writer.WriteString($"{value.FullName}, {value.Assembly.GetName().Name}");
        }

        public override Type Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var value = context.Reader.ReadString();
            return Type.GetType(value);
        }
    }
}