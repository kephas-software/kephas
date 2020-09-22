// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkUpdateCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bulk update command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for bulk update commands.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class BulkUpdateCommand : BulkOperationCommandBase<IBulkUpdateContext, IBulkDataOperationResult>,
                                     IBulkUpdateCommand
    {
        /// <summary>
        /// Gets the generic method of <see cref="BulkUpdateAsync{T}"/>.
        /// </summary>
        private static readonly MethodInfo BulkUpdateAsyncMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((BulkUpdateCommand)null!).BulkUpdateAsync<string>(default, default));

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkUpdateCommand"/> class.
        /// </summary>
        /// <param name="logManager">Optional. Manager for log.</param>
        public BulkUpdateCommand(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override async Task<IBulkDataOperationResult> ExecuteAsync(IBulkUpdateContext operationContext, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(operationContext, nameof(operationContext));
            Requires.NotNull(operationContext.DataContext, nameof(operationContext.DataContext));
            Requires.NotNull(operationContext.EntityType, nameof(operationContext.EntityType));
            Requires.NotNull(operationContext.Criteria, nameof(operationContext.Criteria));
            Requires.NotNull(operationContext.Values, nameof(operationContext.Values));

            var opAsync = BulkUpdateAsyncMethod.MakeGenericMethod(operationContext.EntityType);
            var asyncResult = (Task<IBulkDataOperationResult>)opAsync.Call(this, operationContext, cancellationToken);
            var result = await asyncResult.PreserveThreadContext();
            return result;
        }

        /// <summary>
        /// Updates the entities matching the provided criteria and returns the number of affected entities.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="bulkUpdateContext">The bulk update context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the number of updated entities.
        /// </returns>
        protected virtual async Task<IBulkDataOperationResult> BulkUpdateAsync<T>(
            IBulkUpdateContext bulkUpdateContext,
            CancellationToken cancellationToken)
            where T : class
        {
            var dataContext = bulkUpdateContext.DataContext;
            var criteria = this.GetMatchingCriteria<T>(bulkUpdateContext);
            var values = bulkUpdateContext.Values.ToExpando().ToDictionary();

            var localCache = this.TryGetLocalCache(dataContext);
            var localCacheCount = 0L;
            if (localCache != null)
            {
                var compiledCriteria = criteria.Compile();
                var toUpdateFromCache = localCache.Values.Where(v => v.Entity is T entity && compiledCriteria(entity)).ToList();
                toUpdateFromCache.ForEach(e => this.UpdateEntity(e.Entity, values));
                localCacheCount = toUpdateFromCache.Count;
            }

            var count = await this.BulkUpdateCoreAsync(bulkUpdateContext, criteria, values, cancellationToken).PreserveThreadContext();
            return this.GetBulkOperationResult(bulkUpdateContext, count, localCacheCount, criteria);
        }

        /// <summary>
        /// Updates the entities matching the provided criteria and returns the number of affected
        /// entities.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="bulkDeleteContext">The bulk delete context.</param>
        /// <param name="criteria">The criteria for finding the entities to operate on.</param>
        /// <param name="values">The values.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the number of updated entities.
        /// </returns>
        protected virtual Task<long> BulkUpdateCoreAsync<T>(
            IBulkUpdateContext bulkDeleteContext,
            Expression<Func<T, bool>> criteria,
            IDictionary<string, object> values,
            CancellationToken cancellationToken)
            where T : class
        {
            throw new NotSupportedException(
                $"The {nameof(BulkDeleteCommand)}.{nameof(this.BulkUpdateCoreAsync)}(context, criteria, values, cancellationToken) must be overridden to provide a meaningful execution.");
        }

        /// <summary>
        /// Updates the entity with the values in the dictionary.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="values">The values.</param>
        protected virtual void UpdateEntity<T>(T entity, IDictionary<string, object> values)
        {
            var expandoEntity = entity.ToExpando();
            foreach (var kv in values)
            {
                expandoEntity[kv.Key] = kv.Value;
            }
        }
    }
}