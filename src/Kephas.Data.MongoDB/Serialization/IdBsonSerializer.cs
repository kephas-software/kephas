// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdBsonSerializer.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the identifier bson serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Serialization
{
    using System;
    using global::MongoDB.Bson.Serialization;

    /// <summary>
    /// A BSON serializer for <see cref="Id"/>.
    /// </summary>
    public class IdBsonSerializer : IBsonSerializer<Id>
    {
        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        public Type ValueType => typeof(Id);

        /// <summary>Deserializes a value.</summary>
        /// <param name="context">The deserialization context.</param>
        /// <param name="args">The deserialization args.</param>
        /// <returns>A deserialized value.</returns>
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return this.Deserialize(context, args);
        }

        /// <summary>Serializes a value.</summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="args">The serialization args.</param>
        /// <param name="value">The value.</param>
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Id value)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>Deserializes a value.</summary>
        /// <param name="context">The deserialization context.</param>
        /// <param name="args">The deserialization args.</param>
        /// <returns>A deserialized value.</returns>
        public Id Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>Serializes a value.</summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="args">The serialization args.</param>
        /// <param name="value">The value.</param>
        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            this.Serialize(context, args, value as Id ?? new Id(value));
        }
    }
}