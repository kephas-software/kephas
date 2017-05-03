// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextCache.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    public interface IDataContextCache : IDictionary<Id, IEntityInfo>
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