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
            var entities = operationContext.Entities;
            foreach (var entity in entities)
            {
                var entityEntry = dataContext.GetEntityEntry(entity);
                if (entityEntry == null)
                {
                    throw new InvalidOperationException(Strings.DataContextBase_EntityNotAttached_Exception);
                }

                if (entityEntry.ChangeState == ChangeState.Added)
                {
                    dataContext.Detach(entityEntry);
                }

                entityEntry.ChangeState = ChangeState.Deleted;
            }

            return DataCommandResult.Success;
        }
    }
}