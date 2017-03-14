// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataContextProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data context provider base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Composition;
    using Kephas.Data.Composition;
    using Kephas.Data.Resources;
    using Kephas.Data.Store;
    using Kephas.Services;

    /// <summary>
    /// Default implementation of a <see cref="IDataContextProvider"/>.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataContextProvider : IDataContextProvider
    {
        /// <summary>
        /// The data store provider.
        /// </summary>
        private readonly IDataStoreProvider dataStoreProvider;

        /// <summary>
        /// Dictionary of data context factories.
        /// </summary>
        private readonly Dictionary<string, List<IExportFactory<IDataContext, DataContextMetadata>>> dataContextFactoryDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataContextProvider"/> class.
        /// </summary>
        /// <param name="dataContextFactories">The data context factories.</param>
        /// <param name="dataStoreProvider">The data store provider.</param>
        protected DefaultDataContextProvider(
            ICollection<IExportFactory<IDataContext, DataContextMetadata>> dataContextFactories,
            IDataStoreProvider dataStoreProvider)
        {
            Contract.Requires(dataContextFactories != null);
            Contract.Requires(dataStoreProvider != null);

            var factoriesByDataStoreKind =
                dataContextFactories.SelectMany(
                    f =>
                        f.Metadata.SupportedDataStoreKinds.Any()
                            ? f.Metadata.SupportedDataStoreKinds.ToDictionary(k => k, k => f)
                            : new Dictionary<string, IExportFactory<IDataContext, DataContextMetadata>> { { string.Empty, f } });

            this.dataContextFactoryDictionary =
                (from kv in factoriesByDataStoreKind
                 group kv by kv.Key into dataStoreGroup
                 select dataStoreGroup)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Value.Metadata.ProcessingPriority).Select(i => i.Value).ToList());

            this.dataStoreProvider = dataStoreProvider;
        }

        /// <summary>
        /// Gets a data context for the provided data store name.
        /// </summary>
        /// <param name="dataStoreName">Name of the data store.</param>
        /// <returns>
        /// The new data context.
        /// </returns>
        public IDataContext GetDataContext(string dataStoreName)
        {
            var dataStore = this.dataStoreProvider.GetDataStore(dataStoreName);
            var dataContextFactories = this.dataContextFactoryDictionary.TryGetValue(dataStore.Kind);
            IDataContext dataContext = null;
            if (dataStore.DataContextType != null)
            {
                var factory =
                    dataContextFactories.FirstOrDefault(
                        f => f.Metadata.AppServiceImplementationType == dataStore.DataContextType);
                if (factory == null)
                {
                    throw new DataException(string.Format(Strings.DefaultDataContextProvider_DataStoreDataContextTypeNotFound_Exception, dataStore.DataContextType, dataStore.Name));
                }

                dataContext = factory.CreateExportedValue();
            }
            else if (dataContextFactories == null)
            {
                throw new DataException(string.Format(Strings.DefaultDataContextProvider_DataContextNotFoundForDataStoreKind_Exception, dataStore.Name, dataStore.Kind));
            }
            else if (dataContextFactories.Count > 1
                     && dataContextFactories[0].Metadata.ProcessingPriority
                     == dataContextFactories[1].Metadata.ProcessingPriority)
            {
                throw new DataException(
                    string.Format(
                        Strings.DefaultDataContextProvider_AmbiguousDataContext_Exception,
                        dataStore.Name,
                        dataContextFactories[0].Metadata.AppServiceImplementationType,
                        dataContextFactories[1].Metadata.AppServiceImplementationType));
            }
            else
            {
                dataContext = dataContextFactories[0].CreateExportedValue();
            }

            dataContext.Initialize(dataStore.DataContextConfiguration);
            return dataContext;
        }
    }
}