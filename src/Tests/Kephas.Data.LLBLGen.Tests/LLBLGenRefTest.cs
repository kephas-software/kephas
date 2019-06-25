// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenRefTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the llbl generate reference test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Tests
{
    using Kephas.Data.Capabilities;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class LLBLGenRefTest
    {
        [Test]
        public void Id()
        {
            var entity = Substitute.For<TestEntity, IEntityEntryAware>();
            var entry = new EntityEntry(entity);
            (entity as IEntityEntryAware).GetEntityEntry().Returns(entry);

            var eref = new LLBLGenRef<IIdentifiable>((IEntityEntryAware)entity, nameof(TestEntity.ParentId));

            entity.ParentId = 123;
            Assert.AreEqual(123, eref.Id);
        }

        public class TestEntity : IIdentifiable
        {
            public object Id { get; set; }

            public long? ParentId { get; set; }
        }
    }
}