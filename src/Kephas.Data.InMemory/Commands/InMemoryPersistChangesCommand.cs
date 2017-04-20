// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryPersistChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the in memory persist changes command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;

    /// <summary>
    /// Persist changes command for <see cref="InMemoryDataContext"/>.
    /// </summary>
    [DataContextType(typeof(InMemoryDataContext))]
    public class InMemoryPersistChangesCommand : PersistChangesCommandBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryPersistChangesCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        public InMemoryPersistChangesCommand(IDataBehaviorProvider behaviorProvider)
            : base(behaviorProvider)
        {
        }

        /// <summary>
        /// Persists the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task PersistModifiedEntriesAsync(
            IList<IPersistChangesEntry> modifiedEntries,
            IPersistChangesContext operationContext,
            CancellationToken cancellationToken)
        {
            var dataContext = operationContext.DataContext as InMemoryDataContext;
            if (dataContext == null)
            {
                return Task.FromResult<IList<IPersistChangesEntry>>(null);
            }

            // remove all deleted from memory.
            var deleted = dataContext.WorkingCache.Values.Where(e => e.ChangeState == ChangeState.Deleted).ToList();
            deleted.ForEach(e => dataContext.WorkingCache.Remove(e.Id));

            return Task.FromResult(0);
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
            var inMemoryDataContext = (InMemoryDataContext)dataContext;
            return inMemoryDataContext.WorkingCache;
        }
    }
}