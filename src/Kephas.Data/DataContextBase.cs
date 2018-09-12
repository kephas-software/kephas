// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Resources;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Reflection;
    using Kephas.Runtime;
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
        /// The query method.
        /// </summary>
        private static readonly MethodInfo QueryMethod = ReflectionHelper.GetGenericMethodOf(_ => ((IDataContext)null).Query<IIdentifiable>(null));

        /// <summary>
        /// The data behavior provider.
        /// </summary>
        private readonly IDataBehaviorProvider dataBehaviorProvider;

        /// <summary>
        /// The data command provider.
        /// </summary>
        private readonly IDataCommandProvider dataCommandProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase"/> class.
        /// </summary>
        /// <param name="compositionContext">Optional. The composition context.</param>
        /// <param name="dataCommandProvider">Optional. The data command provider. If not
        ///                                   provided, the <see cref="DefaultDataCommandProvider"/>
        ///                                   will be used.</param>
        /// <param name="dataBehaviorProvider">Optional. The data behavior provider.</param>
        /// <param name="localCache">Optional. The local cache. If not provided, a new <see cref="DataContextCache"/> will be created.</param>
        protected DataContextBase(
            ICompositionContext compositionContext = null,
            IDataCommandProvider dataCommandProvider = null,
            IDataBehaviorProvider dataBehaviorProvider = null,
            IDataContextCache localCache = null)
            : base(compositionContext)
        {
            this.dataBehaviorProvider = dataBehaviorProvider;
            this.dataCommandProvider = dataCommandProvider ?? new DefaultDataCommandProvider(compositionContext);
            this.LocalCache = localCache ?? new DataContextCache();
            this.Id = Guid.NewGuid();
            this.InitializationMonitor = new InitializationMonitor<DataContextBase>(this.GetType());
        }

        /// <summary>
        /// Gets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public object Id { get; }

        /// <summary>
        /// Gets the entity activator.
        /// </summary>
        /// <value>
        /// The entity activator.
        /// </value>
        public virtual IActivator EntityActivator { get; private set; }

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
            if (!(context is IDataInitializationContext dataInitializationContext))
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
        /// Gets a query over the entity type for the given query operation context, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public virtual IQueryable<T> Query<T>(IQueryOperationContext queryOperationContext = null)
            where T : class
        {
            queryOperationContext = queryOperationContext ?? new QueryOperationContext(this);
            var entityType = typeof(T);
            var implementationTypeInfo = this.EntityActivator.GetImplementationType(
                entityType.AsRuntimeTypeInfo(),
                queryOperationContext);
            var implementationType = ((IRuntimeTypeInfo)implementationTypeInfo).Type;
            if (implementationType != entityType)
            {
                var queryMethod = QueryMethod.MakeGenericMethod(implementationType);
                var implementationQuery = queryMethod.Call(this, queryOperationContext);
                return (IQueryable<T>)implementationQuery;
            }

            var queryBehaviors = this.dataBehaviorProvider?.GetDataBehaviors<IOnQueryBehavior>(typeof(T));
            queryBehaviors?.ForEach(b => b.BeforeQuery(typeof(T), queryOperationContext));

            var query = this.QueryCore<T>(queryOperationContext);
            queryOperationContext.Query = query;

            queryBehaviors?.ForEach(b => b.AfterQuery(typeof(T), queryOperationContext));
            query = (IQueryable<T>)queryOperationContext.Query;
            return query;
        }

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
        protected internal virtual Expression<Func<T, bool>> GetIdEqualityExpression<T>(object entityId)
        {
            return t => entityId.Equals(((IIdentifiable)t).Id);
        }

        /// <summary>
        /// Gets a query over the entity type for the given query operation context, if any is provided (core implementation).
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        protected abstract IQueryable<T> QueryCore<T>(IQueryOperationContext queryOperationContext)
            where T : class;

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="dataInitializationContext">The data initialization context.</param>
        protected virtual void Initialize(IDataInitializationContext dataInitializationContext)
        {
            this.EntityActivator = dataInitializationContext?.DataStore.EntityActivator ?? RuntimeActivator.Instance;
            this.Identity = dataInitializationContext?.Identity
                            ?? dataInitializationContext?.InitializationContext?.Identity;
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
            var entityInfo = new EntityInfo(entity) { DataContext = this };
            if (changeState != null)
            {
                entityInfo.ChangeState = changeState.Value;
            }

            entityInfo.TryAttachToEntity(entity);

            return entityInfo;
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