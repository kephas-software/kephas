// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base implementation of a data context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Kephas.Activation;
    using Kephas.Collections;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Resources;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using Kephas.Services.Transitions;

    /// <summary>
    /// Base implementation of a <see cref="IDataContext"/>.
    /// </summary>
    public abstract class DataContextBase : Context, IDataContext
    {
        private static readonly MethodInfo QueryMethod = ReflectionHelper.GetGenericMethodOf(_ => ((IDataContext)null!).Query<IIdentifiable>(null));

        private readonly IDataBehaviorProvider? dataBehaviorProvider;
        private readonly IDataCommandProvider dataCommandProvider;
        private readonly IRuntimeTypeRegistry typeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextBase"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="dataCommandProvider">Optional. The data command provider. If not
        ///                                   provided, the <see cref="DefaultDataCommandProvider"/>
        ///                                   will be used.</param>
        /// <param name="dataBehaviorProvider">Optional. The data behavior provider.</param>
        /// <param name="localCache">Optional. The local cache. If not provided, a new <see cref="DataContextCache"/> will be created.</param>
        protected DataContextBase(
            IServiceProvider serviceProvider,
            IDataCommandProvider? dataCommandProvider = null,
            IDataBehaviorProvider? dataBehaviorProvider = null,
            IDataContextCache? localCache = null)
            : base(serviceProvider)
        {
            this.dataBehaviorProvider = dataBehaviorProvider;
            this.dataCommandProvider = dataCommandProvider ?? new DefaultDataCommandProvider(serviceProvider);
            this.LocalCache = localCache ?? new DataContextCache();
            this.Id = Guid.NewGuid();
            this.InitializationMonitor = new InitializationMonitor<DataContextBase>(this.GetType());
            this.Logger = serviceProvider.GetLogger(this.GetType());
            this.typeRegistry = serviceProvider.Resolve<IRuntimeTypeRegistry>();
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
        public virtual IActivator EntityActivator { get; private set; } = RuntimeActivator.Instance;

        /// <summary>
        /// Gets the local cache where the session entities are stored.
        /// </summary>
        /// <value>
        /// The local cache.
        /// </value>
        protected internal virtual IDataContextCache LocalCache { get; }

        /// <summary>
        /// Gets the initialization monitor.
        /// </summary>
        protected InitializationMonitor<DataContextBase> InitializationMonitor { get; }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public void Initialize(IContext? context)
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
        /// <param name="queryConfig">Optional. The query configuration.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        public virtual IQueryable<T> Query<T>(Action<IQueryOperationContext>? queryConfig = null)
            where T : class
        {
            this.InitializationMonitor.AssertIsCompletedSuccessfully();

            using var queryOperationContext = this.CreateQueryOperationContext(queryConfig);
            var entityType = typeof(T);
            var implementationTypeInfo = this.EntityActivator.GetImplementationType(
                this.typeRegistry.GetTypeInfo(entityType),
                queryOperationContext);
            var implementationType = ((IRuntimeTypeInfo)implementationTypeInfo).Type;
            if (implementationType != entityType)
            {
                var queryMethod = QueryMethod.MakeGenericMethod(implementationType);
                var implementationQuery = queryMethod.Call(this, queryConfig);
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
            commandType = commandType ?? throw new System.ArgumentNullException(nameof(commandType));

            this.InitializationMonitor.AssertIsCompletedSuccessfully();

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
        /// the framework implementation returns each time a new <see cref="EntityEntry"/>.
        /// </remarks>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        public virtual IEntityEntry GetEntityEntry(object entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            // Try to get the entity info from the local cache.
            var entityEntry = this.LocalCache.GetEntityEntry(entity);
            return entityEntry;
        }

        /// <summary>
        /// Attaches the entity to the data context.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity entry information.
        /// </returns>
        public virtual IEntityEntry Attach(object entity)
        {
            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            return this.AttachCore(entity, attachEntityGraph: true);
        }

        /// <summary>
        /// Detaches the entity from the data context.
        /// </summary>
        /// <param name="entityEntry">The entity entry.</param>
        /// <returns>
        /// The entity entry information.
        /// </returns>
        public virtual IEntityEntry? Detach(IEntityEntry entityEntry)
        {
            entityEntry = entityEntry ?? throw new System.ArgumentNullException(nameof(entityEntry));

            return this.DetachCore(entityEntry, detachEntityGraph: true);
        }

        /// <summary>
        /// Gets the equality expression when querying for the entity ID.
        /// Typically it should be something like 'e.Id == entityId'.
        /// </summary>
        /// <param name="entityId">The actual entity ID.</param>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The expression for ID equality test.</returns>
        public virtual Expression<Func<T, bool>> GetIdEqualityExpression<T>(object entityId)
        {
            var idPropertyName = nameof(IIdentifiable.Id);
            var tp = Expression.Parameter(typeof(T), "t");
            var idProperty = Expression.Property(tp, idPropertyName);
            var idPropertyType = ((PropertyInfo)idProperty.Member).PropertyType;
            var equalsMethod = idPropertyType.GetMethod(nameof(object.Equals), new[] { entityId.GetType() });
            Expression constId = Expression.Constant(entityId);
            if (equalsMethod == null)
            {
                equalsMethod = idPropertyType.GetMethod(nameof(object.Equals), new[] { typeof(object) });
                constId = Expression.Convert(constId, typeof(object));
            }
            else
            {
                var paramType = equalsMethod.GetParameters()[0].ParameterType;
                if (paramType != entityId.GetType())
                {
                    constId = Expression.Convert(constId, paramType);
                }
            }

            return Expression.Lambda<Func<T, bool>>(
                Expression.Call(idProperty, equalsMethod!, constId),
                tp);
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
        /// Creates a new entity entry.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="changeState">The entity's change state.</param>
        /// <returns>
        /// The new entity entry.
        /// </returns>
        protected virtual IEntityEntry CreateEntityEntry(object entity, ChangeState? changeState = null)
        {
            var entityEntry = new EntityEntry(entity) { DataContext = this };
            if (changeState != null)
            {
                entityEntry.ChangeState = changeState.Value;
            }

            entityEntry.TryAttachToEntity(entity);

            return entityEntry;
        }

        /// <summary>
        /// Attaches the entity to the data context, optionally attaching the whole entity graph.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="attachEntityGraph"><c>true</c> to attach the whole entity graph.</param>
        /// <returns>
        /// The entity extended information.
        /// </returns>
        protected virtual IEntityEntry AttachCore(object entity, bool attachEntityGraph)
        {
            var entityEntry = this.GetEntityEntry(entity);
            if (entityEntry != null)
            {
                if (entity != entityEntry.Entity)
                {
                    return this.ResolveAttachConflict(entityEntry, entity, attachEntityGraph);
                }

                return entityEntry;
            }

            entityEntry = this.CreateEntityEntry(entity);
            this.LocalCache.Add(entityEntry);

            if (attachEntityGraph)
            {
                var structuralEntityGraph = entityEntry.GetStructuralEntityGraph();
                if (structuralEntityGraph != null)
                {
                    foreach (var entityPart in structuralEntityGraph)
                    {
                        // already attached if the root entity.
                        if (entityPart == entity)
                        {
                            continue;
                        }

                        this.AttachCore(entityPart, attachEntityGraph: false);
                    }
                }
            }

            return entityEntry;
        }

        /// <summary>
        /// Resolves the attach conflict between an existing attached entity and a challenger.
        /// </summary>
        /// <remarks>
        /// By default, the entity challenger is ignored. A derived class may decide to update the existing entity with refreshed information.
        /// </remarks>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="entityChallenger">The entity challenger.</param>
        /// <param name="attachEntityGraph"><c>true</c> to attach the whole entity graph.</param>
        /// <returns>
        /// An resolved <see cref="IEntityEntry"/>.
        /// </returns>
        protected virtual IEntityEntry ResolveAttachConflict(IEntityEntry entityEntry, object entityChallenger, bool attachEntityGraph)
        {
            return entityEntry;
        }

        /// <summary>
        /// Detaches the entity from the data context.
        /// </summary>
        /// <param name="entityEntry">The entity entry.</param>
        /// <param name="detachEntityGraph"><c>true</c> to detach the whole entity graph.</param>
        /// <returns>
        /// The entity extended information, or <c>null</c> if the entity is not attached to this data context.
        /// </returns>
        protected virtual IEntityEntry? DetachCore(IEntityEntry entityEntry, bool detachEntityGraph)
        {
            if (!this.LocalCache.Remove(entityEntry))
            {
                return null;
            }

            if (detachEntityGraph)
            {
                var structuralEntityGraph = entityEntry.GetStructuralEntityGraph();
                if (structuralEntityGraph != null)
                {
                    foreach (var entityPart in structuralEntityGraph)
                    {
                        // root already detached.
                        if (entityEntry.Entity == entityPart)
                        {
                            continue;
                        }

                        var partEntityEntry = this.GetEntityEntry(entityPart);
                        if (partEntityEntry != null)
                        {
                            this.DetachCore(partEntityEntry, detachEntityGraph: false);
                        }
                    }
                }
            }

            entityEntry.Dispose();
            return entityEntry;
        }

        /// <summary>
        /// Creates the query operation context.
        /// </summary>
        /// <param name="queryConfig">The query configuration.</param>
        /// <returns>
        /// The new query operation context.
        /// </returns>
        protected virtual QueryOperationContext CreateQueryOperationContext(Action<IQueryOperationContext>? queryConfig)
        {
            return new QueryOperationContext(this).Merge(queryConfig);
        }
    }
}