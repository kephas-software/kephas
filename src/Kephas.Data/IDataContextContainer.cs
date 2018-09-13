// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextContainer.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataContextContainer interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Collections.Generic;

    using Kephas.Dynamic;

    /// <summary>
    /// Interface for data context container.
    /// </summary>
    public interface IDataContextContainer : IExpando, IReadOnlyCollection<IDataContext>, IDisposable
    {
        /// <summary>
        /// Gets the data context for the provided entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <returns>
        /// The data context.
        /// </returns>
        IDataContext this[Type entityType] { get; }
    }
}