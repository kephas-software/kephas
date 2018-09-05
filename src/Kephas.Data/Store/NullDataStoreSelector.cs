// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullDataStoreSelector.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null data store selector class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// A null data store selector.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullDataStoreSelector : IDataStoreSelector
    {
        /// <summary>
        /// Gets the data store name for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data store name.
        /// </returns>
        public string GetDataStoreName(Type entityType, IContext context = null)
        {
            throw new NotImplementedException("Please provide a proper data store selector.");
        }
    }
}