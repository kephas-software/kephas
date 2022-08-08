// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MongoDataContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the mongo data context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.MongoDB
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Security.Authentication;

    using global::MongoDB.Driver;
    using Kephas.Data.Behaviors;
    using Kephas.Data.Commands.Factory;
    using Kephas.Data.MongoDB.Linq;
    using Kephas.Data.MongoDB.Resources;
    using Kephas.Data.Store;
    using Kephas.MongoDB;
    using Kephas.MongoDB.Diagnostics;
    using Kephas.Runtime;

    /// <summary>
    /// A data context for MongoDB.
    /// </summary>
    [SupportedDataStoreKinds(DataStoreKind.MongoDB)]
    public class MongoDataContext : DataContextBase
    {
        private readonly IRuntimeTypeRegistry typeRegistry;
        private readonly IMongoClientProvider clientProvider;
        private readonly IMongoNamingStrategy namingStrategy;
        private readonly IMongoClientSettingsProvider mongoClientSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoDataContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="dataCommandProvider">The data command provider.</param>
        /// <param name="dataBehaviorProvider">The data behavior provider.</param>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="clientProvider">The client provider.</param>
        /// <param name="namingStrategy">The naming strategy.</param>
        /// <param name="mongoClientSettingsProvider">The MongoDB client settings provider.</param>
        public MongoDataContext(
            IInjector injector,
            IDataCommandProvider dataCommandProvider,
            IDataBehaviorProvider dataBehaviorProvider,
            IRuntimeTypeRegistry typeRegistry,
            IMongoClientProvider clientProvider,
            IMongoNamingStrategy namingStrategy,
            IMongoClientSettingsProvider mongoClientSettingsProvider)
            : base(injector, dataCommandProvider, dataBehaviorProvider)
        {
            this.typeRegistry = typeRegistry ?? throw new ArgumentNullException(nameof(typeRegistry));
            this.clientProvider = clientProvider ?? throw new ArgumentNullException(nameof(clientProvider));
            this.namingStrategy = namingStrategy ?? throw new ArgumentNullException(nameof(namingStrategy));
            this.mongoClientSettingsProvider = mongoClientSettingsProvider;
        }

        /// <summary>
        /// Gets the MongoDB client.
        /// </summary>
        /// <value>
        /// The MongoDB client.
        /// </value>
        public IMongoClient Client { get; private set; }

        /// <summary>
        /// Gets the MongoDB database.
        /// </summary>
        /// <value>
        /// The MongoDB database.
        /// </value>
        public IMongoDatabase Database { get; private set; }

        /// <summary>
        /// Gets a query over the entity type for the given query operation context, if any is provided.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryOperationContext">Context for the query.</param>
        /// <returns>
        /// A query over the entity type.
        /// </returns>
        protected override IQueryable<T> QueryCore<T>(IQueryOperationContext queryOperationContext)
        {
            var nativeQuery = this.Database.GetCollection<T>(
                this.namingStrategy.GetCollectionName(typeof(T)))
                .AsQueryable();
            var provider = new MongoQueryProvider(queryOperationContext, nativeQuery.Provider, this.typeRegistry);
            var query = new MongoQuery<T>(provider, nativeQuery, this.typeRegistry);
            return query;
        }

        /// <summary>
        /// Initializes the <see cref="MongoDataContext"/>.
        /// </summary>
        /// <param name="dataInitializationContext">The data initialization context.</param>
        protected override void Initialize(IDataInitializationContext dataInitializationContext)
        {
            if (dataInitializationContext.DataStore == null)
            {
                throw new ArgumentException("The data store is not set in the initialization context.", nameof(dataInitializationContext));
            }

            if (dataInitializationContext.DataStore.DataContextSettings == null)
            {
                throw new ArgumentException("The data context settings are not set in the data store from the initialization context.", nameof(dataInitializationContext));
            }

            base.Initialize(dataInitializationContext);

            var config = dataInitializationContext.DataStore.DataContextSettings;
            var mongoUrl = MongoUrl.Create(config.ConnectionString);
            var databaseName = mongoUrl.DatabaseName;
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new MongoDataException(Strings.Initialize_DatabaseNameEmpty_Exception);
            }

            this.Client = this.clientProvider.GetMongoClient(new MongoContext(this.Injector) { ConnectionString = config.ConnectionString });
            this.Database = this.Client.GetDatabase(databaseName);
        }
    }
}