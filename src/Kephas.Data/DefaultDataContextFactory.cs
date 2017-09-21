// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataContextFactory.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context provider base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Data.Composition;
    using Kephas.Data.Resources;
    using Kephas.Data.Store;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Default implementation of a <see cref="IDataContextFactory"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataContextFactory : IDataContextFactory
    {
        /// <summary>
        /// The data store provider.
        /// </summary>
        private readonly IDataStoreProvider dataStoreProvider;

        /// <summary>
        /// Dictionary of data context factories.
        /// </summary>
        private readonly Dictionary<string, List<IExportFactory<IDataContext, DataContextMetadata>>> factoriesPerSupportedDataSource;

        /// <summary>
        /// Dictionary of data context factories.
        /// </summary>
        private readonly ConcurrentDictionary<string, IExportFactory<IDataContext>> factories
            = new ConcurrentDictionary<string, IExportFactory<IDataContext>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataContextFactory"/> class.
        /// </summary>
        /// <param name="dataContextFactories">The data context factories.</param>
        /// <param name="dataStoreProvider">The data store provider.</param>
        public DefaultDataContextFactory(
            ICollection<IExportFactory<IDataContext, DataContextMetadata>> dataContextFactories,
            IDataStoreProvider dataStoreProvider)
        {
            Requires.NotNull(dataContextFactories, nameof(dataContextFactories));
            Requires.NotNull(dataStoreProvider, nameof(dataStoreProvider));

            var factoriesByDataStoreKind =
                dataContextFactories.SelectMany(
                    f =>
                        f.Metadata.SupportedDataStoreKinds.Any()
                            ? f.Metadata.SupportedDataStoreKinds.ToDictionary(k => k, k => f)
                            : new Dictionary<string, IExportFactory<IDataContext, DataContextMetadata>> { { string.Empty, f } });

            this.factoriesPerSupportedDataSource =
                (from kv in factoriesByDataStoreKind
                 group kv by kv.Key into dataStoreGroup
                 select dataStoreGroup)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Value.Metadata.ProcessingPriority).Select(i => i.Value).ToList());

            this.dataStoreProvider = dataStoreProvider;
        }

        /// <summary>
        /// Creates a data context for the provided data store name.
        /// </summary>
        /// <param name="dataStoreName">Name of the data store.</param>
        /// <param name="initializationContext">An initialization context (optional).</param>
        /// <returns>
        /// The newly created data context.
        /// </returns>
        public IDataContext CreateDataContext(string dataStoreName, IContext initializationContext = null)
        {
            var dataStore = this.dataStoreProvider.GetDataStore(dataStoreName);
            var factory = this.factories.GetOrAdd(dataStoreName, _ => this.ComputeDataContextExportFactory(dataStore));
            var dataContext = factory.CreateExportedValue();

            dataContext.Initialize(this.CreateDataInitializationContext(dataContext, dataStore, initializationContext));
            return dataContext;
        }

        /// <summary>
        /// Creates the data initialization context.
        /// </summary>
        /// <param name="dataContext">Context for the data.</param>
        /// <param name="dataStore">The data store.</param>
        /// <param name="initializationContext">An initialization context.</param>
        /// <returns>
        /// The new data initialization context.
        /// </returns>
        protected virtual IDataInitializationContext CreateDataInitializationContext(
            IDataContext dataContext,
            IDataStore dataStore,
            IContext initializationContext)
        {
            return new DataInitializationContext(dataContext, dataStore, initializationContext);
        }

        /// <summary>
        /// Calculates the data context export factory for the provided data source name.
        /// </summary>
        /// <param name="dataStore">The data store.</param>
        /// <returns>
        /// The calculated data context export factory.
        /// </returns>
        protected virtual IExportFactory<IDataContext> ComputeDataContextExportFactory(IDataStore dataStore)
        {
            var dataContextFactories = this.factoriesPerSupportedDataSource.TryGetValue(dataStore.Kind);
            if (dataStore.DataContextType != null)
            {
                var factory =
                    dataContextFactories.FirstOrDefault(
                        f => f.Metadata.AppServiceImplementationType == dataStore.DataContextType);
                if (factory == null)
                {
                    return new ExportFactory<IDataContext>((Func<IDataContext>)(() =>
                            {
                                throw new DataException(
                                    string.Format(
                                        Strings.DefaultDataContextProvider_DataStoreDataContextTypeNotFound_Exception,
                                        dataStore.DataContextType,
                                        dataStore.Name));
                            }));
                }

                return factory;
            }

            if (dataContextFactories == null)
            {
                return new ExportFactory<IDataContext>((Func<IDataContext>)(() =>
                    {
                        throw new DataException(
                            string.Format(
                                Strings.DefaultDataContextProvider_DataContextNotFoundForDataStoreKind_Exception,
                                dataStore.Name,
                                dataStore.Kind));
                    }));
            }

            if (dataContextFactories.Count > 1
                && dataContextFactories[0].Metadata.ProcessingPriority == dataContextFactories[1].Metadata.ProcessingPriority)
            {
                return new ExportFactory<IDataContext>((Func<IDataContext>)(() =>
                    {
                        throw new AmbiguousMatchDataException(
                            string.Format(
                                Strings.DefaultDataContextProvider_AmbiguousDataContext_Exception,
                                dataStore.Name,
                                dataContextFactories[0].Metadata.AppServiceImplementationType,
                                dataContextFactories[1].Metadata.AppServiceImplementationType));
                    }));
            }

            return dataContextFactories[0];
        }
    }
}