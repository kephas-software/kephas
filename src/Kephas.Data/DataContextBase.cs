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
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Activation;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Runtime;
    using Kephas.Security;
    using Kephas.Services;
    using Kephas.Services.Transitioning;

    /// <summary>
    /// Base implementation of a <see cref="IDataContext"/>.
    /// </summary>
    public abstract class DataContextBase : Context, IDataContext
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
        /// <param name="dataCommandProvider">The data command provider (optional). If not provided, the <see cref="DefaultDataCommandProvider"/> will be used.</param>
        /// <param name="entityActivator">The entity activator (optional). If not provided, the <see cref="RuntimeActivator"/> will be used.</param>
        /// <param name="identityProvider">The identity provider (optional).</param>
        /// <param name="localCache">The local cache (optional). If not provided, a new <see cref="DataContextCache"/> will be created.</param>
        protected DataContextBase(
            IAmbientServices ambientServices,
            IDataCommandProvider dataCommandProvider = null,
            IActivator entityActivator = null,
            IIdentityProvider identityProvider = null,
            IDataContextCache localCache = null)
            : base(ambientServices)
        {
            Requires.NotNull(ambientServices, nameof(ambientServices));

            this.Identity = identityProvider?.GetCurrentIdentity();
            this.dataCommandProvider = dataCommandProvider ?? new DefaultDataCommandProvider(ambientServices.CompositionContainer);
            this.LocalCache = localCache ?? new DataContextCache();
            this.EntityActivator = entityActivator ?? RuntimeActivator.Instance;
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
        /// Gets the local cache where the session entities are stored.
        /// </summary>
        /// <value>
        /// The local cache.
        /// </value>
        protected internal virtual IDataContextCache LocalCache { get; }

        /// <summary>
        /// Gets the entity activator.
        /// </summary>
        /// <value>
        /// The entity activator.
        /// </value>
        protected internal virtual IActivator EntityActivator { get; }

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

            // Try to get the entity info from the local cache.
            var entityInfo = this.LocalCache.GetEntityInfo(entity);
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

            return this.AttachEntityCore(entity, attachEntityGraph: true);
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

            return this.DetachEntityCore(entityInfo, detachEntityGraph: true);
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
        /// Gets the equality expression for of: t =&gt; t.Id == entityInfo.Id.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entityId">The entity ID.</param>
        /// <returns>
        /// The equality expression.
        /// </returns>
        protected internal virtual Expression<Func<T, bool>> GetIdEqualityExpression<T>(Id entityId)
        {
            return t => ((IIdentifiable)t).Id == entityId;
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
            return new EntityInfo(entity);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c>false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Attaches the entity to the data context, optionally attaching the whole entity graph.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="attachEntityGraph"><c>true</c> to attach the whole entity graph.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        protected virtual IEntityInfo AttachEntityCore(object entity, bool attachEntityGraph)
        {
            var entityInfo = this.GetEntityInfo(entity);
            if (entityInfo != null)
            {
                return entityInfo;
            }

            entityInfo = this.CreateEntityInfo(entity);
            this.LocalCache.Add(entityInfo);

            if (attachEntityGraph)
            {
                var structuralEntityGraph = entityInfo.GetStructuralEntityGraph();
                if (structuralEntityGraph != null)
                {
                    foreach (var entityPart in structuralEntityGraph)
                    {
                        // already attached if the root entity.
                        if (entityPart == entity)
                        {
                            continue;
                        }

                        this.AttachEntityCore(entityPart, attachEntityGraph: false);
                    }
                }
            }

            return entityInfo;
        }

        /// <summary>
        /// Detaches the entity from the data context.
        /// </summary>
        /// <param name="entityInfo">The entity information.</param>
        /// <param name="detachEntityGraph"><c>true</c> to detach the whole entity graph.</param>
        /// <returns>
        /// The entity extended information, or <c>null</c> if the entity is not attached to this data context.
        /// </returns>
        protected virtual IEntityInfo DetachEntityCore(IEntityInfo entityInfo, bool detachEntityGraph)
        {
            if (!this.LocalCache.Remove(entityInfo))
            {
                return null;
            }

            if (detachEntityGraph)
            {
                var structuralEntityGraph = entityInfo.GetStructuralEntityGraph();
                if (structuralEntityGraph != null)
                {
                    foreach (var entityPart in structuralEntityGraph)
                    {
                        // root already detached.
                        if (entityInfo.Entity == entityPart)
                        {
                            continue;
                        }

                        var partEntityInfo = this.GetEntityInfo(entityPart);
                        if (partEntityInfo != null)
                        {
                            this.DetachEntityCore(partEntityInfo, detachEntityGraph: false);
                        }
                    }
                }
            }

            entityInfo.Dispose();
            return entityInfo;
        }
    }
}