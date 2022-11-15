// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoPersistChangesCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo persist changes command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using global::MongoDB.Driver;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.MongoDB.Resources;
    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Command for persisting changes targeting <see cref="MongoDataContext"/>.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoPersistChangesCommand : PersistChangesCommand
    {
        /// <summary>
        /// The bulk write asynchronous method.
        /// </summary>
        private static readonly MethodInfo BulkWriteAsyncMethod = ReflectionHelper.GetGenericMethodOf(_ => ((MongoPersistChangesCommand)null).BulkWriteAsync<IIdentifiable>(null, null, null, CancellationToken.None));

        private readonly IMongoNamingStrategy namingStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPersistChangesCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        /// <param name="namingStrategy">The naming strategy.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public MongoPersistChangesCommand(
            IDataBehaviorProvider behaviorProvider,
            IMongoNamingStrategy namingStrategy,
            ILogManager? logManager = null)
            : base(behaviorProvider, logManager)
        {
            this.namingStrategy = namingStrategy;
        }

        /// <summary>
        /// Persists the modified entities asynchronously.
        /// </summary>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override async Task PersistChangeSetAsync(IList<IEntityEntry> changeSet, IPersistChangesContext operationContext, CancellationToken cancellationToken)
        {
            var dataContext = (MongoDataContext)operationContext.DataContext;
            var modifiedMongoDocs =
              changeSet.Select(e => e.GetGraphRoot() ?? e.Entity)
                .Distinct()
                .ToList();

            if (modifiedMongoDocs.Count == 0 && changeSet.Count > 0)
            {
                throw new MongoDataException(Strings.MongoPersistChangesCommand_NoDocumentsToPersist_Exception);
            }

            var mongoDocTypes = modifiedMongoDocs.Select(e => e.GetType()).Distinct().ToList();
            foreach (var mongoDocType in mongoDocTypes)
            {
                var collectionName = this.namingStrategy.GetCollectionName(dataContext, mongoDocType);
                var writeAsync = BulkWriteAsyncMethod.MakeGenericMethod(mongoDocType);
                await ((Task)writeAsync.Call(
                     this,
                     operationContext,
                     changeSet,
                     collectionName,
                     cancellationToken))
                    .PreserveThreadContext();
            }
        }

        /// <summary>
        /// Writes the changes.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The task for async chaining.
        /// </returns>
        protected virtual async Task BulkWriteAsync<T>(
            IPersistChangesContext operationContext,
            IList<IEntityEntry> changeSet,
            string collectionName,
            CancellationToken cancellationToken)
            where T : IIdentifiable
        {
            var dataContext = (MongoDataContext)operationContext.DataContext;
            var collection = dataContext.Database.GetCollection<T>(collectionName);
            var eligibleModifiedEntries = changeSet.Where(e => e.Entity is T).ToList();

            var writeRequests = this.GetBulkWriteRequests<T>(operationContext, eligibleModifiedEntries);

            await this.NativeBulkWriteAsync(operationContext, collection, writeRequests, cancellationToken).PreserveThreadContext();
        }

        /// <summary>
        /// Performs asynchronous bulk write.
        /// </summary>
        /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="writeRequests">The write requests.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        private async Task NativeBulkWriteAsync<T>(
            IPersistChangesContext operationContext,
            IMongoCollection<T> collection,
            IList<WriteModel<T>> writeRequests,
            CancellationToken cancellationToken)
            where T : IIdentifiable
        {
            BulkWriteResult<T>? saveResult = null;
            Exception? exception = null;

            var opResult = await Profiler.WithStopwatchAsync(async () =>
            {
                try
                {
                    saveResult = await collection.BulkWriteAsync(writeRequests, cancellationToken: cancellationToken).PreserveThreadContext();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }).PreserveThreadContext();

            // log exceptions and other important data.
            if (exception != null)
            {
                this.Logger.Error(
                    exception,
                    "{operation}: {message}, data context ID: {dataContextId}, elapsed: {elapsed}.",
                    $"{nameof(MongoPersistChangesCommand)}.{nameof(this.BulkWriteAsync)}",
                    exception.Message,
                    operationContext.DataContext.Id,
                    opResult.Elapsed);
                throw exception;
            }

            if (opResult.Elapsed.TotalMilliseconds > 1000)
            {
                this.Logger.Warn(
                    "{operation}: {message}, data context ID: {dataContextId}, elapsed: {elapsed}, inserted: {inserted}, modified: {modified}, deleted: {deleted}.",
                    $"{nameof(MongoPersistChangesCommand)}.{nameof(this.BulkWriteAsync)}",
                    "Elapsed time more than 1s",
                    operationContext.DataContext.Id,
                    opResult.Elapsed,
                    saveResult.InsertedCount,
                    saveResult.ModifiedCount,
                    saveResult.DeletedCount);
            }
            else
            {
                if (this.Logger.IsDebugEnabled())
                {
                    this.Logger.Debug(
                        "{operation}: {message}, data context ID: {dataContextId}, elapsed: {elapsed}, inserted: {inserted}, modified: {modified}, deleted: {deleted}.",
                        $"{nameof(MongoPersistChangesCommand)}.{nameof(this.BulkWriteAsync)}",
                        "Success",
                        operationContext.DataContext.Id,
                        opResult.Elapsed,
                        saveResult.InsertedCount,
                        saveResult.ModifiedCount,
                        saveResult.DeletedCount);
                }
            }
        }

        /// <summary>
        /// Gets the write requests for the bulk operation.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="operationContext">The operation context.</param>
        /// <param name="eligibleChangeSet">The eligible modified entities.</param>
        /// <returns>
        /// The write requests for the bulk operation.
        /// </returns>
        private IList<WriteModel<T>> GetBulkWriteRequests<T>(IPersistChangesContext operationContext, IEnumerable<IEntityEntry> eligibleChangeSet)
        {
            var writeModel = new List<WriteModel<T>>();
            foreach (var entityEntry in eligibleChangeSet)
            {
                var changeState = entityEntry.ChangeState;
                var entity = (T)entityEntry.Entity;
                switch (changeState)
                {
                    case ChangeState.Added:
                        writeModel.Add(new InsertOneModel<T>(entity));
                        break;
                    case ChangeState.AddedOrChanged:
                        writeModel.Add(new ReplaceOneModel<T>(new ExpressionFilterDefinition<T>(operationContext.DataContext.GetIdEqualityExpression<T>(entityEntry.EntityId!)), entity) { IsUpsert = true });
                        break;
                    case ChangeState.Changed:
                        writeModel.Add(new ReplaceOneModel<T>(new ExpressionFilterDefinition<T>(operationContext.DataContext.GetIdEqualityExpression<T>(entityEntry.EntityId!)), entity));
                        break;
                    case ChangeState.Deleted:
                        writeModel.Add(new DeleteOneModel<T>(new ExpressionFilterDefinition<T>(operationContext.DataContext.GetIdEqualityExpression<T>(entityEntry.EntityId!))));
                        break;
                }
            }

            return writeModel;
        }
    }
}