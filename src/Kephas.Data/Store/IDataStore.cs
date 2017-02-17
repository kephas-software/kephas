// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStore.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataStore interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Store
{
    using System;

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
    }
}