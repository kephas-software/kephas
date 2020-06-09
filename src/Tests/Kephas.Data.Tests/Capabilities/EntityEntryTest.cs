// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityEntryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity entry test class.
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
    public class EntityEntryTest
    {
        [Test]
        public void IsChanged_IInstance_entity()
        {
            var entity = new DerivedTestEntity();
            entity.Id = Guid.NewGuid();
            entity.Name = "test";
            var entityEntry = new EntityEntry(entity);

            entity.Name = "another test";

            Assert.IsFalse(entityEntry.IsChanged(nameof(entity.Id)));
            Assert.IsTrue(entityEntry.IsChanged(nameof(entity.Name)));
        }

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
            var entityEntry = new EntityEntry(entity);

            var originalEntity = entityEntry.OriginalEntity;
            Assert.AreEqual(1, originalEntity.ToDictionary().Keys.Count);

            entity[namePropInfo.Name] = "belogea";
            entityEntry.DiscardChanges();

            Assert.AreEqual("gigi", entity[namePropInfo.Name]);
        }

        [Test]
        public void OriginalEntity_inherited_entity()
        {
            var entity = new DerivedTestEntity();
            entity.Id = Guid.NewGuid();
            entity.Name = "test";
            var entityEntry = new EntityEntry(entity);

            var originalEntity = entityEntry.OriginalEntity;
            Assert.AreEqual(2, originalEntity.ToDictionary().Keys.Count);

            Assert.AreEqual(entity.Id, originalEntity[nameof(DerivedTestEntity.Id)]);
            Assert.AreEqual(entity.Name, originalEntity[nameof(DerivedTestEntity.Name)]);
        }

        [Test]
        public void EntityId_identifiable()
        {
            var entity = Substitute.For<IIdentifiable>();
            entity.Id.Returns(3);
            var entityEntry = new EntityEntry(entity);

            Assert.AreEqual(3, entityEntry.EntityId);
        }

        [Test]
        public void EntityId_dynamic_entity_with_id()
        {
            dynamic entity = new Expando();
            entity.Id = "aha";
            var entityEntry = new EntityEntry(entity);

            Assert.AreEqual("aha", entityEntry.EntityId);
        }

        [Test]
        public void EntityId_entity_with_id()
        {
            var guid = Guid.NewGuid();
            var entity = new TestEntity { Id = guid };
            var entityEntry = new EntityEntry(entity);

            Assert.AreEqual(guid, entityEntry.EntityId);
        }

        [Test]
        public void EntityId_entity_without_id()
        {
            var entity = "aha";
            var entityEntry = new EntityEntry(entity);

            Assert.IsNull(entityEntry.EntityId);
        }

        [Test]
        public void ChangeState_trackable_entity()
        {
            var changeState = ChangeState.Added;
            var entity = Substitute.For<IChangeStateTrackable>();
            entity.ChangeState = Arg.Do<ChangeState>(ci => changeState = ci);
            entity.ChangeState.Returns(ci => changeState);
            var entityEntry = new EntityEntry(entity);

            Assert.AreEqual(ChangeState.Added, entityEntry.ChangeState);

            entityEntry.ChangeState = ChangeState.NotChanged;

            Assert.AreEqual(ChangeState.NotChanged, entityEntry.ChangeState);
            Assert.AreEqual(ChangeState.NotChanged, entity.ChangeState);
        }

        [Test]
        public void ChangeState_non_trackable_entity()
        {
            var entity = "abc";
            var entityEntry = new EntityEntry(entity);

            Assert.AreEqual(ChangeState.NotChanged, entityEntry.ChangeState);

            entityEntry.ChangeState = ChangeState.Added;

            Assert.AreEqual(ChangeState.Added, entityEntry.ChangeState);
        }

        [Test]
        public void GetGraphRoot_non_aggregatable()
        {
            var entityEntry = new EntityEntry("123");
            Assert.IsNull(entityEntry.GetGraphRoot());
        }

        [Test]
        public async Task GetFlattenedEntityGraphAsync_non_aggregatable()
        {
            var entityEntry = new EntityEntry("123");
            var graph = await entityEntry.GetFlattenedEntityGraphAsync(new GraphOperationContext(Substitute.For<IDataContext>()));

            Assert.AreEqual(1, graph.Count());
            Assert.AreEqual("123", graph.First());
        }

        [Test]
        public void GetStructuralEntityGraph_non_aggregatable()
        {
            var entityEntry = new EntityEntry("123");
            var graph = entityEntry.GetStructuralEntityGraph();

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
            var entityEntry = new EntityEntry(entity);
            Assert.AreSame(rootEntity, entityEntry.GetGraphRoot());
        }

        [Test]
        public async Task GetFlattenedEntityGraphAsync_aggregatable()
        {
            var entity = Substitute.For<IAggregatable>();
            var rootEntity = Substitute.For<IAggregatable>();
            entity.GetFlattenedEntityGraphAsync(Arg.Any<IGraphOperationContext>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IEnumerable<object>>(new[] { rootEntity, entity }));
            var entityEntry = new EntityEntry(entity);

            var graph = (await entityEntry.GetFlattenedEntityGraphAsync(null, CancellationToken.None)).ToList();
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
            var entityEntry = new EntityEntry(entity);

            var graph = entityEntry.GetStructuralEntityGraph().ToList();
            Assert.AreEqual(2, graph.Count);
            Assert.AreSame(rootEntity, graph[0]);
            Assert.AreSame(entity, graph[1]);
        }

        [Test]
        public void IsNotifyPropertyChangedSensitive()
        {
            var entity = new TestEntity();
            var entityEntry = new EntityEntry(entity);
            entity.Id = Guid.NewGuid();

            Assert.AreEqual(ChangeState.Changed, entityEntry.ChangeState);
        }

        [Test]
        public void AcceptChanges()
        {
            var originalGuid = Guid.NewGuid();
            var entity = new TestEntity { Id = originalGuid };
            var entityEntry = new EntityEntry(entity);
            var newGuid = Guid.NewGuid();
            entity.Id = newGuid;

            entityEntry.AcceptChanges();

            Assert.AreEqual(newGuid, entity.Id);
            Assert.AreEqual(ChangeState.NotChanged, entityEntry.ChangeState);
            Assert.AreEqual(newGuid, entityEntry.OriginalEntity["Id"]);
        }

        [Test]
        public void DiscardChanges_Added()
        {
            var entity = "123";
            var entityEntry = new EntityEntry(entity)
            {
                ChangeState = ChangeState.Added
            };

            entityEntry.DiscardChanges();

            Assert.AreEqual(ChangeState.NotChanged, entityEntry.ChangeState);
        }

        [Test]
        public void DiscardChanges_Changed()
        {
            var originalGuid = Guid.NewGuid();
            var entity = new TestEntity { Id = originalGuid };
            var entityEntry = new EntityEntry(entity);
            var newGuid = Guid.NewGuid();
            entity.Id = newGuid;

            entityEntry.DiscardChanges();

            Assert.AreEqual(originalGuid, entity.Id);
            Assert.AreEqual(ChangeState.NotChanged, entityEntry.ChangeState);
            Assert.AreEqual(originalGuid, entityEntry.OriginalEntity["Id"]);
        }

        [Test]
        public void DiscardChanges_Changed_entity_with_readOnlyProperties()
        {
            var entity = "123";
            var entityEntry = new EntityEntry(entity) { ChangeState = ChangeState.Changed };

            entityEntry.DiscardChanges();

            Assert.AreEqual(ChangeState.NotChanged, entityEntry.ChangeState);
        }

        [Test]
        public void DiscardChanges_Deleted()
        {
            var entity = "123";
            var entityEntry = new EntityEntry(entity)
            {
                ChangeState = ChangeState.Deleted
            };

            entityEntry.DiscardChanges();

            Assert.AreEqual(ChangeState.NotChanged, entityEntry.ChangeState);
        }

        private IPropertyInfo CreatePropertyInfo<TValue>(string name, Func<TValue> getter = null, Action<TValue> setter = null)
        {
            var propInfo = Substitute.For<IPropertyInfo>();
            propInfo.Name.Returns(name);
            propInfo.ValueType.Returns(typeof(TValue).AsRuntimeTypeInfo());
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
            private readonly ITypeInfo? typeInfo;

            public InstanceEntity()
            {
            }

            public InstanceEntity(ITypeInfo typeInfo)
            {
                this.typeInfo = typeInfo;
            }

            protected override bool TrySetValue(string key, object? value)
            {
                this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(key));
                bool result;
                if (this.typeInfo == null)
                {
                    result = base.TrySetValue(key, value);
                }
                else
                {
                    this.typeInfo.Properties.Single(p => p.Name == key).SetValue(this, value);
                    result = true;
                }

                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(key));

                return result;
            }

            protected override bool TryGetValue(string key, out object? value)
            {
                bool result;
                if (this.typeInfo == null)
                {
                    result = base.TryGetValue(key, out value);
                }
                else
                {
                    value = this.typeInfo.Properties.Single(p => p.Name == key).GetValue(this);
                    result = true;
                }

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