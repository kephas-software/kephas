// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenFindCommand.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate find command class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen.Commands
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Activation;
    using Kephas.Data;
    using Kephas.Data.Commands;
    using Kephas.Data.LLBLGen.Entities;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Runtime;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    /// The find command for the LLBLGen infrastructure.
    /// </summary>
    [DataContextType(typeof(LLBLGenDataContext))]
    public class LLBLGenFindCommand : FindCommand
    {
        /// <summary>
        /// The entity activator.
        /// </summary>
        private readonly IActivator entityActivator;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenFindCommand"/> class.
        /// </summary>
        /// <param name="entityActivator">The entity activator.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public LLBLGenFindCommand(IEntityActivator entityActivator, ILogManager logManager = null)
            : base(logManager)
        {
            this.entityActivator = entityActivator;
        }

        /// <summary>
        /// Searches for the first entity matching the provided criteria and returns it asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="operationContext">The find context.</param>
        /// <param name="cancellationToken">The cancellation token (optional).</param>
        /// <returns>
        /// A promise of the found entity.
        /// </returns>
        protected override Task<IFindResult> FindAsync<T>(
            IFindContext operationContext,
            CancellationToken cancellationToken)
        {
            Exception exception = null;

            if (Id.IsEmpty(operationContext.Id))
            {
                exception = new DataException("The provided ID is null.");
                if (operationContext.ThrowOnNotFound)
                {
                    throw exception;
                }

                return Task.FromResult((IFindResult)new FindResult(null, exception: exception));
            }

            var id = Convert.ToInt64(operationContext.Id);
            var dataContext = (LLBLGenDataContext)operationContext.DataContext;
            var cache = (LLBLGenCache)this.TryGetLocalCache(dataContext);
            var runtimeEntityType = operationContext.EntityType.AsRuntimeTypeInfo();
            var underlyingEntityType = (IRuntimeTypeInfo)this.entityActivator.GetImplementationType(runtimeEntityType);

            IEntity2 entity = null;

            // not already in cache or new.
            // 1. check whether this is new
            var cacheEntity = (IEntity2)cache.GetAll(underlyingEntityType.Type).Cast<IEntityBase>().FirstOrDefault(e => e.Id == id);
            if (cacheEntity == null)
            {
                cacheEntity = (IEntity2)cache.NewEntities.Values.OfType<IEntityBase>().FirstOrDefault(e => e.Id == id);
                if (cacheEntity == null)
                {
                    // 2. entity is not new/not already in cache, get it from the database.
                    entity = (IEntity2)underlyingEntityType.CreateInstance();
                    ((IEntityBase)entity).Id = id;
                    var success = dataContext.DataAccessAdapter.FetchEntity(entity, cache);
                    if (!success)
                    {
                        // TODO localization
                        exception = new NotFoundDataException($"{operationContext.EntityType.Name} with ID {id} was not found.");
                        entity = null;
                    }
                    else
                    {
                        dataContext.Attach(entity);
                    }
                }
            }

            if (cacheEntity != null)
            {
                entity = cacheEntity;
                dataContext.Attach(cacheEntity);
            }

            if (operationContext.ThrowOnNotFound && exception != null)
            {
                throw exception;
            }

            var result = new FindResult(entity, exception: exception);
            return Task.FromResult((IFindResult)result);
        }
    }
}