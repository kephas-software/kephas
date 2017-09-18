// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscardChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the discard changes command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// Base implementation of a <see cref="IDiscardChangesCommand"/>.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class DiscardChangesCommand : SyncDataCommandBase<IDiscardChangesContext, IDataCommandResult>, IDiscardChangesCommand
    {
        /// <summary>
        /// Removes all the changed entity infos from the cache.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IDataCommandResult"/>.
        /// </returns>
        public override IDataCommandResult Execute(IDiscardChangesContext operationContext)
        {
            var dataContext = operationContext.DataContext;
            var modifiedEntries = this.DetectModifiedEntries(operationContext);

            // remove added entities
            foreach (var addition in modifiedEntries.Where(this.IsAdded))
            {
                dataContext.DetachEntity(addition);
            }

            // undo the changes for changed entitites
            foreach (var change in modifiedEntries.Where(this.IsChangedOrDeleted))
            {
                change.DiscardChanges();
            }

            return DataCommandResult.Success;
        }

        /// <summary>
        /// Detects the modified entries.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A list of entity information objects.
        /// </returns>
        protected virtual IList<IEntityInfo> DetectModifiedEntries(IDiscardChangesContext operationContext)
        {
            var localCache = this.TryGetLocalCache(operationContext.DataContext);
            if (localCache == null)
            {
                return new List<IEntityInfo>();
            }

            return localCache.Values
                .Where(e => e.ChangeState != ChangeState.NotChanged)
                .ToList();
        }

        /// <summary>
        /// Queries if the entity is added.
        /// </summary>
        /// <param name="e">The IEntityInfo to process.</param>
        /// <returns>
        /// True if added, false if not.
        /// </returns>
        private bool IsAdded(IEntityInfo e) =>
            e.ChangeState == ChangeState.Added || e.ChangeState == ChangeState.AddedOrChanged;

        /// <summary>
        /// Queries if 'e' is changed or deleted.
        /// </summary>
        /// <param name="e">The IEntityInfo to process.</param>
        /// <returns>
        /// True if changed or deleted, false if not.
        /// </returns>
        private bool IsChangedOrDeleted(IEntityInfo e) =>
            e.ChangeState == ChangeState.Changed || e.ChangeState == ChangeState.Deleted;
    }
}