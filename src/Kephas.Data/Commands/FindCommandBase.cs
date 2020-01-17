// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindCommandBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the find command base class.
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

    using Kephas.Data.Linq;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for find commands.
    /// </summary>
    /// <typeparam name="TFindContext">Type of the find context.</typeparam>
    public abstract class FindCommandBase<TFindContext> : DataCommandBase<TFindContext, IFindResult>
        where TFindContext : class, IFindContextBase
    {
        /// <summary>
        /// Gets the generic method of <see cref="FindAsync{T}"/>.
        /// </summary>
        private static readonly MethodInfo FindAsyncMethod =
            ReflectionHelper.GetGenericMethodOf(_ => ((FindCommandBase<TFindContext>)null).FindAsync<string>(default, CancellationToken.None));

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandBase{TFindContext}"/> class.
        /// </summary>
        /// <param name="logManager">Manager for log.</param>
        protected FindCommandBase(ILogManager logManager)
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
        public override async Task<IFindResult> ExecuteAsync(TFindContext operationContext, CancellationToken cancellationToken = default)
        {
            Requires.NotNull(operationContext, nameof(operationContext));
            Requires.NotNull(operationContext.DataContext, nameof(operationContext.DataContext));
            Requires.NotNull(operationContext.EntityType, nameof(operationContext.EntityType));

            var findAsync = FindAsyncMethod.MakeGenericMethod(operationContext.EntityType);
            var asyncResult = (Task<IFindResult>)findAsync.Call(this, operationContext, cancellationToken);
            var result = await asyncResult.PreserveThreadContext();
            return result;
        }

        /// <summary>
        /// Gets the find criteria.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="findContext">The find context.</param>
        /// <returns>
        /// The find criteria.
        /// </returns>
        protected abstract Expression<Func<T, bool>> GetFindCriteria<T>(TFindContext findContext)
            where T : class;

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="findContext">The find context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        protected virtual async Task<IFindResult> FindAsync<T>(
            TFindContext findContext,
            CancellationToken cancellationToken)
            where T : class
        {
            var dataContext = findContext.DataContext;
            var criteria = this.GetFindCriteria<T>(findContext);
            IList<T> result;

            var localCacheQuery = this.TryGetLocalCacheQuery<T>(findContext);
            if (localCacheQuery != null)
            {
                result = localCacheQuery
                    .Where(criteria.Compile())
                    .Take(2)
                    .ToList();
                if (result.Count > 0)
                {
                    return this.GetFindResult(findContext, result, criteria);
                }
            }

            var query = dataContext.Query<T>().Where(criteria).Take(2);
            result = await query.ToListAsync(cancellationToken).PreserveThreadContext();
            return this.GetFindResult(findContext, result, criteria);
        }

        /// <summary>
        /// Tries to get a query over the local cache entities of a particular type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="findContext">The find context.</param>
        /// <returns>
        /// A query of local cache entities.
        /// </returns>
        protected virtual IEnumerable<T> TryGetLocalCacheQuery<T>(TFindContext findContext)
            where T : class
        {
            var dataContext = findContext.DataContext;
            var localCache = this.TryGetLocalCache(dataContext);
            return localCache?.Values.Select(ei => ei.Entity).OfType<T>();
        }

        /// <summary>
        /// Gets the find result out of the results.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="findContext">The find context.</param>
        /// <param name="result">The result.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// The find result.
        /// </returns>
        protected virtual IFindResult GetFindResult<T>(TFindContext findContext, IList<T> result, Expression<Func<T, bool>> criteria)
            where T : class
        {
            if (result.Count > 1)
            {
                throw new AmbiguousMatchDataException(string.Format(Strings.DataContext_FindAsync_AmbiguousMatch_Exception, this.GetCriteriaString(findContext, criteria)));
            }

            Exception exception = null;
            if (result.Count == 0)
            {
                exception = new NotFoundDataException(string.Format(Strings.DataContext_FindAsync_NotFound_Exception, this.GetCriteriaString(findContext, criteria)));
                if (findContext.ThrowOnNotFound)
                {
                    throw exception;
                }
            }

            return new FindResult(result.Count == 0 ? default : result[0], exception: exception);
        }

        /// <summary>
        /// Gets the criteria string for exception display.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="findContext">The find context.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>
        /// The criteria string.
        /// </returns>
        protected virtual string GetCriteriaString<T>(
            TFindContext findContext,
            Expression<Func<T, bool>> criteria)
        {
            return criteria.ToString();
        }
    }
}