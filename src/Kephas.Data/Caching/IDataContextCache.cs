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
        /// Gets the entity information associated to the provided entity, or <c>null</c> if none could be found.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// The entity information or <c>null</c>.
        /// </returns>
        IEntityInfo GetEntityInfo(object entity);
    }
}