// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Linq;
    using Kephas.Data.Resources;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for find commands.
    /// </summary>
    [DataContextType(typeof(DataContextBase))]
    public class FindCommand : DataCommandBase<IFindContext, IFindResult>, IFindCommand
    {
        /// <summary>
        /// The query method.
        /// </summary>
        private static readonly MethodInfo GetMatchingEntitiesMethod = ReflectionHelper.GetGenericMethodOf(_ => ((FindCommand)null).GetMatchingEntities<string>(null, CancellationToken.None));

        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override async Task<IFindResult> ExecuteAsync(IFindContext operationContext, CancellationToken cancellationToken = default)
        {
            var getMatchingEntities = GetMatchingEntitiesMethod.MakeGenericMethod(operationContext.EntityType);
            var entitiesPromise = (Task<IEnumerable<object>>)getMatchingEntities.Call(this, operationContext, cancellationToken);
            var entities = (await entitiesPromise.PreserveThreadContext()).ToList();
            if (entities.Count > 1)
            {
                throw new AmbiguousMatchDataException(string.Format(Strings.DataContext_FindOneAsync_AmbiguousMatch_Exception, $"Id == {operationContext.Id}"));
            }

            Exception exception = null;
            if (entities.Count == 0)
            {
                exception = new NotFoundDataException(string.Format(Strings.DataContext_FindAsync_NotFound_Exception, operationContext.Id));
                if (operationContext.ThrowIfNotFound)
                {
                    throw exception;
                }
            }

            var result = new FindResult(entities.Count == 0 ? null : entities[0], exception: exception);
            return result;
        }

        /// <summary>
        /// Gets the matching entities.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// The matching entities.
        /// </returns>
        public virtual async Task<IEnumerable<object>> GetMatchingEntities<T>(IFindContext operationContext, CancellationToken cancellationToken)
            where T : class
        {
            var entities = await this.GetEntityQuery<T>(operationContext)
                                    .ToListAsync(cancellationToken: cancellationToken)
                                    .PreserveThreadContext();
            return entities;
        }

        /// <summary>
        /// Gets the entity query for filtering out the required entity.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// The entity query.
        /// </returns>
        protected virtual IQueryable<T> GetEntityQuery<T>(IFindContext operationContext)
            where T : class
        {
            var dataContext = operationContext.DataContext;
            var query = dataContext
                            .Query<T>(new QueryOperationContext(dataContext))
                            .Where(this.GetIdEqualityExpression<T>(dataContext, operationContext.Id))
                            .Take(2);
            return query;
        }
    }
}