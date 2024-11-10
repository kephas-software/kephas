// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityCommandTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the create entity command test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Commands
{
    using System;
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
    public class CreateEntityCommandTest
    {
        [Test]
        public async Task ExecuteAsync_success()
        {
            var behaviorProvider = Substitute.For<IDataBehaviorProvider>();
            behaviorProvider.GetDataBehaviors<IOnInitializeBehavior>(Arg.Any<Type>())
                .Returns(new List<IOnInitializeBehavior>());

            var localCache = new DataContextCache();
            var dataContext = TestDataContext.InitializeDataContext(localCache: localCache);
            var cmd = new CreateEntityCommand(behaviorProvider);

            var result = await cmd.ExecuteAsync(new CreateEntityContext<TestEntity>(dataContext));
            var newEntity = result.Entity;
            var newEntityEntry = result.EntityEntry;

            Assert.IsNotNull(newEntity);
            Assert.IsInstanceOf<TestEntity>(newEntity);
            Assert.IsNotNull(newEntityEntry);
            Assert.AreSame(newEntity, newEntityEntry.Entity);
            Assert.AreEqual(ChangeState.Added, newEntityEntry.ChangeState);
            Assert.AreEqual(1, localCache.Count);
            Assert.AreSame(newEntityEntry, localCache.First().Value);
        }

        [Test]
        public async Task ExecuteAsync_with_init_behaviors()
        {
            object behaviorEntity = null;
            var initCalls = 0;
            var onInitBehavior = Substitute.For<IOnInitializeBehavior>();
            onInitBehavior
                .InitializeAsync(Arg.Any<object>(), Arg.Any<IEntityEntry>(), Arg.Any<IDataOperationContext>(), Arg.Any<CancellationToken>())
                .Returns((Task)Task.FromResult(0))
                .AndDoes(
                    ci =>
                        {
                            behaviorEntity = ci.Args()[0];
                            initCalls++;
                        });

            var behaviorProvider = Substitute.For<IDataBehaviorProvider>();
            behaviorProvider.GetDataBehaviors<IOnInitializeBehavior>(Arg.Any<Type>())
                .Returns(new List<IOnInitializeBehavior> { onInitBehavior });

            var dataContext = TestDataContext.InitializeDataContext();
            var cmd = new CreateEntityCommand(behaviorProvider);

            var result = await cmd.ExecuteAsync(new CreateEntityContext<TestEntity>(dataContext));
            var newEntity = result.Entity;

            Assert.AreSame(newEntity, behaviorEntity);
            Assert.AreEqual(1, initCalls);
        }

        public class TestEntity {}
    }
}