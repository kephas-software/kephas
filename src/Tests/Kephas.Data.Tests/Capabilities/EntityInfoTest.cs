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
    using System.ComponentModel;
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
        public async Task GetFlattenedEntityGraphAsync_non_aggregatable()
        {
            var entityInfo = new EntityInfo("123");
            var graph = await entityInfo.GetFlattenedEntityGraphAsync(new GraphOperationContext(Substitute.For<IDataContext>()));

            Assert.AreEqual(1, graph.Count());
            Assert.AreEqual("123", graph.First());
        }

        [Test]
        public void GetStructuralEntityGraph_non_aggregatable()
        {
            var entityInfo = new EntityInfo("123");
            var graph = entityInfo.GetStructuralEntityGraph();

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
        public async Task GetFlattenedEntityGraphAsync_aggregatable()
        {
            var entity = Substitute.For<IAggregatable>();
            var rootEntity = Substitute.For<IAggregatable>();
            entity.GetFlattenedEntityGraphAsync(Arg.Any<IGraphOperationContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<object>>(new[] { rootEntity, entity }));
            var entityInfo = new EntityInfo(entity);

            var graph = (await entityInfo.GetFlattenedEntityGraphAsync(null, CancellationToken.None)).ToList();
            Assert.AreEqual(2, graph.Count);
            Assert.AreSame(rootEntity, graph[0]);
            Assert.AreSame(entity, graph[1]);
        }

        [Test]
        public void GetStructuralEntityGraph_aggregatable()
        {
            var entity = Substitute.For<IAggregatable>();
            var rootEntity = Substitute.For<IAggregatable>();
            entity.GetStructuralEntityGraph()
                .Returns(new[] { rootEntity, entity });
            var entityInfo = new EntityInfo(entity);

            var graph = entityInfo.GetStructuralEntityGraph().ToList();
            Assert.AreEqual(2, graph.Count);
            Assert.AreSame(rootEntity, graph[0]);
            Assert.AreSame(entity, graph[1]);
        }

        [Test]
        public void IsNotifyPropertyChangedSensitive()
        {
            var entity = new TestEntity();
            var entityInfo = new EntityInfo(entity);
            entity.Id = Guid.NewGuid();

            Assert.AreEqual(ChangeState.Changed, entityInfo.ChangeState);
        }

        [Test]
        public void AcceptChanges()
        {
            var originalGuid = Guid.NewGuid();
            var entity = new TestEntity { Id = originalGuid };
            var entityInfo = new EntityInfo(entity);
            var newGuid = Guid.NewGuid();
            entity.Id = newGuid;

            entityInfo.AcceptChanges();

            Assert.AreEqual(newGuid, entity.Id);
            Assert.AreEqual(ChangeState.NotChanged, entityInfo.ChangeState);
            Assert.AreEqual(newGuid, entityInfo.OriginalEntity["Id"]);
        }

        [Test]
        public void DiscardChanges()
        {
            var originalGuid = Guid.NewGuid();
            var entity = new TestEntity { Id = originalGuid };
            var entityInfo = new EntityInfo(entity);
            var newGuid = Guid.NewGuid();
            entity.Id = newGuid;

            entityInfo.DiscardChanges();

            Assert.AreEqual(originalGuid, entity.Id);
            Assert.AreEqual(ChangeState.NotChanged, entityInfo.ChangeState);
            Assert.AreEqual(originalGuid, entityInfo.OriginalEntity["Id"]);
        }

        public class TestEntity : INotifyPropertyChanging, INotifyPropertyChanged
        {
            private Guid id;

            public Guid Id
            {
                get
                {
                    return this.id;
                }
                set
                {
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs("Id"));
                    this.id = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Id"));
                }
            }

            public event PropertyChangingEventHandler PropertyChanging;

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}