// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscardChangesCommandTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the discard changes command test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Tests.Commands
{
    using System.Linq;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;

    using NUnit.Framework;

    [TestFixture]
    public class DiscardChangesCommandTest
    {
        [Test]
        public void Execute_success()
        {
            var localCache = new DataContextCache();
            var dataContext = new TestDataContext(localCache: localCache);
            var cmd = new DiscardChangesCommand();

            var entityEntry = new EntityEntry("added") { ChangeState = ChangeState.Added };
            localCache.Add(entityEntry);
            entityEntry = new EntityEntry("addedOrChanged") { ChangeState = ChangeState.AddedOrChanged };
            localCache.Add(entityEntry);
            entityEntry = new EntityEntry("changed") { ChangeState = ChangeState.Changed };
            localCache.Add(entityEntry);
            entityEntry = new EntityEntry("deleted") { ChangeState = ChangeState.Deleted };
            localCache.Add(entityEntry);
            entityEntry = new EntityEntry("notchanged") { ChangeState = ChangeState.NotChanged };
            localCache.Add(entityEntry);

            var result = cmd.Execute(new DiscardChangesContext(dataContext));
            Assert.AreSame(DataCommandResult.Success, result);
            Assert.AreEqual(3, localCache.Count);
            Assert.IsTrue(localCache.All(e => e.Value.ChangeState == ChangeState.NotChanged));
            Assert.IsTrue(localCache.Any(e => (string)e.Value.Entity == "changed"));
            Assert.IsTrue(localCache.Any(e => (string)e.Value.Entity == "deleted"));
            Assert.IsTrue(localCache.Any(e => (string)e.Value.Entity == "notchanged"));
        }
    }
}