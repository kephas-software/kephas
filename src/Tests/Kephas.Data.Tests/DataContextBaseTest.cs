// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBaseTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System;
    using System.Linq;

    using Kephas.Data.Caching;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;

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
        public void DetachEntity_object()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = "hello";
            var entityInfo = dataContext.AttachEntity(entity);

            var detachedEntityInfo = dataContext.DetachEntity(entity);
            Assert.AreSame(entityInfo, detachedEntityInfo);
            Assert.AreEqual(0, localCache.Count);
        }
    }
}