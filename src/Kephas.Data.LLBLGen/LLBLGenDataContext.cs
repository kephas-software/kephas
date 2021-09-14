// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LLBLGenDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the llbl generate data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.LLBLGen
{
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Data;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Caching;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Linq;
    using Kephas.Data.LLBLGen.Entities;
    using Kephas.Data.LLBLGen.Linq;
    using Kephas.Data.Store;
    using Kephas.Model;

    using SD.LLBLGen.Pro.ORMSupportClasses;

    /// <summary>
    ///     A data context for the LLBLGen infrastructure.
    /// </summary>
    [SupportedDataStoreKinds(LLBLGenDataStoreKind)]
    public class LLBLGenDataContext : DataContextBase, ILLBLGenDataContext
    {
        /// <summary>
        /// The LLBLGen data store kind.
        /// </summary>
        public const string LLBLGenDataStoreKind = "LLBLGen";

        /// <summary>
        /// The data access adapter factory.
        /// </summary>
        private readonly IDataAccessAdapterFactory dataAccessAdapterFactory;

        /// <summary>
        /// The model type resolver.
        /// </summary>
        private readonly IModelTypeResolver modelTypeResolver;

        /// <summary>
        /// The query factory provider.
        /// </summary>
        private readonly IQueryFactoryProvider queryFactoryProvider;

        /// <summary>
        ///     The data access adapter.
        /// </summary>
        private DataAccessAdapterBase dataAccessAdapter;

        /// <summary>
        ///     Meta-data for the construction of LINQ queries.
        /// </summary>
        private IQueryFactory linqMetaData;

        /// <summary>
        /// Initializes a new instance of the <see cref="LLBLGenDataContext" /> class.
        /// </summary>
        /// <param name="injector">The composition context.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        /// <param name="dataAccessAdapterFactory">The data access adapter factory.</param>
        /// <param name="modelTypeResolver">The model type resolver.</param>
        /// <param name="dataBehaviorProvider">The data behavior provider.</param>
        /// <param name="queryFactoryProvider">The query factory provider.</param>
        public LLBLGenDataContext(
            IInjector injector,
            IDataCommandProvider dataCommandProvider,
            IDataAccessAdapterFactory dataAccessAdapterFactory,
            IModelTypeResolver modelTypeResolver,
            IDataBehaviorProvider dataBehaviorProvider,
            IQueryFactoryProvider queryFactoryProvider)
            : base(injector, dataCommandProvider, dataBehaviorProvider, localCache: new LLBLGenCache())
        {
            this.dataAccessAdapterFactory = dataAccessAdapterFactory;
            this.modelTypeResolver = modelTypeResolver;
            this.queryFactoryProvider = queryFactoryProvider;

            var localCache = (LLBLGenCache)this.LocalCache;
            localCache.EntityInfoFactory = entity => this.CreateEntityEntry(entity);
        }

        /// <summary>
        /// Gets the data access adapter.
        /// </summary>
        public virtual DataAccessAdapterBase DataAccessAdapter
            =>
                this.dataAccessAdapter ??
                (this.dataAccessAdapter = this.dataAccessAdapterFactory.CreateDataAccessAdapter(this));

        /// <summary>
        ///     Gets the meta-data for the construction of LINQ queries.
        /// </summary>
        public virtual IQueryFactory QueryFactory =>
            this.linqMetaData ?? (this.linqMetaData = this.queryFactoryProvider.CreateQueryFactory(this.DataAccessAdapter, this.LocalCache as Context));

        /// <summary>
        /// Gets the local cache where the session entities are stored.
        /// </summary>
        /// <value>
        /// The local cache.
        /// </value>
        public new IDataContextCache LocalCache => base.LocalCache;

        /// <summary>
        /// Gets a query over the entity type for the given query operation context, if any is provided
        /// (core implementation).
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        protected override IQueryable<T> QueryCore<T>(IQueryOperationContext queryOperationContext)
        {
            var nativeQuery = this.QueryFactory.ToQueryable<T>();
            var provider = new LLBLGenQueryProvider(queryOperationContext, nativeQuery.Provider);
            var queryAdapter = new DataContextQuery<T>(provider, nativeQuery);
            return queryAdapter;
        }

        /// <summary>Creates a new entity information.</summary>
        /// <param name="entity">The entity.</param>
        /// <param name="changeState">The entity's change state.</param>
        /// <returns>The new entity information.</returns>
        protected override IEntityEntry CreateEntityEntry(object entity, ChangeState? changeState = null)
        {
            var entityEntry = new LLBLGenEntityEntry(entity, this.modelTypeResolver) { DataContext = this };
            if (changeState != null)
            {
                entityEntry.ChangeState = changeState.Value;
            }

            var entityEntryAware = entity as IEntityEntryAware;
            entityEntryAware?.SetEntityEntry(entityEntry);

            return entityEntry;
        }

        /// <summary>
        /// Resolves the attach conflict between an existing attached entity and a challenger.
        /// </summary>
        /// <remarks>
        /// By default, the entity challenger is ignored. A derived class may decide to update the existing entity with refreshed information.
        /// </remarks>
        /// <param name="entityEntry">The entity information.</param>
        /// <param name="entityChallenger">The entity challenger.</param>
        /// <param name="attachEntityGraph"><c>true</c> to attach the whole entity graph.</param>
        /// <returns>
        /// An resolved <see cref="T:Kephas.Data.Capabilities.IEntityEntry" />.
        /// </returns>
        protected override IEntityEntry ResolveAttachConflict(IEntityEntry entityEntry, object entityChallenger, bool attachEntityGraph)
        {
            // TODO this case occurs when querying for entities while having some of them already in the cache
            // Currently, the cached entity is preserved, as it may be referenced by other entities in cache
            // and the challenger is simply ignored. A better approach would be, for not changed entities,
            // to update the content of the cached entity with the newest information.
            return entityEntry;
        }
    }
}