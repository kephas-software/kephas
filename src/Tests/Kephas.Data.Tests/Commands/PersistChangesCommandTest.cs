﻿// --------------------------------------------------------------------------------------------------------------------
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
    using Kephas.Operations;
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
            dataContext.Attach("123").ChangeState = ChangeState.Added;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            Assert.AreEqual("Persisted 1 changes.", result.Messages.First().Message);
            Assert.IsFalse(result.HasErrors());
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
            dataContext.Attach("123").ChangeState = ChangeState.Changed;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            Assert.AreEqual("Persisted 1 changes.", result.Messages.First().Message);
            Assert.IsFalse(result.HasErrors());
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
            dataContext.Attach("123").ChangeState = ChangeState.Deleted;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            Assert.AreEqual("Persisted 1 changes.", result.Messages.First().Message);
            Assert.IsFalse(result.HasErrors());
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public async Task ExecuteAsync_pre_persist_change_state_properly_set()
        {
            var behaviorProvider = Substitute.For<IDataBehaviorProvider>();
            var cmd = new PersistChangesCommand(behaviorProvider);

            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);
            var entityEntries = new IEntityEntry[5];
            (entityEntries[0] = dataContext.Attach("123")).ChangeState = ChangeState.Added;
            (entityEntries[1] = dataContext.Attach("abc")).ChangeState = ChangeState.Changed;
            (entityEntries[2] = dataContext.Attach("-123")).ChangeState = ChangeState.Deleted;
            (entityEntries[3] = dataContext.Attach("123abc")).ChangeState = ChangeState.AddedOrChanged;
            (entityEntries[4] = dataContext.Attach("same")).ChangeState = ChangeState.NotChanged;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            Assert.AreEqual(ChangeState.Added, entityEntries[0].PrePersistChangeState);
            Assert.AreEqual(ChangeState.Changed, entityEntries[1].PrePersistChangeState);
            Assert.AreEqual(ChangeState.Deleted, entityEntries[2].PrePersistChangeState);
            Assert.AreEqual(ChangeState.AddedOrChanged, entityEntries[3].PrePersistChangeState);
            Assert.AreEqual(ChangeState.NotChanged, entityEntries[4].PrePersistChangeState);
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
            dataContext.Attach("123").ChangeState = ChangeState.Added;

            var context = new PersistChangesContext(dataContext);
            var result = await cmd.ExecuteAsync(context);

            behavior.Received(1).BeforePersistAsync(Arg.Any<object>(), Arg.Any<IEntityEntry>(), Arg.Any<IDataOperationContext>(), Arg.Any<CancellationToken>());
            behavior.Received(1).AfterPersistAsync(Arg.Any<object>(), Arg.Any<IEntityEntry>(), Arg.Any<IDataOperationContext>(), Arg.Any<CancellationToken>());
        }
    }
}