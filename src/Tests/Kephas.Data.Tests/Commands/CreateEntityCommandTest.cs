// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateEntityCommandTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Data.Commands.Factory;

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
            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>(), localCache);
            var cmd = new CreateEntityCommand(behaviorProvider);

            var result = await cmd.ExecuteAsync(new CreateEntityContext<TestEntity>(dataContext));
            var newEntity = result.Entity;
            var newEntityInfo = result.EntityInfo;

            Assert.IsNotNull(newEntity);
            Assert.IsInstanceOf<TestEntity>(newEntity);
            Assert.IsNotNull(newEntityInfo);
            Assert.AreSame(newEntity, newEntityInfo.Entity);
            Assert.AreEqual(ChangeState.Added, newEntityInfo.ChangeState);
            Assert.AreEqual(1, localCache.Count);
            Assert.AreSame(newEntityInfo, localCache.First().Value);
        }

        [Test]
        public async Task ExecuteAsync_with_init_behaviors()
        {
            object behaviorEntity = null;
            var initCalls = 0;
            var onInitBehavior = Substitute.For<IOnInitializeBehavior>();
            onInitBehavior
                .InitializeAsync(Arg.Any<object>(), Arg.Any<IDataOperationContext>(), Arg.Any<CancellationToken>())
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

            var dataContext = new TestDataContext(Substitute.For<IAmbientServices>(), Substitute.For<IDataCommandProvider>());
            var cmd = new CreateEntityCommand(behaviorProvider);

            var result = await cmd.ExecuteAsync(new CreateEntityContext<TestEntity>(dataContext));
            var newEntity = result.Entity;

            Assert.AreSame(newEntity, behaviorEntity);
            Assert.AreEqual(1, initCalls);
        }

        public class TestEntity {}
    }
}