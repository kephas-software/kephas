﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System;
    using System.Linq;
    using System.Security.Principal;

    using Kephas.Activation;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Store;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Transitions;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class DataContextBaseTest
    {
        [Test]
        public void CreateCommand_success()
        {
            var dataCommandProvider = Substitute.For<IDataCommandProvider>();
            var findCmd = Substitute.For<IFindCommand>();
            dataCommandProvider.CreateCommand(Arg.Any<Type>(), typeof(IFindCommand)).Returns(findCmd);

            var dataContext = TestDataContext.InitializeDataContext(dataCommandProvider: dataCommandProvider);
            var cmd = dataContext.CreateCommand<IFindCommand>();
            Assert.AreSame(findCmd, cmd);
        }

        [Test]
        public void CreateCommand_not_initialized()
        {
            var dataCommandProvider = Substitute.For<IDataCommandProvider>();
            var findCmd = Substitute.For<IFindCommand>();
            dataCommandProvider.CreateCommand(Arg.Any<Type>(), typeof(IFindCommand)).Returns(findCmd);

            var dataContext = TestDataContext.CreateDataContext(dataCommandProvider: dataCommandProvider);
            Assert.Throws<ServiceTransitionException>(() => dataContext.CreateCommand<IFindCommand>());
        }

        [Test]
        public void Attach_object()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.InitializeDataContext(localCache: localCache);

            var entity = "hello";
            var entityEntry = dataContext.Attach(entity);
            Assert.AreEqual(1, localCache.Count);
            Assert.AreSame(entityEntry, localCache.First().Value);
            Assert.AreSame(entity, entityEntry.Entity);
        }

        [Test]
        public void Attach_graph()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.InitializeDataContext(localCache: localCache);

            var entity = Substitute.For<IAggregatable>();
            var entityPart = new object();
            entity.GetStructuralEntityGraph().Returns(new[] { entity, entityPart });
            var entityEntry = dataContext.Attach(entity);
            Assert.AreEqual(2, localCache.Count);
            Assert.AreSame(entityEntry, localCache.First().Value);
            Assert.AreSame(entity, entityEntry.Entity);
            Assert.AreSame(entityPart, localCache.Skip(1).First().Value.Entity);
        }

        [Test]
        public void Detach_object()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.InitializeDataContext(localCache: localCache);

            var entity = "hello";
            var entityEntry = dataContext.Attach(entity);

            var detachedEntityEntry = dataContext.Detach(entityEntry);
            Assert.AreSame(entityEntry, detachedEntityEntry);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void Detach_graph()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.InitializeDataContext(localCache: localCache);

            var entity = Substitute.For<IAggregatable>();
            var entityPart = new object();
            entity.GetStructuralEntityGraph().Returns(new[] { entity, entityPart });
            var entityEntry = dataContext.Attach(entity);

            var detachedEntityEntry = dataContext.Detach(entityEntry);
            Assert.AreSame(entityEntry, detachedEntityEntry);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void Detach_not_own_entity_entry()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.InitializeDataContext(localCache: localCache);

            var entityEntry = new EntityEntry("hello");

            var detachedEntityEntry = dataContext.Detach(entityEntry);
            Assert.IsNull(detachedEntityEntry);
        }

        [Test]
        public void Query_not_initialized()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.CreateDataContext(localCache: localCache);

            Assert.Throws<ServiceTransitionException>(() => dataContext.Query<string>());
        }

        [Test]
        public void Query_non_abstract_entity_queryable()
        {
            var typeRegistry = new RuntimeTypeRegistry();
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.CreateDataContext(localCache: localCache, typeRegistry: typeRegistry);
            var dataStore = Substitute.For<IDataStore>();
            var entityActivator = Substitute.For<IActivator>();
            entityActivator
                .GetImplementationType(typeRegistry.GetTypeInfo(typeof(IEntity)), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeRegistry.GetTypeInfo(typeof(Entity)));

            entityActivator
                .GetImplementationType(typeRegistry.GetTypeInfo(typeof(Entity)), Arg.Any<IContext>(), Arg.Any<bool>())
                .Returns(typeRegistry.GetTypeInfo(typeof(Entity)));

            dataStore.EntityActivator.Returns(entityActivator);
            dataContext.Initialize(dataContext.CreateDataInitializationContext(dataStore));

            var queryable = dataContext.Query<IEntity>();
            Assert.IsInstanceOf<IQueryable<Entity>>(queryable);
        }

        [Test]
        public void Query_with_behaviors()
        {
            var localCache = new DataContextCache();
            var behaviorProvider = Substitute.For<IDataBehaviorProvider>();

            var dataContext = TestDataContext.CreateDataContext(dataBehaviorProvider: behaviorProvider, localCache: localCache);
            dataContext.Initialize(dataContext.CreateDataInitializationContext(activator: TestDataContext.CreateIdempotentActivator()));

            var behavior = Substitute.For<IOnQueryBehavior>();
            behaviorProvider.GetDataBehaviors<IOnQueryBehavior>(typeof(IEntity)).Returns(new[] { behavior });

            var queryable = dataContext.Query<IEntity>();

            behavior.Received(1).BeforeQuery(typeof(IEntity), Arg.Any<IQueryOperationContext>());
            behavior.Received(1).AfterQuery(typeof(IEntity), Arg.Any<IQueryOperationContext>());
        }

        [Test]
        public void Initialize_bad_initialization_context()
        {
            var dataContext = TestDataContext.CreateDataContext();
            var context = Substitute.For<IContext>();
            Assert.Throws<ArgumentException>(() => dataContext.Initialize(context));
        }

        [Test]
        public void Initialize_identity_properly_set()
        {
            var identity = Substitute.For<IIdentity>();
            var dataContext = TestDataContext.CreateDataContext();
            dataContext.Initialize(dataContext.CreateDataInitializationContext(identity: identity));

            Assert.AreSame(identity, dataContext.Identity);
        }

        [Test]
        public void Initialize_identity_properly_set_from_inner_context()
        {
            var dataContext = TestDataContext.CreateDataContext();
            var identity = Substitute.For<IIdentity>();
            var context = Substitute.For<IContext>();
            context.Identity.Returns(identity);
            var initContext = Substitute.For<IDataInitializationContext>();
            initContext.Identity.Returns((IIdentity)null);
            initContext.InitializationContext.Returns(context);
            dataContext.Initialize(initContext);

            Assert.AreSame(identity, dataContext.Identity);
        }

        public interface IEntity
        {
        }

        public class Entity : IEntity
        {
        }
    }
}