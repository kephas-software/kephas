// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryBulkDeleteCommandTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory bulk delete command test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.InMemory.Commands
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.InMemory.Commands;

    using NUnit.Framework;

    [TestFixture]
    public class InMemoryBulkDeleteCommandTest
    {
        [Test]
        public async Task ExecuteAsync_success()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.CreateDataContext(localCache: localCache);
            var cmd = new InMemoryBulkDeleteCommand();

            var entityEntry = new EntityEntry(new TestEntity { Id = 1 });
            localCache.Add(entityEntry);
            entityEntry = new EntityEntry(new TestEntity { Id = 2 });
            localCache.Add(entityEntry);
            entityEntry = new EntityEntry(new TestEntity { Id = 3 });
            localCache.Add(entityEntry);

            var opContext = new BulkDeleteContext<TestEntity>(dataContext, e => e.Id > 1);
            var result = await cmd.ExecuteAsync(opContext);
            var affected = result.Count;

            Assert.AreEqual(2, affected);
            Assert.AreEqual(1, localCache.Count);
            Assert.IsTrue(localCache.Values.Select(v => v.Entity).OfType<TestEntity>().All(e => e.Id <= 1));
        }

        [Test]
        public async Task ExecuteAsync_not_found_returns_zero()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.InitializeDataContext(localCache: localCache);

            var cmd = new InMemoryBulkDeleteCommand();

            var entityEntry = new EntityEntry(new TestEntity { Id = 1 });
            localCache.Add(entityEntry);

            var opContext = new BulkDeleteContext<TestEntity>(dataContext, e => e.Id > 1);
            var result = await cmd.ExecuteAsync(opContext);
            var affected = result.Count;

            Assert.AreEqual(0, affected);
            Assert.AreEqual(1, localCache.Count);
        }

        [Test]
        public async Task ExecuteAsync_not_found_exception()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.InitializeDataContext(localCache: localCache);

            var cmd = new InMemoryBulkDeleteCommand();

            var entityEntry = new EntityEntry(new TestEntity { Id = 1 });
            localCache.Add(entityEntry);

            var opContext = new BulkDeleteContext<TestEntity>(dataContext, e => e.Id > 1, throwIfNotFound: true);
            Assert.ThrowsAsync<NotFoundDataException>(() => cmd.ExecuteAsync(opContext));
        }

        public class TestEntity : IIdentifiable
        {
            object IIdentifiable.Id => this.Id;
            public int Id { get; set; }
        }
    }
}