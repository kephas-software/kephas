// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkDeleteCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bulk delete command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for bulk delete commands.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class BulkDeleteCommand : BulkOperationCommandBase<IBulkDeleteContext, IBulkDataOperationResult>, IBulkDeleteCommand
    {
        /// <summary>
        /// Gets the generic method of <see cref="BulkDeleteCoreAsync{T}"/>.
        /// </summary>
        private static readonly MethodInfo BulkDeleteAsyncMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((BulkDeleteCommand)null).BulkDeleteAsync<string>(default, default));

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IBulkDataOperationResult"/>.
        /// </returns>
        public override async Task<IBulkDataOperationResult> ExecuteAsync(IBulkDeleteContext operationContext, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(operationContext, nameof(operationContext));
            Requires.NotNull(operationContext.DataContext, nameof(operationContext.DataContext));
            Requires.NotNull(operationContext.EntityType, nameof(operationContext.EntityType));
            Requires.NotNull(operationContext.Criteria, nameof(operationContext.Criteria));


            var opAsync = BulkDeleteAsyncMethod.MakeGenericMethod(operationContext.EntityType);
            var asyncResult = (Task<IBulkDataOperationResult>)opAsync.Call(this, operationContext, cancellationToken);
            var result = await asyncResult.PreserveThreadContext();
            return result;
        }

        /// <summary>
        /// Deletes the entities matching the provided criteria and returns the number of affected entities.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="bulkDeleteContext">The bulk delete context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the number of deleted entities.
        /// </returns>
        protected virtual async Task<IBulkDataOperationResult> BulkDeleteAsync<T>(
            IBulkDeleteContext bulkDeleteContext,
            CancellationToken cancellationToken)
            where T : class
        {
            var dataContext = bulkDeleteContext.DataContext;
            var criteria = this.GetMatchingCriteria<T>(bulkDeleteContext);

            var localCache = this.TryGetLocalCache(dataContext);
            var localCacheCount = 0L;
            if (localCache != null)
            {
                var compiledCriteria = criteria.Compile();
                var toDeleteFromCache = localCache.Values.Where(v => v.Entity is T entity && compiledCriteria(entity)).ToList();
                toDeleteFromCache.ForEach(p => localCache.Remove(p));
                localCacheCount = toDeleteFromCache.Count;
            }

            var count = await this.BulkDeleteCoreAsync(bulkDeleteContext, criteria, cancellationToken).PreserveThreadContext();
            return this.GetBulkOperationResult(bulkDeleteContext, count, localCacheCount, criteria);
        }

        /// <summary>
        /// Deletes the entities matching the provided criteria and returns the number of affected entities.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="bulkDeleteContext">The bulk delete context.</param>
        /// <param name="criteria">The criteria for finding the entities to operate on.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the number of deleted entities.
        /// </returns>
        protected virtual Task<long> BulkDeleteCoreAsync<T>(
            IBulkDeleteContext bulkDeleteContext,
            Expression<Func<T, bool>> criteria,
            CancellationToken cancellationToken)
            where T : class
        {
            throw new NotSupportedException(
                "The BulkDeleteCommand.BulkDeleteCoreAsync(criteria, cancellationToken) must be overridden to provide a meaningful execution.");
        }
    }
}