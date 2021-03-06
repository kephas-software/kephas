// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMongoNamingStrategy.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.MongoDB
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// Naming strategy used for MongoDB entities.
    /// </summary>
    [SingletonAppServiceContract]
    public interface IMongoNamingStrategy
    {
        /// <summary>
        /// Gets the collection name for the provided entity type.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The collection name.
        /// </returns>
#if NETSTANDARD2_0
        string GetCollectionName(IDataContext dataContext, Type entityType);
#else
        string GetCollectionName(IDataContext dataContext, Type entityType)
        {
            return entityType.Name;
        }
#endif
    }
}