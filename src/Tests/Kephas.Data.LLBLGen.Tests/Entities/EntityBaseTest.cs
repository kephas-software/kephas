// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the entity base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Tests.Entities
{
    using Kephas.Data.Capabilities;
    using Kephas.Data.LLBLGen.Entities;

    using NUnit.Framework;

    [TestFixture]
    public class EntityBaseTest
    {
        [Test]
        public void ChangeState_new()
        {
            var entity = new TestEntity();
            entity.IsNew = true;

            Assert.AreEqual(ChangeState.Added, entity.ChangeState);
        }

        [Test]
        public void Id_zero_when_new()
        {
            var entity = new TestEntity();
            Assert.AreEqual(0, ((IEntityBase)entity).Id);
            Assert.AreEqual(0, ((IIdentifiable)entity).Id);
        }

        [Test]
        public void Id_sync()
        {
            var entity = new TestEntity();

            ((IEntityBase)entity).Id = 12;

            Assert.AreEqual(12, entity.Id);
            Assert.AreEqual(12, ((IEntityBase)entity).Id);
            Assert.AreEqual(12, ((IIdentifiable)entity).Id);
        }

        [Test]
        public void ImplementedInterfaces()
        {
            var entity = new TestEntity();

            Assert.IsInstanceOf<IEntityBase>(entity);
            Assert.IsInstanceOf<IIdentifiable>(entity);
            Assert.IsInstanceOf<IEntityEntryAware>(entity);
            Assert.IsInstanceOf<IInstance>(entity);
            Assert.IsInstanceOf<IChangeStateTrackable>(entity);
        }

        public class TestEntity : EntityBase
        {
            /// <summary>
            /// Gets the entity static meta data instance from the generated type.
            /// </summary>
            /// <returns>The instance requested</returns>
            protected override EntityStaticMetaDataBase GetEntityStaticMetaData()
            {
                throw new System.NotImplementedException();
            }

            public long Id { get; set; }
        }
    }
}