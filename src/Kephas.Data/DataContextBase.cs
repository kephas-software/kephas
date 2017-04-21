// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Base implementation of a data context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Services;
    using Kephas.Services.Transitioning;

    /// <summary>
    /// Base implementation of a <see cref="IDataContext"/>.
    /// </summary>
    public abstract class DataContextBase : Expando, IDataContext
    {
        /// <summary>
        /// The initialization monitor.
        /// </summary>
        protected readonly InitializationMonitor<DataContextBase> InitializationMonitor;

        /// <summary>
        /// The data command provider.
        /// </summary>
        private readonly IDataCommandProvider dataCommandProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase"/> class.
        /// </summary>
        /// <param name="ambientServices">The ambient services.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        /// <param name="localCache">The local cache (optional). If not provided, a default one will be created.</param>
        protected DataContextBase(
            IAmbientServices ambientServices,
            IDataCommandProvider dataCommandProvider,
            IDataContextCache localCache = null)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));
            Requires.NotNull(dataCommandProvider, nameof(dataCommandProvider));

            this.AmbientServices = ambientServices;
            this.dataCommandProvider = dataCommandProvider;
            this.LocalCache = localCache ?? new DataContextCache();
            this.Id = new Id(Guid.NewGuid());
            this.InitializationMonitor = new InitializationMonitor<DataContextBase>(this.GetType());
        }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Id Id { get; }

        /// <summary>
        /// Gets the ambient services.
        /// </summary>
        /// <value>
        /// The ambient services.
        /// </value>
        public IAmbientServices AmbientServices { get; }

        /// <summary>
        /// Gets the local cache where the session entities are stored.
        /// </summary>
        /// <value>
        /// The local cache.
        /// </value>
        protected internal virtual IDataContextCache LocalCache { get; }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public void Initialize(IContext context)
        {
            var dataInitializationContext = context as IDataInitializationContext;
            if (dataInitializationContext == null)
            {
                throw new ArgumentException(string.Format(Strings.DataContextBase_BadInitializationContext_Exception, typeof(IDataInitializationContext).FullName), nameof(context));
            }

            this.InitializationMonitor.Start();
            try
            {
                this.Initialize(dataInitializationContext);
                this.InitializationMonitor.Complete();
            }
            catch (Exception ex)
            {
                this.InitializationMonitor.Fault(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets a query over the entity type for the given query operationContext, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public abstract IQueryable<T> Query<T>(IQueryOperationContext queryOperationContext = null)
            where T : class;

        /// <summary>
        /// Creates the command with the provided type.
        /// </summary>
        /// <param name="commandType">The type of the command to be created.</param>
        /// <returns>
        /// The new command.
        /// </returns>
        public IDataCommand CreateCommand(Type commandType)
        {
            Requires.NotNull(commandType, nameof(commandType));

            if (!typeof(IDataCommand).GetTypeInfo().IsAssignableFrom(commandType.GetTypeInfo()))
            {
                throw new ArgumentException(string.Format(Strings.DataContextBase_CreateCommand_BadCommandType_Exception, commandType, typeof(IDataCommand)), nameof(commandType));
            }

            return this.dataCommandProvider.CreateCommand(this.GetType(), commandType);
        }

        /// <summary>
        /// Tries to get a capability.
        /// </summary>
        /// <typeparam name="TCapability">Type of the capability.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="operationContext">Context for the operation (optional).</param>
        /// <returns>
        /// The capability.
        /// </returns>
        public virtual TCapability TryGetCapability<TCapability>(object entity, IDataOperationContext operationContext = null)
            where TCapability : class
        {
            var entityCapability = entity as TCapability;
            if (entityCapability != null)
            {
                return entityCapability;
            }

            return this.GetEntityInfo(entity) as TCapability;
        }

        /// <summary>
        /// Gets the entity extended information.
        /// </summary>
        /// <remarks>
        /// Note to inheritors: it is a good practice to override this method
        /// and provide a custom implementation, because, by default,
        /// the framework implementation returns each time a new <see cref="EntityInfo"/>.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        public virtual IEntityInfo GetEntityInfo(object entity)
        {
            Requires.NotNull(entity, nameof(entity));

            // TODO optimize, maybe set the entity info in the entity
            // if it is an expando.
            // Try to get the entity info from the local cache.
            var entityInfo = this.LocalCache.Values.FirstOrDefault(ei => ei.Entity == entity);

            return entityInfo;
        }

        /// <summary>
        /// Attaches the entity to the data context.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        public virtual IEntityInfo AttachEntity(object entity)
        {
            Requires.NotNull(entity, nameof(entity));

            var entityInfo = this.GetEntityInfo(entity);
            if (entityInfo == null)
            {
                entityInfo = this.CreateEntityInfo(entity);
                this.LocalCache.Add(entityInfo.Id, entityInfo);
            }

            return entityInfo;
        }

        /// <summary>
        /// Detaches the entity from the data context.
        /// </summary>
        /// <param name="entityInfo">The entity information.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        public virtual IEntityInfo DetachEntity(IEntityInfo entityInfo)
        {
            Requires.NotNull(entityInfo, nameof(entityInfo));

            this.LocalCache.Remove(entityInfo.Id);
            return entityInfo;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="dataInitializationContext">The data initialization context.</param>
        protected virtual void Initialize(IDataInitializationContext dataInitializationContext)
        {
        }

        /// <summary>
        /// Creates a new entity information.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="changeState">The entity's change state.</param>
        /// <returns>
        /// The new entity information.
        /// </returns>
        protected virtual IEntityInfo CreateEntityInfo(object entity, ChangeState? changeState = null)
        {
            var changeStateTracker = entity as IChangeStateTrackable;
            if (changeStateTracker != null)
            {
                if (changeState.HasValue)
                {
                    changeStateTracker.ChangeState = changeState.Value;
                }

                return new EntityInfo(entity, changeStateTracker);
            }

            return new EntityInfo(entity, changeState ?? ChangeState.NotChanged);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c>false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}