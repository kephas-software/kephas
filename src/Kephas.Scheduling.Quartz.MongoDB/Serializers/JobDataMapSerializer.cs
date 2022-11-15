// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobDataMapSerializer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job data map serializer class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Quartz.Spi.MongoDbJobStore.Serializers
{
    using System;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    using Quartz.Simpl;

    /// <summary>
    /// A job data map serializer.
    /// </summary>
    internal class JobDataMapSerializer : SerializerBase<JobDataMap>
    {
        private readonly DefaultObjectSerializer _objectSerializer = new DefaultObjectSerializer();

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, JobDataMap value)
        {
            if (value == null)
            {
                context.Writer.WriteNull();
                return;
            }

            var base64 = Convert.ToBase64String(_objectSerializer.Serialize(value));
            context.Writer.WriteString(base64);
        }

        public override JobDataMap Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Null)
            {
                context.Reader.ReadNull();
                return null;
            }

            var bytes = Convert.FromBase64String(context.Reader.ReadString());
            return _objectSerializer.DeSerialize<JobDataMap>(bytes);
        }
    }
}