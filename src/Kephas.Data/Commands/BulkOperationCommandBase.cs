// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BulkOperationCommandBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the bulk operation command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq.Expressions;

    using Kephas.Data.Resources;
    using Kephas.Logging;

    /// <summary>
    /// Base abstract class for bulk operation commands.
    /// </summary>
    /// <typeparam name="TOperationContext">Type of the operation context.</typeparam>
    /// <typeparam name="TOperationResult">Type of the operation result.</typeparam>
    public abstract class BulkOperationCommandBase<TOperationContext, TOperationResult> : DataCommandBase<TOperationContext, TOperationResult>
        where TOperationContext : IBulkDataOperationContext
        where TOperationResult : IBulkDataOperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkOperationCommandBase{TOperationContext,TResult}"/> class.
        /// </summary>
        /// <param name="logManager">Optional. Manager for log.</param>
        protected BulkOperationCommandBase(ILogManager? logManager = null)
            : base(logManager)
        {
        }

        /// <summary>
        /// Gets the find criteria.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="bulkOperationContext">The bulk operation context.</param>
        /// <returns>
        /// The matching criteria.
        /// </returns>
        protected virtual Expression<Func<T, bool>> GetMatchingCriteria<T>(TOperationContext bulkOperationContext)
            where T : class
        {
            return (Expression<Func<T, bool>>)bulkOperationContext.Criteria;
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
        protected virtual TOperationResult GetBulkOperationResult<T>(TOperationContext bulkOperationContext, long count, long localCacheCount, Expression<Func<T, bool>> criteria)
            where T : class
        {
            Exception exception = null;
            if (count == 0 && bulkOperationContext.ThrowOnNotFound)
            {
                exception = new NotFoundDataException(string.Format(Strings.DataContext_FindAsync_NotFound_Exception, this.GetCriteriaString(bulkOperationContext, criteria)));
                if (bulkOperationContext.ThrowOnNotFound)
                {
                    throw exception;
                }
            }

            return this.CreateBulkOperationResult<T>(bulkOperationContext, count, exception);
        }

        /// <summary>
        /// Creates a bulk operation result.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="bulkOperationContext">The bulk operation context.</param>
        /// <param name="count">The number of affected entities.</param>
        /// <param name="exception">The exception.</param>
        /// <returns>
        /// The new bulk operation result.
        /// </returns>
        protected virtual TOperationResult CreateBulkOperationResult<T>(TOperationContext bulkOperationContext, long count, Exception exception)
        {
            return (TOperationResult)(object)new BulkDataOperationResult(count, exception: exception);
        }

        /// <summary>
        /// Gets the criteria string for exception display.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="bulkOperationContext">The bulk operation context.</param>
        /// <param name="criteria">The criteria for matching entities.</param>
        /// <returns>
        /// The criteria string.
        /// </returns>
        protected virtual string GetCriteriaString<T>(
            IBulkDataOperationContext bulkOperationContext,
            Expression<Func<T, bool>> criteria)
        {
            return criteria.ToString();
        }
    }
}