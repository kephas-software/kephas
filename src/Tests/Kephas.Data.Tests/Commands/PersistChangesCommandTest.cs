// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesCommandTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the persist changes command test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class PersistChangesCommandTest
    {
        [Test]
        public async Task ExecuteAsync_added()
        {
            var behaviorProvider = Substitute.For<IDataBehaviorProvider>();
            var cmd = new PersistChangesCommand(behaviorProvider);

            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);
            dataContext.AttachEntity("123").ChangeState = ChangeState.Added;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            Assert.AreEqual("Persisted 1 changes.", result.Message);
            Assert.IsNull(result.Exception);
            Assert.AreEqual(1, localCache.Count);
            Assert.IsTrue(localCache.Values.All(e => e.ChangeState == ChangeState.NotChanged));
        }

        [Test]
        public async Task ExecuteAsync_modified()
        {
            var behaviorProvider = Substitute.For<IDataBehaviorProvider>();
            var cmd = new PersistChangesCommand(behaviorProvider);

            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);
            dataContext.AttachEntity("123").ChangeState = ChangeState.Changed;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            Assert.AreEqual("Persisted 1 changes.", result.Message);
            Assert.IsNull(result.Exception);
            Assert.AreEqual(1, localCache.Count);
            Assert.IsTrue(localCache.Values.All(e => e.ChangeState == ChangeState.NotChanged));
        }

        [Test]
        public async Task ExecuteAsync_deleted()
        {
            var behaviorProvider = Substitute.For<IDataBehaviorProvider>();
            var cmd = new PersistChangesCommand(behaviorProvider);

            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);
            dataContext.AttachEntity("123").ChangeState = ChangeState.Deleted;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            Assert.AreEqual("Persisted 1 changes.", result.Message);
            Assert.IsNull(result.Exception);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public async Task ExecuteAsync_pre_persist_change_state_properly_set()
        {
            var behaviorProvider = Substitute.For<IDataBehaviorProvider>();
            var cmd = new PersistChangesCommand(behaviorProvider);

            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);
            var entityInfos = new IEntityEntry[5];
            (entityInfos[0] = dataContext.AttachEntity("123")).ChangeState = ChangeState.Added;
            (entityInfos[1] = dataContext.AttachEntity("abc")).ChangeState = ChangeState.Changed;
            (entityInfos[2] = dataContext.AttachEntity("-123")).ChangeState = ChangeState.Deleted;
            (entityInfos[3] = dataContext.AttachEntity("123abc")).ChangeState = ChangeState.AddedOrChanged;
            (entityInfos[4] = dataContext.AttachEntity("same")).ChangeState = ChangeState.NotChanged;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            Assert.AreEqual(ChangeState.Added, entityInfos[0].PrePersistChangeState);
            Assert.AreEqual(ChangeState.Changed, entityInfos[1].PrePersistChangeState);
            Assert.AreEqual(ChangeState.Deleted, entityInfos[2].PrePersistChangeState);
            Assert.AreEqual(ChangeState.AddedOrChanged, entityInfos[3].PrePersistChangeState);
            Assert.AreEqual(ChangeState.NotChanged, entityInfos[4].PrePersistChangeState);
        }

        [Test]
        public async Task ExecuteAsync_added_with_behaviors()
        {
            var behavior = Substitute.For<IOnPersistBehavior>();
            behavior.BeforePersistAsync(Arg.Any<object>(), Arg.Any<IEntityEntry>(), Arg.Any<IDataOperationContext>(), Arg.Any<CancellationToken>())
                    .Returns(ci => Task.FromResult(0));
            behavior.AfterPersistAsync(Arg.Any<object>(), Arg.Any<IEntityEntry>(), Arg.Any<IDataOperationContext>(), Arg.Any<CancellationToken>())
                    .Returns(ci => Task.FromResult(0));
            var behaviorProvider = Substitute.For<IDataBehaviorProvider>();
            behaviorProvider.GetDataBehaviors<IOnPersistBehavior>(typeof(string))
                .Returns(new List<IOnPersistBehavior> { behavior });
            var cmd = new PersistChangesCommand(behaviorProvider);

            var dataContext = new TestDataContext();
            dataContext.AttachEntity("123").ChangeState = ChangeState.Added;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            behavior.Received(1).BeforePersistAsync(Arg.Any<object>(), Arg.Any<IEntityEntry>(), Arg.Any<IDataOperationContext>(), Arg.Any<CancellationToken>());
            behavior.Received(1).AfterPersistAsync(Arg.Any<object>(), Arg.Any<IEntityEntry>(), Arg.Any<IDataOperationContext>(), Arg.Any<CancellationToken>());
        }
    }
}