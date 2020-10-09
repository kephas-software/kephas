// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryBulkDeleteCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the in memory bulk delete command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.InMemory.Commands
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Commands;
    using Kephas.Logging;

    /// <summary>
    /// An in memory bulk delete command.
    /// </summary>
    [DataContextType(typeof(InMemoryDataContext))]
    public class InMemoryBulkDeleteCommand : BulkDeleteCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryBulkDeleteCommand"/> class.
        /// </summary>
        /// <param name="logManager">Optional. Manager for log.</param>
        public InMemoryBulkDeleteCommand(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Deletes the entities matching the provided criteria and returns the number of deleted entities.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="bulkDeleteContext">The bulk delete context.</param>
        /// <param name="criteria">The criteria for finding the entities to operate on.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// A promise of the number of deleted entities.
        /// </returns>
        protected override Task<long> BulkDeleteCoreAsync<T>(
            IBulkDeleteContext bulkDeleteContext,
            Expression<Func<T, bool>> criteria,
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
            IBulkDeleteContext bulkOperationContext,
            long count,
            long localCacheCount,
            Expression<Func<T, bool>> criteria)
        {
            return base.GetBulkOperationResult(bulkOperationContext, localCacheCount, localCacheCount, criteria);
        }
    }
}