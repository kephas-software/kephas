// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeleteEntityCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
            var entityInfo = dataContext.GetEntityInfo(entity);
            if (entityInfo == null)
            {
                throw new InvalidOperationException(Strings.DataContextBase_EntityNotAttached_Exception);
            }

            if (entityInfo.ChangeState == ChangeState.Added)
            {
                dataContext.DetachEntity(entityInfo);
            }

            entityInfo.ChangeState = ChangeState.Deleted;

            return DataCommandResult.Success;
        }
    }
}