// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoBulkUpdateCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo bulk update command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using global::MongoDB.Driver;

    using Kephas.Data.Commands;
    using Kephas.Logging;
    using Kephas.MongoDB;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// A MongoDB bulk update command.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoBulkUpdateCommand : BulkUpdateCommand
    {
        private readonly IMongoNamingStrategy namingStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoBulkUpdateCommand"/> class.
        /// </summary>
        /// <param name="namingStrategy">The naming strategy.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public MongoBulkUpdateCommand(IMongoNamingStrategy namingStrategy, ILogManager? logManager = null)
            : base(logManager)
        {
            this.namingStrategy = namingStrategy;
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
        protected override async Task<long> BulkUpdateCoreAsync<T>(
            IBulkUpdateContext bulkDeleteContext,
            Expression<Func<T, bool>> criteria,
            IDictionary<string, object> values,
            CancellationToken cancellationToken)
        {
            // TODO make sure the T is the entity type, not an abstraction
            // then convert the criteria from abstraction to concrete.
            var dataContext = (MongoDataContext)bulkDeleteContext.DataContext;
            var collectionName = this.namingStrategy.GetCollectionName(typeof(T));
            var collection = dataContext.Database.GetCollection<T>(collectionName);
            var result = await collection.UpdateManyAsync(criteria, this.GetUpdateDefinition<T>(values), cancellationToken: cancellationToken).PreserveThreadContext();
            return result.ModifiedCount;
        }

        /// <summary>
        /// Gets update definition.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="values">The values.</param>
        /// <returns>
        /// The update definition.
        /// </returns>
        private UpdateDefinition<T> GetUpdateDefinition<T>(IDictionary<string, object> values)
            where T : class
        {
            var builder = new UpdateDefinitionBuilder<T>();
            UpdateDefinition<T> updateDef = null;
            foreach (var kv in values)
            {
                updateDef = updateDef == null ? builder.Set(kv.Key, kv.Value) : updateDef.Set(kv.Key, kv.Value);
            }

            if (updateDef == null)
            {
                throw new InvalidOperationException($"Cannot update {typeof(T).Name} as long as there is no value to update.");
            }

            return updateDef;
        }
    }
}