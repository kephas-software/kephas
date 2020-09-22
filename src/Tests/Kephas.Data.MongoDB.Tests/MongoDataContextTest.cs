// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDataContextTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using Kephas.Data.Linq;

namespace Kephas.Data.MongoDB.Tests
{
    using System.Threading.Tasks;

    using global::MongoDB.Bson.Serialization.Attributes;
    using NUnit.Framework;

    [TestFixture]
    public class MongoDataContextTest : MongoTestBase
    {
        [Test]
        public void Composition()
        {
            var container = this.CreateContainer();
            using var dataSpace = container.GetExport<IDataSpace>();
            var dataContext = dataSpace[typeof(NotificationMongoEntity)];

            Assert.IsInstanceOf<MongoDataContext>(dataContext);
        }

        [Test]
        public async Task CreateEntityAsync()
        {
            var container = this.CreateContainer();
            using var dataSpace = container.GetExport<IDataSpace>();
            var dataContext = dataSpace[typeof(NotificationMongoEntity)];

            var entity = await dataContext.CreateAsync<NotificationMongoEntity>();
            entity.Id = container.GetExport<IIdGenerator>().GenerateId();

            await dataContext.PersistChangesAsync();

            dataContext.Delete(entity);

            await dataContext.PersistChangesAsync();
        }

        [Test]
        public async Task UpdateEntityAsync()
        {
            var container = this.CreateContainer();
            using var dataSpace = container.GetExport<IDataSpace>();
            var dataContext = dataSpace[typeof(NotificationMongoEntity)];

            var entity = await dataContext.CreateAsync<NotificationMongoEntity>();
            entity.Id = container.GetExport<IIdGenerator>().GenerateId();
            entity.Description = $"Description for {entity.Id}";
            await dataContext.PersistChangesAsync();

            entity.Description = $"Description for {entity.Id} updated!";
            await dataContext.PersistChangesAsync();

            using var dataSpace2 = container.GetExport<IDataSpace>();
            var dataContext2 = dataSpace2[typeof(NotificationMongoEntity)];
            var entity2 = (await dataContext.Query<NotificationMongoEntity>().Where(e => e.Id == entity.Id)
                .ToListAsync()).Single();

            Assert.AreEqual($"Description for {entity.Id} updated!", entity2.Description);

            dataContext2.Delete(entity2);
            await dataContext2.PersistChangesAsync();
        }

        public abstract class MongoEntityBase : EntityBase
        {
            public MongoEntityBase()
            {
                this.Zone = "default";
            }

            [BsonId]
            public long Id
            {
                get => (long?)this[nameof(this.Id)] ?? 0;
                set => this[nameof(this.Id)] = value;
            }

            public string Zone
            {
                get => (string?)this[nameof(this.Zone)] ?? string.Empty;
                set => this[nameof(this.Zone)] = value;
            }
        }

        public class NotificationMongoEntity : MongoEntityBase
        {
            public DateTimeOffset TriggerTime
            {
                get => (DateTimeOffset?)this[nameof(this.TriggerTime)] ?? DateTimeOffset.MinValue;
                set => this[nameof(this.TriggerTime)] = value;
            }

            public string? Description
            {
                get => (string?)this[nameof(this.Description)];
                set => this[nameof(this.Description)] = value;
            }
        }
    }
}