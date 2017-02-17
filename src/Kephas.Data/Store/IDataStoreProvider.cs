// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStoreProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataStoreProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using Kephas.Services;

    /// <summary>
    /// Provides the <see cref="GetDataStore"/> method for getting a data store by name.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataStoreProvider
    {
        /// <summary>
        /// Gets data store with the provided name.
        /// </summary>
        /// <param name="dataStoreName">Name of the data store.</param>
        /// <returns>
        /// The data store.
        /// </returns>
        IDataStore GetDataStore(string dataStoreName);
    }
}