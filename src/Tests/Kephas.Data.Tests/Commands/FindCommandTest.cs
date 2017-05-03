// --------------------------------------------------------------------------------------------------------------------
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

            var entityInfo = new EntityInfo(new TestEntity { Id = new Id(1) });
            localCache.Add(entityInfo);
            entityInfo = new EntityInfo(new TestEntity { Id = new Id(2) });
            localCache.Add(entityInfo);
            entityInfo = new EntityInfo(new TestEntity { Id = new Id(3) });
            localCache.Add(entityInfo);

            var findContext = new FindContext<TestEntity>(dataContext, new Id(2));
            var result = await cmd.ExecuteAsync(findContext);
            var foundEntity = result.Entity;

            Assert.AreSame(localCache.Values.First(e => new Id(2) == e.EntityId).Entity, foundEntity);
        }

        public class TestEntity : IIdentifiable
        {
            public Id Id { get; set; }
        }
    }
}