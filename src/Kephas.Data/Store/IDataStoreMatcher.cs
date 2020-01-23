// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStoreMatcher.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataStoreMatcher interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Interface for data store matcher.
    /// </summary>
    [AppServiceContract(AllowMultiple = true)]
    public interface IDataStoreMatcher
    {
        /// <summary>
        /// Gets the data store name for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data store name and a flag indicating whether the matching was successful.
        /// </returns>
        (string dataStoreName, bool canHandle) GetDataStoreName(Type entityType, IContext context = null);

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <param name="dataStoreName">Name of the data store.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data store.
        /// </returns>
        (IDataStore dataStore, bool canHandle) GetDataStore(string dataStoreName, IContext context = null);
    }
}
