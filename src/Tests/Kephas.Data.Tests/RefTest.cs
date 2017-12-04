// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RefTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the reference test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class RefTest
    {
        [Test]
        public void IsEmpty()
        {
            var dataContext = Substitute.For<IDataContext>();

            var entity = new TestEntity(dataContext);
            var r = new Ref<RefEntity>(entity, nameof(TestEntity.ReferenceId));

            r.Id = null;
            Assert.IsTrue(r.IsEmpty);

            r.Id = 0;
            Assert.IsTrue(r.IsEmpty);

            r.Id = Guid.Empty;
            Assert.IsTrue(r.IsEmpty);

            r.Id = -1;
            Assert.IsFalse(r.IsEmpty);
        }

        [Test]
        public async Task GetAsync_null_reference()
        {
            var refEntity = new RefEntity();
            var dataContext = this.CreateDataContext((_, throwOnNotFound) => refEntity);

            var entity = new TestEntity(dataContext);
            var r = new Ref<RefEntity>(entity, nameof(TestEntity.ReferenceId));

            var result = await r.GetAsync();

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAsync_resolved_reference()
        {
            var refEntity = new RefEntity();
            var dataContext = this.CreateDataContext((id, _) => id.Equals(1) ? refEntity : throw new KeyNotFoundException("ex"));

            var entity = new TestEntity(dataContext) { ReferenceId = 1 };
            var r = new Ref<RefEntity>(entity, nameof(TestEntity.ReferenceId));

            var result = await r.GetAsync();

            Assert.AreSame(refEntity, result);
        }

        [Test]
        public async Task GetAsync_not_resolved_reference()
        {
            var refEntity = new RefEntity();
            var dataContext = this.CreateDataContext((id, throwOnNotFound) => id.Equals(1) ? refEntity : throwOnNotFound ? throw new KeyNotFoundException("ex") : (object)null);

            var entity = new TestEntity(dataContext) { ReferenceId = 100 };
            var r = new Ref<RefEntity>(entity, nameof(TestEntity.ReferenceId));

            Assert.ThrowsAsync<KeyNotFoundException>(() => r.GetAsync());
        }

        [Test]
        public async Task GetAsync_not_resolved_reference_tollerant()
        {
            var refEntity = new RefEntity();
            var dataContext = this.CreateDataContext((id, throwOnNotFound) => id.Equals(1) ? refEntity : throwOnNotFound ? throw new KeyNotFoundException("ex") : (object)null);

            var entity = new TestEntity(dataContext) { ReferenceId = 100 };
            var r = new Ref<RefEntity>(entity, nameof(TestEntity.ReferenceId));

            var result = await r.GetAsync(throwIfNotFound: false);

            Assert.IsNull(result);
        }

        private IDataContext CreateDataContext(Func<object, bool, object> findResolver)
        {
            var findCommand = Substitute.For<IFindCommand>();
            findCommand.ExecuteAsync(Arg.Any<IDataOperationContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult<IDataCommandResult>(new FindResult(findResolver(ci.Arg<IFindContext>().Id, ci.Arg<IFindContext>().ThrowIfNotFound))));
            findCommand.ExecuteAsync(Arg.Any<IFindContext>(), Arg.Any<CancellationToken>())
                .Returns(ci => Task.FromResult<IFindResult>(new FindResult(findResolver(ci.Arg<IFindContext>().Id, ci.Arg<IFindContext>().ThrowIfNotFound))));
            var dataContext = Substitute.For<IDataContext>();
            dataContext.CreateCommand(typeof(IFindCommand)).Returns(ci => findCommand);

            return dataContext;
        }

        public class TestEntity : IEntityInfoAware, IIdentifiable
        {
            private IEntityInfo entityInfo;

            public TestEntity(IDataContext dataContext)
            {
                this.entityInfo = Substitute.For<IEntityInfo>();
                this.entityInfo.Entity.Returns(this);
                this.entityInfo.EntityId.Returns(ci => this.Id);
                this.entityInfo.DataContext.Returns(ci => dataContext);
            }

            public object Id { get; set; }

            public object ReferenceId { get; set; }

            public IEntityInfo GetEntityInfo() => this.entityInfo;

            public void SetEntityInfo(IEntityInfo entityInfo) => this.entityInfo = entityInfo;
        }

        public class RefEntity
        {
        }
    }
}