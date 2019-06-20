// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRefPropertiesProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IRefPropertiesProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Analysis
{
    using System.Collections.Generic;

    using Kephas.Data;
    using Kephas.Services;

    /// <summary>
    /// Interface for reference properties provider.
    /// </summary>
    [SharedAppServiceContract]
    public interface IRefPropertiesProvider
    {
        /// <summary>
        /// Gets the reference properties of the provided entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>
        /// An enumeration of reference properties.
        /// </returns>
        IEnumerable<IRef> GetRefProperties(object entity);
    }
}