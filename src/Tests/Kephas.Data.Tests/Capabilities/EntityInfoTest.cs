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
    using Kephas.Reflection;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class EntityInfoTest
    {
        [Test]
        public void OriginalEntity_IInstance_entity()
        {
            var collector = new Expando();
            var namePropInfo = this.CreatePropertyInfo("Name", () => (string)collector["Name"], value => collector["Name"] = value);
            var dynTypeInfo = Substitute.For<ITypeInfo>();
            dynTypeInfo.GetMember(Arg.Any<string>(), Arg.Any<bool>()).Returns(namePropInfo);
            dynTypeInfo.Properties.Returns(new[] { namePropInfo });
            var entity = new InstanceEntity(dynTypeInfo);
            entity[namePropInfo.Name] = "gigi";
            var entityInfo = new EntityInfo(entity);

            var originalEntity = entityInfo.OriginalEntity;
            Assert.AreEqual(1, originalEntity.ToDictionary().Keys.Count);

            entity[namePropInfo.Name] = "belogea";
            entityInfo.DiscardChanges();

            Assert.AreEqual("gigi", entity[namePropInfo.Name]);
        }

        [Test]
        public void OriginalEntity_inherited_entity()
        {
            var entity = new DerivedTestEntity();
            entity.Id = Guid.NewGuid();
            entity.Name = "test";
            var entityInfo = new EntityInfo(entity);

            var originalEntity = entityInfo.OriginalEntity;
            Assert.AreEqual(2, originalEntity.ToDictionary().Keys.Count);

            Assert.AreEqual(entity.Id, originalEntity[nameof(DerivedTestEntity.Id)]);
            Assert.AreEqual(entity.Name, originalEntity[nameof(DerivedTestEntity.Name)]);
        }

        [Test]
        public void EntityId_identifiable()
        {
            var entity = Substitute.For<IIdentifiable>();
            entity.Id.Returns(3);
            var entityInfo = new EntityInfo(entity);

            Assert.AreEqual(3, entityInfo.EntityId);
        }

        [Test]
        public void EntityId_dynamic_entity_with_id()
        {
            dynamic entity = new Expando();
            entity.Id = "aha";
            var entityInfo = new EntityInfo(entity);

            Assert.AreEqual("aha", entityInfo.EntityId);
        }

        [Test]
        public void EntityId_entity_with_id()
        {
            var guid = Guid.NewGuid();
            var entity = new TestEntity { Id = guid };
            var entityInfo = new EntityInfo(entity);

            Assert.AreEqual(guid, entityInfo.EntityId);
        }

        [Test]
        public void EntityId_entity_without_id()
        {
            var entity = "aha";
            var entityInfo = new EntityInfo(entity);

            Assert.IsNull(entityInfo.EntityId);
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
        public void DiscardChanges_Added()
        {
            var entity = "123";
            var entityInfo = new EntityInfo(entity)
                                 {
                                     ChangeState = ChangeState.Added
                                 };

            entityInfo.DiscardChanges();

            Assert.AreEqual(ChangeState.NotChanged, entityInfo.ChangeState);
        }

        [Test]
        public void DiscardChanges_Changed()
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

        [Test]
        public void DiscardChanges_Changed_entity_with_readOnlyProperties()
        {
            var entity = "123";
            var entityInfo = new EntityInfo(entity) { ChangeState = ChangeState.Changed };

            entityInfo.DiscardChanges();

            Assert.AreEqual(ChangeState.NotChanged, entityInfo.ChangeState);
        }

        [Test]
        public void DiscardChanges_Deleted()
        {
            var entity = "123";
            var entityInfo = new EntityInfo(entity)
                                 {
                                     ChangeState = ChangeState.Deleted
                                 };

            entityInfo.DiscardChanges();

            Assert.AreEqual(ChangeState.NotChanged, entityInfo.ChangeState);
        }

        private IPropertyInfo CreatePropertyInfo<TValue>(string name, Func<TValue> getter = null, Action<TValue> setter = null)
        {
            var propInfo = Substitute.For<IPropertyInfo>();
            propInfo.Name.Returns(name);
            propInfo.PropertyType.Returns(typeof(TValue).AsRuntimeTypeInfo());
            propInfo.CanRead.Returns(true);
            propInfo.CanWrite.Returns(true);
            propInfo.GetValue(Arg.Any<object>()).Returns(ci => getter == null ? default(TValue) : getter());
            propInfo.When(p => p.SetValue(Arg.Any<object>(), Arg.Any<object>())).Do(ci => setter?.Invoke((TValue)ci.Args()[1]));

            return propInfo;
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
                    this.OnPropertyChanging(new PropertyChangingEventArgs(nameof(this.Id)));
                    this.id = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Id)));
                }
            }

            public event PropertyChangingEventHandler PropertyChanging;

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
            {
                this.PropertyChanging?.Invoke(this, e);
            }

            protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                this.PropertyChanged?.Invoke(this, e);
            }
        }

        public class DerivedTestEntity : TestEntity
        {
            private string name;

            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    this.OnPropertyChanging(new PropertyChangingEventArgs(nameof(this.Name)));
                    this.name = value;
                    this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(this.Name)));
                }
            }
        }

        public class InstanceEntity : Expando, INotifyPropertyChanging, INotifyPropertyChanged, IInstance
        {
            private readonly ITypeInfo typeInfo;

            public InstanceEntity()
            {
            }

            public InstanceEntity(ITypeInfo typeInfo)
            {
                this.typeInfo = typeInfo;
            }

            /// <summary>
            /// Attempts to set the value with the given key.
            /// </summary>
            /// <remarks>
            /// First of all, it is tried to set the property value to the inner object, if one is set.
            /// The next try is to set the property value to the expando object itself.
            /// Lastly, if still a property by the provided name cannot be found, the inner dictionary is used to set the value with the provided key.
            /// </remarks>
            /// <param name="key">The key.</param>
            /// <param name="value">The value to set.</param>
            /// <returns>
            /// <c>true</c> if the value could be set, <c>false</c> otherwise.
            /// </returns>
            protected override bool TrySetValue(string key, object value)
            {
                this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(key));
                var result = base.TrySetValue(key, value);
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));
                return result;
            }

            public event PropertyChangingEventHandler PropertyChanging;

            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets the type information for this instance.
            /// </summary>
            /// <returns>
            /// The type information.
            /// </returns>
            public ITypeInfo GetTypeInfo() => this.typeInfo ?? this.GetRuntimeTypeInfo();
        }
    }
}