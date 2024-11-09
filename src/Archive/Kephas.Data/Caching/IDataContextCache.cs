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
    public interface IDataContextCache : IDictionary<object, IEntityEntry>
    {
        /// <summary>
        /// Adds an <see cref="IEntityEntry"/> to the <see cref="IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to add.</param>
        void Add(IEntityEntry value);

        /// <summary>
        /// Removes an <see cref="IEntityEntry"/> from the <see cref="IDataContextCache" />.
        /// </summary>
        /// <param name="value">The object to remove.</param>
        /// <returns>
        /// <c>true</c> if the removal succeeds, <c>false</c> otherwise.
        /// </returns>
        bool Remove(IEntityEntry value);

        /// <summary>
        /// Gets the entity entry associated to the provided entity, or <c>null</c> if none could be found.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity entry or <c>null</c>.
        /// </returns>
        IEntityEntry GetEntityEntry(object entity);
    }
}