// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncDataCommandBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the synchronise data command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base implementation of a synchronous data command.
    /// </summary>
    /// <typeparam name="TOperationContext">Type of the operationContext.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public abstract class SyncDataCommandBase<TOperationContext, TResult>
        : DataCommandBase<TOperationContext, TResult>, ISyncDataCommand<TOperationContext, TResult>
        where TOperationContext : IDataOperationContext
        where TResult : IDataCommandResult
    {
        /// <summary>
        /// Executes the data command.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IDataCommandResult"/>.
        /// </returns>
        IDataCommandResult ISyncDataCommand.Execute(IDataOperationContext operationContext)
        {
            var result = this.Execute((TOperationContext)operationContext);
            return result;
        }

        /// <summary>
        /// Executes the data command.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// A <see cref="IDataCommandResult"/>.
        /// </returns>
        public abstract TResult Execute(TOperationContext operationContext);

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override Task<TResult> ExecuteAsync(
            TOperationContext operationContext,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(this.Execute(operationContext));
        }
    }
}