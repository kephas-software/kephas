// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscardChangesCommandBase.cs" company="Quartz Software SRL">
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
    public abstract class DiscardChangesCommandBase : SyncDataCommandBase<IDataOperationContext, IDataCommandResult>, IDiscardChangesCommand
    {
        /// <summary>
        /// Executes the data command.
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