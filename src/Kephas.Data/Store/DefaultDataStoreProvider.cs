// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultDataStoreProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null data store provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.Store
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A data store provider.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultDataStoreProvider : IDataStoreProvider
    {
        /// <summary>
        /// The data store mappings.
        /// </summary>
        private static readonly ConcurrentDictionary<string, IDataStore> DataStoreMappings = new ConcurrentDictionary<string, IDataStore>();

        /// <summary>
        /// The data store mappings.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, string> DataStoreTypeMappings = new ConcurrentDictionary<Type, string>();

        /// <summary>
        /// The data store matchers.
        /// </summary>
        private readonly ICollection<IDataStoreMatcher> dataStoreMatchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataStoreProvider"/> class.
        /// </summary>
        /// <param name="matcherFactories">The matcher factories.</param>
        public DefaultDataStoreProvider(ICollection<IExportFactory<IDataStoreMatcher, AppServiceMetadata>> matcherFactories)
        {
            this.dataStoreMatchers = matcherFactories
                .Order()
                .Select(f => f.CreateExportedValue())
                .ToList();
        }

        /// <summary>
        /// Gets data store with the provided name.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when the requested operation is not supported.</exception>
        /// <param name="dataStoreName">Name of the data store.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data store.
        /// </returns>
        public IDataStore GetDataStore(string dataStoreName, IContext? context = null)
        {
            Requires.NotNull(dataStoreName, nameof(dataStoreName));

            return DataStoreMappings.GetOrAdd(
                dataStoreName,
                _ =>
                {
                    foreach (var matcher in this.dataStoreMatchers)
                    {
                        var (dataStore, canHandle) = matcher.GetDataStore(dataStoreName, null);
                        if (canHandle)
                        {
                            return dataStore!;
                        }
                    }

                    // TODO localization
                    throw new NotSupportedException($"The data store '{dataStoreName}' is not supported.");
                });
        }

        /// <summary>
        /// Gets the data store name for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data store name.
        /// </returns>
        public string GetDataStoreName(Type entityType, IContext? context = null)
        {
            Requires.NotNull(entityType, nameof(entityType));

            return DataStoreTypeMappings.GetOrAdd(
                entityType,
                _ =>
                {
                    foreach (var matcher in this.dataStoreMatchers)
                    {
                        var (dataStoreName, canHandle) = matcher.GetDataStoreName(entityType, context);
                        if (canHandle)
                        {
                            return dataStoreName!;
                        }
                    }

                    throw new NotSupportedException($"The entity type '{entityType}' is not supported by any known data store.");
                });
        }
    }
}
