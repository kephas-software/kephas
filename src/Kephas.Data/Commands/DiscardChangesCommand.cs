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
    using System.Linq;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// Base implementation of a <see cref="IDiscardChangesCommand"/>.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class DiscardChangesCommand : SyncDataCommandBase<IDataOperationContext, IDataCommandResult>, IDiscardChangesCommand
    {
        /// <summary>
        /// Removes all the changed entity infos from the cache.
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

            var changes = localCache.Values.Where(e => e.ChangeState != ChangeState.NotChanged).ToList();
            foreach (var change in changes)
            {
                localCache.Remove(change.Id);
            }

            return DataCommandResult.Success;
        }
    }
}