// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System;
    using System.Linq;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataContextBaseTest
    {
        [Test]
        public void CreateCommand()
        {
            var dataCommandProvider = Substitute.For<IDataCommandProvider>();
            var findCmd = Substitute.For<IFindCommand>();
            dataCommandProvider.CreateCommand(Arg.Any<Type>(), typeof(IFindCommand)).Returns(findCmd);

            var dataContext = new TestDataContext(dataCommandProvider: dataCommandProvider);
            var cmd = dataContext.CreateCommand<IFindCommand>();
            Assert.AreSame(findCmd, cmd);
        }

        [Test]
        public void AttachEntity_object()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = "hello";
            var entityInfo = dataContext.AttachEntity(entity);
            Assert.AreEqual(1, localCache.Count);
            Assert.AreSame(entityInfo, localCache.First().Value);
            Assert.AreSame(entity, entityInfo.Entity);
        }

        [Test]
        public void AttachEntity_graph()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = Substitute.For<IAggregatable>();
            var entityPart = new object();
            entity.GetStructuralEntityGraph().Returns(new[] { entity, entityPart });
            var entityInfo = dataContext.AttachEntity(entity);
            Assert.AreEqual(2, localCache.Count);
            Assert.AreSame(entityInfo, localCache.First().Value);
            Assert.AreSame(entity, entityInfo.Entity);
            Assert.AreSame(entityPart, localCache.Skip(1).First().Value.Entity);
        }

        [Test]
        public void DetachEntity_object()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = "hello";
            var entityInfo = dataContext.AttachEntity(entity);

            var detachedEntityInfo = dataContext.DetachEntity(entityInfo);
            Assert.AreSame(entityInfo, detachedEntityInfo);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void DetachEntity_graph()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = Substitute.For<IAggregatable>();
            var entityPart = new object();
            entity.GetStructuralEntityGraph().Returns(new[] { entity, entityPart });
            var entityInfo = dataContext.AttachEntity(entity);

            var detachedEntityInfo = dataContext.DetachEntity(entityInfo);
            Assert.AreSame(entityInfo, detachedEntityInfo);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void DetachEntity_not_own_entity_info()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entityInfo = new EntityInfo("hello");

            var detachedEntityInfo = dataContext.DetachEntity(entityInfo);
            Assert.IsNull(detachedEntityInfo);
        }
    }
}