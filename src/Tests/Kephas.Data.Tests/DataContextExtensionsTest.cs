// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextExtensionsTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests
{
    using System;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataContextExtensionsTest
    {
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

        [Test]
        public void DetachEntity_not_attached()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);

            var entity = "hello";

            var detachedEntityInfo = dataContext.DetachEntity(entity);
            Assert.IsNull(detachedEntityInfo);
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

            var detachedEntityInfo = dataContext.DetachEntity(entity);
            Assert.AreSame(entityInfo, detachedEntityInfo);
            Assert.AreEqual(0, localCache.Count);
        }
    }
}