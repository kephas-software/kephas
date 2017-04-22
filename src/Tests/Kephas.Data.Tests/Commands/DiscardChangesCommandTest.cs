// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscardChangesCommandTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DiscardChangesCommandTest
    {
        [Test]
        public void Execute_success()
        {
            var localCache = new DataContextCache();
            var cmd = new TestDiscardChangesCommand(localCache);

            var entityInfo = new EntityInfo("added") { ChangeState = ChangeState.Added };
            localCache.Add(entityInfo.Id, entityInfo);
            entityInfo = new EntityInfo("addedChanged") { ChangeState = ChangeState.AddedOrChanged };
            localCache.Add(entityInfo.Id, entityInfo);
            entityInfo = new EntityInfo("changed") { ChangeState = ChangeState.Changed };
            localCache.Add(entityInfo.Id, entityInfo);
            entityInfo = new EntityInfo("deleted") { ChangeState = ChangeState.Deleted };
            localCache.Add(entityInfo.Id, entityInfo);
            entityInfo = new EntityInfo("notchanged") { ChangeState = ChangeState.NotChanged };
            localCache.Add(entityInfo.Id, entityInfo);

            var result = cmd.Execute(new DataOperationContext(Substitute.For<IDataContext>()));
            Assert.AreSame(DataCommandResult.Success, result);
            Assert.AreEqual(1, localCache.Count);
            Assert.AreEqual(ChangeState.NotChanged, localCache.First().Value.ChangeState);
            Assert.AreEqual("notchanged", localCache.First().Value.Entity);
        }
    }

    public class TestDiscardChangesCommand : DiscardChangesCommand
    {
        private readonly IDataContextCache localCache;

        public TestDiscardChangesCommand(IDataContextCache localCache)
        {
            this.localCache = localCache;
        }

        /// <summary>
        /// Tries to get the data context's local cache.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <returns>
        /// An IDataContextCache.
        /// </returns>
        protected override IDataContextCache TryGetLocalCache(IDataContext dataContext)
        {
            return this.localCache;
        }
    }
}