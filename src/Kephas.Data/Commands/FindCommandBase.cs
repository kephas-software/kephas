// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FindCommandBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the find command base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Linq;
    using Kephas.Data.Resources;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for find commands.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    public abstract class FindCommandBase<T> : DataCommandBase<IFindContext, IFindResult<T>>, IFindCommand<T>
        where T : class
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override async Task<IFindResult<T>> ExecuteAsync(IFindContext operationContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dataContext = operationContext.DataContext;
            var entities = await this.GetEntityQuery(operationContext)
                                .Take(2)
                                .ToListAsync(cancellationToken: cancellationToken)
                                .PreserveThreadContext();
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
                    throw new NotFoundDataException(string.Format(Strings.DataContext_FindAsync_NotFound_Exception, operationContext.Id));
                }
            }

            var result = new FindResult<T>(entities.Count == 0 ? default(T) : (T)entities[0], exception: exception);
            return result;
        }

        /// <summary>
        /// Gets the entity query for filtering out the required entity.
        /// </summary>
        /// <param name="operationContext">The operation context.</param>
        /// <returns>
        /// The entity query.
        /// </returns>
        protected virtual IQueryable<T> GetEntityQuery(IFindContext operationContext)
        {
            var dataContext = operationContext.DataContext;
            var query = dataContext.Query<T>().Where(e => ((IIdentifiable)e).Id == operationContext.Id);
            return query;
        }
    }
}