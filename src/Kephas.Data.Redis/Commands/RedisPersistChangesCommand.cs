// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisPersistChangesCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Redis.Commands
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Data;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Redis;
    using Kephas.Serialization;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// The redis persist changes command.
    /// </summary>
    [DataContextType(typeof(RedisDataContext))]
    public class RedisPersistChangesCommand : PersistChangesCommand
    {
        /// <summary>
        /// The identifier factory.
        /// </summary>
        private readonly IIdGenerator idFactory;

        /// <summary>
        /// The serialization service.
        /// </summary>
        private readonly ISerializationService serializationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisPersistChangesCommand"/> class.
        /// </summary>
        /// <param name="behaviorProvider">The behavior provider.</param>
        /// <param name="idFactory">The identifier factory.</param>
        /// <param name="serializationService">The serialization service.</param>
        public RedisPersistChangesCommand(IDataBehaviorProvider behaviorProvider, IIdGenerator idFactory, ISerializationService serializationService)
            : base(behaviorProvider)
        {
            this.idFactory = idFactory;
            this.serializationService = serializationService;
        }

        /// <summary>
        /// Persists the entities in the change set asynchronously.
        /// </summary>
        /// <param name="changeSet">The modified entities.</param>
        /// <param name="operationContext">The data operation context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        protected override async Task PersistChangeSetAsync(
            IList<IEntityEntry> changeSet,
            IPersistChangesContext operationContext,
            CancellationToken cancellationToken)
        {
            var dataContext = (RedisDataContext)operationContext.DataContext;
            //using (var tx = dataContext.NewTransaction())
            //{
            foreach (var entry in changeSet)
            {
                var identifiableEntity = (EntityBase)entry.Entity;

                // for new entries compute the ID
                if ((entry.ChangeState == ChangeState.Added || entry.ChangeState == ChangeState.AddedOrChanged)
                    && Id.IsTemporary(entry.EntityId))
                {
                    identifiableEntity[nameof(IIdentifiable.Id)] = this.idFactory.GenerateId();
                }

                // perform operation on the hash set
                var entityHash = dataContext.GetEntityHash(entry.Entity.GetType());
                var entityKey = entry.EntityId?.ToString();
                if (entityKey == null)
                {
                    throw new DataException($"Cannot operate on an entity with a null ID: {entry.Entity}.");
                }

                //tx.QueueCommand(
                //    _ =>
                //        {
                if (entry.ChangeState == ChangeState.Deleted)
                {
                    entityHash.Remove(entityKey);
                }
                else
                {
                    if (entityHash.ContainsKey(entityKey))
                    {
                        if (entry.ChangeState == ChangeState.Added)
                        {
                            throw new DataException(
                                $"The entity with ID '{entityKey}' exists already, adding to the collection is denied.");
                        }
                    }
                    else
                    {
                        if (entry.ChangeState == ChangeState.Changed)
                        {
                            throw new DataException(
                                $"The entity with ID '{entityKey}' does not exist, changing it in the collection is denied.");
                        }
                    }

                    var serializedEntity = await this.serializationService.JsonSerializeAsync(entry.Entity, cancellationToken: cancellationToken).PreserveThreadContext();
                    entityHash.Add(entityKey, serializedEntity);
                }
                //            });
            }

            //tx.Commit();
            //}

            //return Task.FromResult(0);
        }
    }
}