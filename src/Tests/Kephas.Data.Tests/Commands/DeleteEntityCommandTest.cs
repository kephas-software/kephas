// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteEntityCommandTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the delete entity command test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Commands
{
    using System;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;

    using NUnit.Framework;

    [TestFixture]
    public class DeleteEntityCommandTest
    {
        [Test]
        public void Execute_delete_not_attached_entity()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.CreateDataContext(localCache: localCache);

            var cmd = new DeleteEntityCommand();
            Assert.Throws<InvalidOperationException>(() => cmd.Execute(new DeleteEntityContext(dataContext, "123")));
        }

        [Test]
        public void Execute_delete_added_entity()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.CreateDataContext(localCache: localCache);

            var entityEntry = new EntityEntry("123") { ChangeState = ChangeState.Added };
            localCache.Add(entityEntry);

            var cmd = new DeleteEntityCommand();
            var result = cmd.Execute(new DeleteEntityContext(dataContext, "123"));

            Assert.AreSame(DataCommandResult.Success, result);
            Assert.AreEqual(ChangeState.Deleted, entityEntry.ChangeState);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void Execute_delete_not_changed_entity()
        {
            var localCache = new DataContextCache();
            var dataContext = TestDataContext.CreateDataContext(localCache: localCache);

            var entityEntry = new EntityEntry("123") { ChangeState = ChangeState.NotChanged };
            localCache.Add(entityEntry);

            var cmd = new DeleteEntityCommand();
            var result = cmd.Execute(new DeleteEntityContext(dataContext, "123"));

            Assert.AreSame(DataCommandResult.Success, result);
            Assert.AreEqual(ChangeState.Deleted, entityEntry.ChangeState);
            Assert.AreEqual(1, localCache.Count);
        }
    }
}