// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStore.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataStore interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;

    using Kephas.Activation;

    /// <summary>
    /// Interface for data store.
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Gets the data store name.
        /// </summary>
        /// <value>
        /// The data store name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the data store kind.
        /// </summary>
        /// <value>
        /// The data store kind.
        /// </value>
        string Kind { get; }

        /// <summary>
        /// Gets the type of the data context.
        /// </summary>
        /// <value>
        /// The type of the data context.
        /// </value>
        Type DataContextType { get; }

        /// <summary>
        /// Gets the data context configuration.
        /// </summary>
        /// <value>
        /// The data context configuration.
        /// </value>
        IDataContextConfiguration DataContextConfiguration { get; }

        /// <summary>
        /// Gets the entity activator.
        /// </summary>
        /// <value>
        /// The entity activator.
        /// </value>
        IActivator EntityActivator { get; }
    }
}