// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextCache.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataContextCache interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Caching
{
    using System.Collections.Generic;

    using Kephas.Data.Capabilities;

    /// <summary>
    /// Interface for data context cache.
    /// </summary>
    public interface IDataContextCache : IDictionary<object, IEntityInfo>
    {
        /// <summary>
        /// Adds an <see cref="IEntityInfo"/> to the <see cref="IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to add.</param>
        void Add(IEntityInfo value);

        /// <summary>
        /// Removes an <see cref="IEntityInfo"/> from the <see cref="IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to remove.</param>
        /// <returns>
        /// <c>true</c> if the removal succeeds, <c>false</c> otherwise.
        /// </returns>
        bool Remove(IEntityInfo value);

        /// <summary>
        /// Gets the entity information associated to the provided entity, or <c>null</c> if none could be found.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity information or <c>null</c>.
        /// </returns>
        IEntityInfo GetEntityInfo(object entity);
    }
}