// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RedisDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Redis
{
    using System;
    using System.Linq;

    using Kephas.Configuration;
    using Kephas.Data;
    using Kephas.Data.Capabilities;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.Linq;
    using Kephas.Data.Redis.Configuration;
    using Kephas.Data.Store;
    using Kephas.Injection;
    using Kephas.Redis;
    using Kephas.Serialization;
    using StackExchange.Redis;

    /// <summary>
    /// The Redis data context.
    /// </summary>
    [SupportedDataStoreKinds(DataStoreKind.Redis)]
    public class RedisDataContext : DataContextBase
    {
        private const int DefaultDatabase = 1;

        private readonly IRedisConnectionManager connectionManager;
        private readonly ISerializationService serializationService;
        private readonly IConfiguration<RedisDbSettings> dbConfiguration;

        private IDatabase? database;
        private IConnectionMultiplexer? dbConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisDataContext"/> class.
        /// </summary>
        /// <param name="connectionManager">The redis connection manager.</param>
        /// <param name="injector">The injector.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        /// <param name="serializationService">The serialization service.</param>
        /// <param name="dbConfiguration">The Redis database settings.</param>
        public RedisDataContext(
            IRedisConnectionManager connectionManager,
            IInjector injector,
            IDataCommandProvider dataCommandProvider,
            ISerializationService serializationService,
            IConfiguration<RedisDbSettings> dbConfiguration)
        : base(injector, dataCommandProvider)
        {
            this.connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
            this.serializationService = serializationService ?? throw new ArgumentNullException(nameof(serializationService));
            this.dbConfiguration = dbConfiguration;
        }

        /// <summary>
        /// Gets the entity hash.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>
        /// The list.
        /// </returns>
        protected internal virtual IRedisHash GetEntityHash(Type type)
        {
            var listName = this.GetListName(type);
            return new RedisHash(this.database!, listName);
        }

        /// <summary>
        /// Gets the entity hash.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <returns>
        /// The list.
        /// </returns>
        protected internal virtual IRedisHash<T> GetEntityHash<T>()
            where T : class
        {
            var listName = this.GetListName(typeof(T));
            return new RedisHash<T>(this.database!, listName, this.serializationService);
        }

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        /// <param name="dataInitializationContext">The data initialization context.</param>
        protected override void Initialize(IDataInitializationContext dataInitializationContext)
        {
            base.Initialize(dataInitializationContext);

            var settings = this.dbConfiguration.GetSettings(dataInitializationContext);
            this.dbConnection = this.connectionManager.CreateConnection();
            this.database = this.dbConnection.GetDatabase(settings?.Database ?? DefaultDatabase);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Services.Context and optionally releases
        /// the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.dbConnection != null)
            {
                this.connectionManager.DisposeConnection(this.dbConnection);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets a query over the entity type for the given query operationContext, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>A query over the entity type.</returns>
        protected override IQueryable<T> QueryCore<T>(IQueryOperationContext queryOperationContext)
        {
            var entityHash = this.GetEntityHash<T>();
            var nativeQuery = entityHash.AsQueryable();
            var provider = new RedisQueryProvider(queryOperationContext, nativeQuery.Provider);
            var queryAdapter = new DataContextQuery<T>(provider, nativeQuery);
            return queryAdapter;
        }

        /// <summary>
        /// Gets the type's list name.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>
        /// The list name.
        /// </returns>
        protected virtual string GetListName(Type type)
        {
            // TODO enable multiple databases as specified in the config.
            return $"urn:db:{type.FullName}";
        }

        /// <summary>
        /// Attaches the entity to the data context, optionally attaching the whole entity graph.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="attachEntityGraph"><c>true</c> to attach the whole entity graph.</param>
        /// <returns>The entity extended information.</returns>
        protected override IEntityEntry AttachCore(object entity, bool attachEntityGraph)
        {
            var localCache = this.LocalCache;
            var entityId = ((EntityBase)entity)[nameof(IIdentifiable.Id)];
            if (entityId != null)
            {
                var cachedEntityInfo = localCache.Values.FirstOrDefault(ei => Equals(ei.EntityId, entityId));
                if (cachedEntityInfo != null)
                {
                    return cachedEntityInfo;
                }
            }

            return base.AttachCore(entity, attachEntityGraph);
        }
    }
}