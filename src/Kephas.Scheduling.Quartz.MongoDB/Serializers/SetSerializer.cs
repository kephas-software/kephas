// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the set serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Quartz.Spi.MongoDbJobStore.Serializers
{
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    internal class SetSerializer<T> : SerializerBase<ISet<T>>
    {
        private readonly IBsonSerializer serializer;

        public SetSerializer()
        {
            this.serializer = BsonSerializer.LookupSerializer(typeof(IEnumerable<T>));
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ISet<T> value)
            => this.serializer.Serialize(context, args, value);

        public override ISet<T> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var enumerable = (IEnumerable<T>)this.serializer.Deserialize(context, args);
            return new HashSet<T>(enumerable);
        }
    }
}
