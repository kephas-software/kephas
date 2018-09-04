﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DeleteEntityCommandTest
    {
        [Test]
        public void Execute_delete_not_attached_entity()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<ICompositionContext>(), Substitute.For<IDataCommandProvider>(), localCache);

            var cmd = new DeleteEntityCommand();
            Assert.Throws<InvalidOperationException>(() => cmd.Execute(new DeleteEntityContext(dataContext, "123")));
        }

        [Test]
        public void Execute_delete_added_entity()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<ICompositionContext>(), Substitute.For<IDataCommandProvider>(), localCache);

            var entityInfo = new EntityInfo("123") { ChangeState = ChangeState.Added };
            localCache.Add(entityInfo);

            var cmd = new DeleteEntityCommand();
            var result = cmd.Execute(new DeleteEntityContext(dataContext, "123"));

            Assert.AreSame(DataCommandResult.Success, result);
            Assert.AreEqual(ChangeState.Deleted, entityInfo.ChangeState);
            Assert.AreEqual(0, localCache.Count);
        }

        [Test]
        public void Execute_delete_not_changed_entity()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(Substitute.For<ICompositionContext>(), Substitute.For<IDataCommandProvider>(), localCache);

            var entityInfo = new EntityInfo("123") { ChangeState = ChangeState.NotChanged };
            localCache.Add(entityInfo);

            var cmd = new DeleteEntityCommand();
            var result = cmd.Execute(new DeleteEntityContext(dataContext, "123"));

            Assert.AreSame(DataCommandResult.Success, result);
            Assert.AreEqual(ChangeState.Deleted, entityInfo.ChangeState);
            Assert.AreEqual(1, localCache.Count);
        }
    }
}