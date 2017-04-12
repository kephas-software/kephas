// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullDataStoreProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the null data store provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// A null data store provider.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullDataStoreProvider : IDataStoreProvider
    {
        /// <summary>
        /// Gets data store with the provided name.
        /// </summary>
        /// <param name="dataStoreName">Name of the data store.</param>
        /// <returns>
        /// The data store.
        /// </returns>
        public IDataStore GetDataStore(string dataStoreName)
        {
            throw new NotSupportedException($"Please provide a proper implementation of the {typeof(IDataStoreProvider).FullName} service. The Null implementation cannot resolve the data store {dataStoreName}.");
        }
    }
}