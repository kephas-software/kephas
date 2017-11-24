﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindCommandTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the find command test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Commands
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class FindCommandTest
    {
        [Test]
        public async Task ExecuteAsync_success()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), localCache);
            var cmd = new FindCommand();

            var entityInfo = new EntityInfo(new TestEntity { Id = 1 });
            localCache.Add(entityInfo);
            entityInfo = new EntityInfo(new TestEntity { Id = 2 });
            localCache.Add(entityInfo);
            entityInfo = new EntityInfo(new TestEntity { Id = 3 });
            localCache.Add(entityInfo);

            var findContext = new FindContext<TestEntity>(dataContext, 2);
            var result = await cmd.ExecuteAsync(findContext);
            var foundEntity = result.Entity;

            Assert.AreSame(localCache.Values.First(e => e.EntityId.Equals(2)).Entity, foundEntity);
        }

        [Test]
        public async Task ExecuteAsync_not_found_tollerant()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), localCache);
            var cmd = new FindCommand();

            var entityInfo = new EntityInfo(new TestEntity { Id = 1 });
            localCache.Add(entityInfo);

            var findContext = new FindContext<TestEntity>(dataContext, 2, throwIfNotFound: false);
            var result = await cmd.ExecuteAsync(findContext);
            var foundEntity = result.Entity;

            Assert.IsNull(foundEntity);
        }

        [Test]
        public async Task ExecuteAsync_not_found_exception()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), localCache);
            var cmd = new FindCommand();

            var entityInfo = new EntityInfo(new TestEntity { Id = 1 });
            localCache.Add(entityInfo);

            var findContext = new FindContext<TestEntity>(dataContext, 2, throwIfNotFound: true);
            Assert.ThrowsAsync<NotFoundDataException>(() => cmd.ExecuteAsync(findContext));
        }

        [Test]
        public async Task ExecuteAsync_ambiguous_exception()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), localCache);
            var cmd = new FindCommand();

            var entityInfo = new EntityInfo(new TestEntity { Id = 1 });
            localCache.Add(entityInfo);
            entityInfo = new EntityInfo(new TestEntity { Id = 2 });
            localCache.Add(entityInfo);
            entityInfo = new EntityInfo(new TestEntity { Id = 2 });
            localCache.Add(entityInfo);

            var findContext = new FindContext<TestEntity>(dataContext, 2);
            Assert.ThrowsAsync<AmbiguousMatchDataException>(() => cmd.ExecuteAsync(findContext));
        }

        public class TestEntity : IIdentifiable
        {
            public object Id { get; set; }
        }
    }
}