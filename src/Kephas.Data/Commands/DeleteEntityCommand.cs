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
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Capabilities;
    using Kephas.Data.Resources;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base implementation of a <see cref="IDeleteEntityCommand"/>.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class DeleteEntityCommand : DataCommandBase<IDeleteEntityContext, IOperationResult>, IDeleteEntityCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteEntityCommand"/> class.
        /// </summary>
        /// <param name="logManager">Optional. Manager for log.</param>
        public DeleteEntityCommand(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="T:IDataCommandResult" />.
        /// </returns>
        public override async Task<IOperationResult> ExecuteAsync(IDeleteEntityContext operationContext, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            return this.Execute(operationContext);
        }

        /// <summary>
        /// Discards the changes in the data context.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IOperationResult"/>.
        /// </returns>
        public override IOperationResult Execute(IDeleteEntityContext operationContext)
        {
            var dataContext = operationContext.DataContext;
            var entities = operationContext.Entities;
            foreach (var entity in entities)
            {
                var entityEntry = dataContext.GetEntityEntry(entity);
                if (entityEntry == null)
                {
                    throw new InvalidOperationException(Strings.DataContextBase_EntityNotAttached_Exception.FormatWith(entity));
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