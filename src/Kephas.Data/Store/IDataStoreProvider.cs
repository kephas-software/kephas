// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStoreProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataStoreProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;

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
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data store.
        /// </returns>
        IDataStore GetDataStore(string dataStoreName, IContext context = null);

        /// <summary>
        /// Gets the data store name for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data store name.
        /// </returns>
        string GetDataStoreName(Type entityType, IContext context = null);
    }
}