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
        /// Detects the modified entries and returns them.
        /// </summary>
        /// <param name="operationContext">The entity operationContext.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A list of modified entries tuples.
        /// </returns>
        protected override Task<IList<IPersistChangesEntry>> DetectModifiedEntriesAsync(IPersistChangesContext operationContext, CancellationToken cancellationToken)
        {
            var dataContext = operationContext.DataContext as InMemoryDataContext;
            if (dataContext == null)
            {
                return Task.FromResult<IList<IPersistChangesEntry>>(null);
            }

            var modifiedEntries =
                dataContext.WorkingCache
                    .Where(e => e.ChangeState != ChangeState.NotChanged)
                    .Select(e => (IPersistChangesEntry)new PersistChangesEntry(e.Entity, e.ChangeState, new[] { e.Entity }))
                    .ToList();
            return Task.FromResult<IList<IPersistChangesEntry>>(modifiedEntries);
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

            // TODO remove all deleted from memory.
            // dataContext.WorkingCache.RemoveAll(ei => ei.ChangeState == ChangeState.Deleted);

            return Task.FromResult(0);
        }
    }
}