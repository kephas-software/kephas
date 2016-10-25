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
    public abstract class FindCommandBase<T> : DataCommandBase<IFindContext<T>, IFindResult<T>>, IFindCommand<T>
    {
        /// <summary>
        /// Executes the data command asynchronously.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">(Optional) the cancellation token.</param>
        /// <returns>
        /// A promise of a <see cref="IDataCommandResult"/>.
        /// </returns>
        public override async Task<IFindResult<T>> ExecuteAsync(IFindContext<T> context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entities = await context.Repository.Query<T>()
                                .Cast<IIdentifiable>()
                                .Where(e => e.Id == context.Id)
                                .Take(2)
                                .ToListAsync(cancellationToken: cancellationToken)
                                .PreserveThreadContext();
            if (entities.Count > 1)
            {
                throw new AmbiguousMatchDataException(string.Format(Strings.DataRepository_FindOneAsync_AmbiguousMatch_Exception, $"Id == {context.Id}"));
            }

            Exception exception = null;
            if (entities.Count == 0)
            {
                exception = new NotFoundDataException(string.Format(Strings.DataRepository_FindAsync_NotFound_Exception, context.Id));
                if (context.ThrowIfNotFound)
                {
                    throw new NotFoundDataException(string.Format(Strings.DataRepository_FindAsync_NotFound_Exception, context.Id));
                }
            }

            var result = new FindResult<T>(entities.Count == 0 ? default(T) : (T)entities[0], exception: exception);
            return result;
        }
    }
}