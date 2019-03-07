// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobStoreClassMap.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job store class map class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.MongoDB.JobStore
{
    using System;
    using System.Collections.Generic;

    using global::MongoDB.Bson;
    using global::MongoDB.Bson.Serialization;
    using global::MongoDB.Bson.Serialization.Serializers;

    using global::Quartz;
    using global::Quartz.Spi.MongoDbJobStore.Serializers;
    using global::Quartz.Util;

    using Kephas.Scheduling.Quartz.JobStore.Models;

    internal static class JobStoreClassMap
    {
        public static void RegisterClassMaps()
        {
            BsonSerializer.RegisterGenericSerializerDefinition(typeof (ISet<>), typeof (SetSerializer<>));
            BsonSerializer.RegisterSerializer(new JobDataMapSerializer());

            BsonClassMap.RegisterClassMap<Key<JobKey>>(map =>
            {
                map.AutoMap();
                map.MapProperty(key => key.Group);
                map.MapProperty(key => key.Name);
                map.AddKnownType(typeof(JobKey));
            });
            BsonClassMap.RegisterClassMap<Key<TriggerKey>>(map =>
            {
                map.AutoMap();
                map.MapProperty(key => key.Group);
                map.MapProperty(key => key.Name);
                map.AddKnownType(typeof(TriggerKey));
            });
            BsonClassMap.RegisterClassMap<JobKey>(map =>
            {
                map.MapCreator(jobKey => new JobKey(jobKey.Name));
                map.MapCreator(jobKey => new JobKey(jobKey.Name, jobKey.Group));
            });

            BsonClassMap.RegisterClassMap<TriggerKey>(map =>
            {
                map.MapCreator(triggerKey => new TriggerKey(triggerKey.Name));
                map.MapCreator(triggerKey => new TriggerKey(triggerKey.Name, triggerKey.Group));
            });
            BsonClassMap.RegisterClassMap<TimeOfDay>(map =>
            {
                map.AutoMap();
                map.MapProperty(day => day.Hour);
                map.MapProperty(day => day.Minute);
                map.MapProperty(day => day.Second);
                map.MapCreator(day => new TimeOfDay(day.Hour, day.Minute, day.Second));
                map.MapCreator(day => new TimeOfDay(day.Hour, day.Minute));
            });

            BsonClassMap.RegisterClassMap<JobDetail>(map =>
            {
                map.AutoMap();
                map.MapProperty(detail => detail.JobType).SetSerializer(new TypeSerializer());
            });

            BsonClassMap.RegisterClassMap<MisfireInstruction.DailyTimeIntervalTrigger>(map =>
            {
                map.AutoMap();
                var serializer =
                    new EnumerableInterfaceImplementerSerializer<HashSet<DayOfWeek>, DayOfWeek>(
                        new EnumSerializer<DayOfWeek>(BsonType.String));
                map.MapProperty(trigger => trigger.DaysOfWeek).SetSerializer(serializer);
            });

            /* TODO 
            await this.LockCollection.Indexes.CreateOneAsync(this.IndexBuilder.Ascending(@lock => @lock.AcquiredAt),
                new CreateIndexOptions() {ExpireAfter = TimeSpan.FromSeconds(30)});
            */
        }
    }
}