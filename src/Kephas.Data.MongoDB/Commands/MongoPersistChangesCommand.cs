// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoPersistChangesCommand.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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

    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.MongoDB.Resources;
    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    using global::MongoDB.Driver;

    /// <summary>
    /// Command for persisting changes for <see cref="MongoDataContext"/>.
    /// </summary>
    [DataContextType(typeof(MongoDataContext))]
    public class MongoPersistChangesCommand : PersistChangesCommandBase
    {
        /// <summary>
        /// The bulk write asynchronous method.
        /// </summary>
        private static readonly MethodInfo BulkWriteAsyncMethod = ReflectionHelper.GetGenericMethodOf(_ => ((MongoPersistChangesCommand)null).BulkWriteAsync<IIdentifiable>(null, null, null, CancellationToken.None));

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoPersistChangesCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        public MongoPersistChangesCommand(IDataBehaviorProvider behaviorProvider)
            : base(behaviorProvider)
        {
        }

        /// <summary>
        /// Persists the modified entries asynchronously.
        /// </summary>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override async Task PersistModifiedEntriesAsync(IList<IPersistChangesEntry> modifiedEntries, IPersistChangesContext operationContext, CancellationToken cancellationToken)
        {
            var dataContext = (MongoDataContext)operationContext.DataContext;
            var modifiedMongoDocs =
              modifiedEntries.Select(e => dataContext.GetGraphRoot(e.Entity))
                .Distinct()
                .ToList();

            if (modifiedMongoDocs.Count == 0 && modifiedEntries.Count > 0)
            {
                throw new MongoDataException(Strings.MongoPersistChangesCommand_NoDocumentsToPersist_Exception);
            }

            var mongoDocTypes = modifiedMongoDocs.Select(e => e.GetType()).Distinct().ToList();
            foreach (var mongoDocType in mongoDocTypes)
            {
                var collectionName = dataContext.GetCollectionName(mongoDocType);
                    await((Task)BulkWriteAsyncMethod.Call(
                         this,
                         new object[] { operationContext, modifiedEntries, collectionName, cancellationToken }))
                        .PreserveThreadContext();
            }
        }

        /// <summary>
        /// Writes the changes.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="modifiedEntries">The modified entries.</param>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// The task for async chaining.
        /// </returns>
        protected virtual async Task BulkWriteAsync<T>(
            IPersistChangesContext operationContext,
            IList<IPersistChangesEntry> modifiedEntries,
            string collectionName,
            CancellationToken cancellationToken) where T : IIdentifiable
        {
            var dataContext = (MongoDataContext)operationContext.DataContext;
            var collection = dataContext.Database.GetCollection<T>(collectionName);
            var eligibleModifiedEntries = modifiedEntries.Where(e => e.Entity is T).ToList();

            var writeRequests = this.GetBulkWriteRequests<T>(eligibleModifiedEntries);

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
            CancellationToken cancellationToken) where T : IIdentifiable
        {
            BulkWriteResult<T> saveResult = null;
            Exception exception = null;

            var elapsed = await Profiler.WithStopwatchAsync(async () =>
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
                  $"{nameof(MongoPersistChangesCommand)}.{nameof(this.BulkWriteAsync)}|ID: {operationContext.DataContext.Id}|Message: {exception.Message}|Elapsed: {elapsed}|Change count: {0}|Data: {string.Empty}",
                  exception);
                throw exception;
            }

            if (elapsed.TotalMilliseconds > 1000)
            {
                this.Logger.Warn(
                  $"{nameof(MongoPersistChangesCommand)}.{nameof(this.BulkWriteAsync)}|ID: {operationContext.DataContext.Id}|Message: {"Elapsed time more than 1s"}|Elapsed: {elapsed}|Change count: {saveResult.InsertedCount + saveResult.ModifiedCount + saveResult.DeletedCount}|Data: {saveResult}");
            }
            else
            {
                if (this.Logger.IsDebugEnabled())
                {
                    this.Logger.Debug(
                      $"{nameof(MongoPersistChangesCommand)}.{nameof(this.BulkWriteAsync)}|ID: {operationContext.DataContext.Id}|Message: {"OK"}|Elapsed: {elapsed}|Change count: {saveResult.InsertedCount + saveResult.ModifiedCount + saveResult.DeletedCount}|Data: {saveResult}");
                }
            }
        }

        /// <summary>
        /// Gets the write requests for the bulk operation.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="eligibleModifiedEntries">The eligible modified entries.</param>
        /// <returns>
        /// The write requests for the bulk operation.
        /// </returns>
        private IList<WriteModel<T>> GetBulkWriteRequests<T>(IEnumerable<IPersistChangesEntry> eligibleModifiedEntries) where T : IIdentifiable
        {
            var writeModel = new List<WriteModel<T>>();
            foreach (var tuple in eligibleModifiedEntries)
            {
                var changeState = tuple.ChangeState;
                var entity = (T)tuple.Entity;
                switch (changeState)
                {
                    case ChangeState.Added:
                        writeModel.Add(new InsertOneModel<T>(entity));
                        break;
                    case ChangeState.AddedOrChanged:
                        writeModel.Add(new ReplaceOneModel<T>(new ExpressionFilterDefinition<T>(t => t.Id == entity.Id), entity) { IsUpsert = true });
                        break;
                    case ChangeState.Changed:
                        writeModel.Add(new ReplaceOneModel<T>(new ExpressionFilterDefinition<T>(t => t.Id == entity.Id), entity));
                        break;
                    case ChangeState.Deleted:
                        writeModel.Add(new DeleteOneModel<T>(new ExpressionFilterDefinition<T>(t => t.Id == entity.Id)));
                        break;
                }
            }

            return writeModel;
        }
    }
}