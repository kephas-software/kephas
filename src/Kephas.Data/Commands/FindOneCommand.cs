// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindOneCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the find one command class.
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

    using Kephas.Data.Linq;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for find commands retrieving one result.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class FindOneCommand : DataCommandBase<IFindOneContext, IFindResult>, IFindOneCommand
    {
        /// <summary>
        /// Gets the generic method of <see cref="FindOneAsync{T}"/>.
        /// </summary>
        private static readonly MethodInfo FindOneAsyncMethod =
            ReflectionHelper.GetGenericMethodOf(
                _ => ((FindOneCommand)null).FindOneAsync<string>(null, CancellationToken.None));

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override async Task<IFindResult> ExecuteAsync(IFindOneContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Requires.NotNull(operationContext, nameof(operationContext));
            Requires.NotNull(operationContext.DataContext, nameof(operationContext.DataContext));
            Requires.NotNull(operationContext.EntityType, nameof(operationContext.EntityType));
            Requires.NotNull(operationContext.Criteria, nameof(operationContext.Criteria));

            var findOneAsync = FindOneAsyncMethod.MakeGenericMethod(operationContext.EntityType);
            var asyncResult = (Task<IFindResult>)findOneAsync.Call(this, operationContext, cancellationToken);
            var result = await asyncResult.PreserveThreadContext();
            return result;
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        protected virtual async Task<IFindResult> FindOneAsync<T>(
            IFindOneContext operationContext,
            CancellationToken cancellationToken)
            where T : class
        {
            var dataContext = operationContext.DataContext;
            var queryContext = new QueryOperationContext(dataContext);
            var criteria = (Expression<Func<T, bool>>)operationContext.Criteria;
            var query = dataContext.Query<T>(queryContext).Where(criteria).Take(2);
            var result = await query.ToListAsync(cancellationToken).PreserveThreadContext();
            if (result.Count > 1)
            {
                throw new AmbiguousMatchDataException(string.Format(Strings.DataContext_FindOneAsync_AmbiguousMatch_Exception, criteria));
            }

            if (result.Count == 0 && operationContext.ThrowIfNotFound)
            {
                throw new NotFoundDataException(string.Format(Strings.DataContext_FindOneAsync_NotFound_Exception, criteria));
            }

            return new FindResult(result.Count == 0 ? default(T) : result[0]);
        }
    }
}