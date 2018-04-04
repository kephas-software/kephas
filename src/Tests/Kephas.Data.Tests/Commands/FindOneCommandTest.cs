// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindOneCommandTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the find one command test class.
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
    public class FindOneCommandTest
    {
        [Test]
        public async Task ExecuteAsync_success()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), localCache);
            var cmd = new FindOneCommand();

            var entityInfo = new EntityInfo(new TestEntity { Name = "gigi" });
            localCache.Add(entityInfo);
            entityInfo = new EntityInfo(new TestEntity { Name = "belogea" });
            localCache.Add(entityInfo);

            var findContext = new FindOneContext<TestEntity>(dataContext, e => e.Name == "belogea");
            var result = await cmd.ExecuteAsync(findContext);
            var foundEntity = result.Entity;

            Assert.AreSame(localCache.Values.First(e => e.Entity.ToDynamic().Name == "belogea").Entity, foundEntity);
        }

        [Test]
        public async Task ExecuteAsync_not_found_tollerant()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), localCache);
            var cmd = new FindOneCommand();

            var entityInfo = new EntityInfo(new TestEntity { Name = "gigi" });
            localCache.Add(entityInfo);

            var findContext = new FindOneContext<TestEntity>(dataContext, e => e.Name == "belogea", throwIfNotFound: false);
            var result = await cmd.ExecuteAsync(findContext);
            var foundEntity = result.Entity;

            Assert.IsNull(foundEntity);
        }

        [Test]
        public async Task ExecuteAsync_not_found_exception()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), localCache);
            var cmd = new FindOneCommand();

            var entityInfo = new EntityInfo(new TestEntity { Name = "gigi" });
            localCache.Add(entityInfo);

            var findContext = new FindOneContext<TestEntity>(dataContext, e => e.Name == "belogea", throwIfNotFound: true);
            Assert.ThrowsAsync<NotFoundDataException>(() => cmd.ExecuteAsync(findContext));
        }

        [Test]
        public async Task ExecuteAsync_ambiguous_exception()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), localCache);
            var cmd = new FindOneCommand();

            var entityInfo = new EntityInfo(new TestEntity { Name = "gigi" });
            localCache.Add(entityInfo);
            entityInfo = new EntityInfo(new TestEntity { Name = "belogea" });
            localCache.Add(entityInfo);
            entityInfo = new EntityInfo(new TestEntity { Name = "belogea" });
            localCache.Add(entityInfo);

            var findContext = new FindOneContext<TestEntity>(dataContext, e => e.Name == "belogea");
            Assert.ThrowsAsync<AmbiguousMatchDataException>(() => cmd.ExecuteAsync(findContext));
        }

        public class TestEntity
        {
            public string Name { get; set; }
        }
    }
}