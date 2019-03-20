// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Activation;
    using Kephas.Data.Caching;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base implementation of a data command.
    /// </summary>
    /// <typeparam name="TOperationContext">Type of the operationContext.</typeparam>
    /// <typeparam name="TResult">Type of the result.</typeparam>
    public abstract class DataCommandBase<TOperationContext, TResult> : IDataCommand<TOperationContext, TResult>
        where TOperationContext : IDataOperationContext
        where TResult : IDataCommandResult
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public abstract Task<TResult> ExecuteAsync(TOperationContext operationContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        async Task<IDataCommandResult> IDataCommand.ExecuteAsync(IDataOperationContext operationContext, CancellationToken cancellationToken)
        {
            var result = await this.ExecuteAsync((TOperationContext)operationContext, cancellationToken).PreserveThreadContext();
            return result;
        }

        /// <summary>
        /// Gets the equality expression for of: t =&gt; t.Id == entityEntry.Id.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="entityId">The entity ID.</param>
        /// <returns>
        /// The equality expression.
        /// </returns>
        protected internal virtual Expression<Func<T, bool>> GetIdEqualityExpression<T>(IDataContext dataContext, object entityId)
        {
            return (dataContext as DataContextBase)?.GetIdEqualityExpression<T>(entityId)
                ?? (t => entityId.Equals(((IIdentifiable)t).Id));
        }

        /// <summary>
        /// Tries to get the data context's local cache.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <returns>
        /// An IDataContextCache.
        /// </returns>
        protected virtual IDataContextCache TryGetLocalCache(IDataContext dataContext)
        {
            return (dataContext as DataContextBase)?.LocalCache;
        }

        /// <summary>
        /// Tries to get the data context's entity activator.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <returns>
        /// An IActivator.
        /// </returns>
        protected virtual IActivator TryGetEntityActivator(IDataContext dataContext)
        {
            return (dataContext as DataContextBase)?.EntityActivator;
        }
    }
}