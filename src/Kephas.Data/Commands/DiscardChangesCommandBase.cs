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
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base implementation of a <see cref="IDiscardChangesCommand{TDataContext}"/>.
    /// </summary>
    /// <typeparam name="TDataContext">Type of the data context.</typeparam>
    public abstract class DiscardChangesCommandBase<TDataContext> : DataCommandBase<IDataOperationContext, IDataCommandResult>, IDiscardChangesCommand<TDataContext>
        where TDataContext : IDataContext
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override Task<IDataCommandResult> ExecuteAsync(IDataOperationContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.Execute(operationContext);
            return Task.FromResult<IDataCommandResult>(DataCommandResult.Success);
        }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        public abstract void Execute(IDataOperationContext operationContext);
    }
}