// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoBulkDeleteCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo purge command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Commands
{
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data.Commands;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A MongoDB bulk delete command.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoBulkDeleteCommand : BulkDeleteCommand
    {
        /// <summary>
        /// Deletes the entities matching the provided criteria and returns the number of affected
        /// entities.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="bulkDeleteContext">The bulk delete context.</param>
        /// <param name="criteria">The criteria for finding the entities to operate on.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of the number of deleted entities.
        /// </returns>
        protected override async Task<long> BulkDeleteCoreAsync<T>(
            IBulkDeleteContext bulkDeleteContext,
            Expression<Func<T, bool>> criteria,
            CancellationToken cancellationToken)
        {
            // TODO make sure the T is the entity type, not an abstraction
            // then convert the criteria from abstraction to concrete.
            var dataContext = (MongoDataContext)bulkDeleteContext.DataContext;
            var collectionName = dataContext.GetCollectionName(typeof(T));
            var collection = dataContext.Database.GetCollection<T>(collectionName);
            var result = await collection.DeleteManyAsync(criteria, cancellationToken).PreserveThreadContext();
            return result.DeletedCount;
        }
    }
}