// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDiscardChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the in memory discard changes command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Commands
{
    using System.Linq;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;

    /// <summary>
    /// Discard changes command for <see cref="InMemoryDataContext"/>.
    /// </summary>
    [DataContextType(typeof(InMemoryDataContext))]
    public class InMemoryDiscardChangesCommand : DiscardChangesCommandBase
    {
        /// <summary>
        /// Discards the changes in the data context.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IDataCommandResult"/>.
        /// </returns>
        public override IDataCommandResult Execute(IDataOperationContext operationContext)
        {
            var localCache = this.TryGetLocalCache(operationContext.DataContext);
            if (localCache == null)
            {
                return DataCommandResult.Success;
            }

            // remove added entities
            var additions = localCache.Values.Where(e => e.ChangeState == ChangeState.Added).ToList();
            foreach (var addition in additions)
            {
                localCache.Remove(addition.Id);
            }

            // reset the change state to NotChanged
            // TODO should undo the value changes, too
            var changes = localCache.Values.Where(e => e.ChangeState == ChangeState.Changed || e.ChangeState == ChangeState.Deleted).ToList();
            foreach (var change in changes)
            {
                change.ChangeState = ChangeState.NotChanged;
            }

            return DataCommandResult.Success;
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