// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSpace.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataSpace interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;

    /// <summary>
    /// Interface for data context container.
    /// </summary>
    [AppServiceContract]
    public interface IDataSpace : IContext, IReadOnlyCollection<IDataContext>, IDisposable, IInitializable
    {
        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="context">Optional. The context.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        IDataContext this[Type entityType, IContext context = null] { get; }
    }
}