// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteEntityCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the delete entity command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Resources;

    /// <summary>
    /// Base implementation of a <see cref="IDeleteEntityCommand"/>.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class DeleteEntityCommand : SyncDataCommandBase<IDeleteEntityContext, IDataCommandResult>, IDeleteEntityCommand
    {
        /// <summary>
        /// Discards the changes in the data context.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IDataCommandResult"/>.
        /// </returns>
        public override IDataCommandResult Execute(IDeleteEntityContext operationContext)
        {
            var dataContext = operationContext.DataContext;
            var entity = operationContext.Entity;
            var changeStateTrackable = dataContext.TryGetCapability<IChangeStateTrackable>(entity);
            if (changeStateTrackable == null)
            {
                throw new InvalidOperationException(Strings.DeleteEntityCommandBase_ChangeStateNotSupported_Exception);
            }

            if (changeStateTrackable.ChangeState == ChangeState.Added)
            {
                var entityInfo = dataContext.GetEntityInfo(entity);
                var localCache = this.TryGetLocalCache(dataContext);
                localCache?.Remove(entityInfo.Id);
            }
            else
            {
                changeStateTrackable.ChangeState = ChangeState.Deleted;
            }

            return DataCommandResult.Success;
        }
    }
}