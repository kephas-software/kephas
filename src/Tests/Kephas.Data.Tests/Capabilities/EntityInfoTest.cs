// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityInfoTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the entity information test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Capabilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Dynamic;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class EntityInfoTest
    {
        [Test]
        public void EntityId_identifiable()
        {
            var entity = Substitute.For<IIdentifiable>();
            entity.Id.Returns(new Id(3));
            var entityInfo = new EntityInfo(entity);

            Assert.AreEqual(new Id(3), entityInfo.EntityId);
        }

        [Test]
        public void EntityId_dynamic_entity_with_id()
        {
            dynamic entity = new Expando();
            entity.Id = "aha";
            var entityInfo = new EntityInfo(entity);

            Assert.AreEqual(new Id("aha"), entityInfo.EntityId);
        }

        [Test]
        public void EntityId_entity_with_id()
        {
            var guid = Guid.NewGuid();
            var entity = new TestEntity { Id = guid };
            var entityInfo = new EntityInfo(entity);

            Assert.AreEqual(new Id(guid), entityInfo.EntityId);
        }

        [Test]
        public void EntityId_entity_without_id()
        {
            var entity = "aha";
            var entityInfo = new EntityInfo(entity);

            Assert.AreEqual(entityInfo.Id, entityInfo.EntityId);
        }

        [Test]
        public void ChangeState_trackable_entity()
        {
            var changeState = ChangeState.Added;
            var entity = Substitute.For<IChangeStateTrackable>();
            entity.ChangeState = Arg.Do<ChangeState>(ci => changeState = ci);
            entity.ChangeState.Returns(ci => changeState);
            var entityInfo = new EntityInfo(entity);

            Assert.AreEqual(ChangeState.Added, entityInfo.ChangeState);

            entityInfo.ChangeState = ChangeState.NotChanged;

            Assert.AreEqual(ChangeState.NotChanged, entityInfo.ChangeState);
            Assert.AreEqual(ChangeState.NotChanged, entity.ChangeState);
        }

        [Test]
        public void ChangeState_non_trackable_entity()
        {
            var entity = "abc";
            var entityInfo = new EntityInfo(entity);

            Assert.AreEqual(ChangeState.NotChanged, entityInfo.ChangeState);

            entityInfo.ChangeState = ChangeState.Added;

            Assert.AreEqual(ChangeState.Added, entityInfo.ChangeState);
        }

        [Test]
        public void GetGraphRoot_non_aggregatable()
        {
            var entityInfo = new EntityInfo("123");
            Assert.IsNull(entityInfo.GetGraphRoot());
        }

        [Test]
        public async Task GetFlattenedGraphAsync_non_aggregatable()
        {
            var entityInfo = new EntityInfo("123");
            var graph = await entityInfo.GetFlattenedGraphAsync(new GraphOperationContext(Substitute.For<IDataContext>()));

            Assert.AreEqual(1, graph.Count());
            Assert.AreEqual("123", graph.First());
        }

        [Test]
        public void GetGraphRoot_aggregatable()
        {
            var entity = Substitute.For<IAggregatable>();
            var rootEntity = Substitute.For<IAggregatable>();
            entity.GetGraphRoot()
                .Returns(rootEntity);
            var entityInfo = new EntityInfo(entity);
            Assert.AreSame(rootEntity, entityInfo.GetGraphRoot());
        }

        [Test]
        public async Task GetFlattenedGraphAsync_aggregatable()
        {
            var entity = Substitute.For<IAggregatable>();
            var rootEntity = Substitute.For<IAggregatable>();
            entity.GetFlattenedGraphAsync(Arg.Any<IGraphOperationContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<object>>(new[] { rootEntity, entity }));
            var entityInfo = new EntityInfo(entity);

            var graph = (await entityInfo.GetFlattenedGraphAsync(null, CancellationToken.None)).ToList();
            Assert.AreEqual(2, graph.Count);
            Assert.AreSame(rootEntity, graph[0]);
            Assert.AreSame(entity, graph[1]);
        }

        public class TestEntity
        {
            public Guid Id { get; set; }
        }
    }
}