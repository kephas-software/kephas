// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Services;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataContextBaseTest
    {
        [Test]
        public void CreateCommand()
        {
            var dataCommandProvider = Substitute.For<IDataCommandProvider>();
            var findCmd = Substitute.For<IFindCommand>();
            dataCommandProvider.CreateCommand(Arg.Any<Type>(), typeof(IFindCommand)).Returns(findCmd);

            var dataContext = new TestDataContext(dataCommandProvider: dataCommandProvider);
            var cmd = dataContext.CreateCommand<IFindCommand>();
            Assert.AreSame(findCmd, cmd);
        }

        [Test]
        public void AttachEntity_object()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = "hello";
            var entityInfo = dataContext.AttachEntity(entity);
            Assert.AreEqual(1, localCache.Count);
            Assert.AreSame(entityInfo, localCache.First().Value);
            Assert.AreSame(entity, entityInfo.Entity);
        }

        [Test]
        public void AttachEntity_graph()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = Substitute.For<IAggregatable>();
            var entityPart = new object();
            entity.GetStructuralEntityGraph().Returns(new[] { entity, entityPart });
            var entityInfo = dataContext.AttachEntity(entity);
            Assert.AreEqual(2, localCache.Count);
            Assert.AreSame(entityInfo, localCache.First().Value);
            Assert.AreSame(entity, entityInfo.Entity);
            Assert.AreSame(entityPart, localCache.Skip(1).First().Value.Entity);
        }

        [Test]
        public void DetachEntity_object()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = "hello";
            var entityInfo = dataContext.AttachEntity(entity);

            var detachedEntityInfo = dataContext.DetachEntity(entityInfo);
            Assert.AreSame(entityInfo, detachedEntityInfo);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void DetachEntity_graph()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = Substitute.For<IAggregatable>();
            var entityPart = new object();
            entity.GetStructuralEntityGraph().Returns(new[] { entity, entityPart });
            var entityInfo = dataContext.AttachEntity(entity);

            var detachedEntityInfo = dataContext.DetachEntity(entityInfo);
            Assert.AreSame(entityInfo, detachedEntityInfo);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void DetachEntity_not_own_entity_info()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entityInfo = new EntityInfo("hello");

            var detachedEntityInfo = dataContext.DetachEntity(entityInfo);
            Assert.IsNull(detachedEntityInfo);
        }

        [Test]
        public void Initialize_bad_initialization_context()
        {
            var dataContext = new TestDataContext();
            var context = Substitute.For<IContext>();
            Assert.Throws<ArgumentException>(() => dataContext.Initialize(context));
        }

        [Test]
        public void Initialize_identity_properly_set()
        {
            var dataContext = new TestDataContext();
            var identity = Substitute.For<IIdentity>();
            var context = Substitute.For<IDataInitializationContext>();
            context.Identity.Returns(identity);
            dataContext.Initialize(context);

            Assert.AreSame(identity, dataContext.Identity);
        }

        [Test]
        public void Initialize_identity_properly_set_from_inner_context()
        {
            var dataContext = new TestDataContext();
            var identity = Substitute.For<IIdentity>();
            var context = Substitute.For<IContext>();
            context.Identity.Returns(identity);
            var initContext = Substitute.For<IDataInitializationContext>();
            initContext.Identity.Returns((IIdentity)null);
            initContext.InitializationContext.Returns(context);
            dataContext.Initialize(initContext);

            Assert.AreSame(identity, dataContext.Identity);
        }
    }
}