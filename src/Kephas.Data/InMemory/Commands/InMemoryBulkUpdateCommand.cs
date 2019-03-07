// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryBulkUpdateCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory bulk update command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Commands;

    /// <summary>
    /// An in memory bulk update command.
    /// </summary>
    [DataContextType(typeof(InMemoryDataContext))]
    public class InMemoryBulkUpdateCommand : BulkUpdateCommand
    {
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
        protected override Task<long> BulkUpdateCoreAsync<T>(
            IBulkUpdateContext bulkDeleteContext,
            Expression<Func<T, bool>> criteria,
            IDictionary<string, object> values,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(0L);
        }

        /// <summary>
        /// Gets the bulk operation result based on the affected entities.
        /// </summary>
        /// <exception cref="NotFoundDataException">Thrown when a Not Found Data error condition occurs.</exception>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="bulkOperationContext">The bulk operation context.</param>
        /// <param name="count">The number of affected entities.</param>
        /// <param name="localCacheCount">Number of affected entities in the local caches.</param>
        /// <param name="criteria">The criteria for matching entities.</param>
        /// <returns>
        /// The bulk operation result.
        /// </returns>
        protected override IBulkDataOperationResult GetBulkOperationResult<T>(
            IBulkUpdateContext bulkOperationContext,
            long count,
            long localCacheCount,
            Expression<Func<T, bool>> criteria)
        {
            return base.GetBulkOperationResult(bulkOperationContext, localCacheCount, localCacheCount, criteria);
        }
    }
}