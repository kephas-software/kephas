// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStoreSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataStoreSelector interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Shared application service contract used for selecting the data store for entities.
    /// </summary>
    [SharedAppServiceContract]
    public interface IDataStoreSelector
    {
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