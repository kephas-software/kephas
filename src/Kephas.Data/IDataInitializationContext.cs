// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataInitializationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataInitializationContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Activation;
    using Kephas.Data.Store;
    using Kephas.Services;

    /// <summary>
    /// Interface for data initialization context.
    /// </summary>
    public interface IDataInitializationContext : IDataOperationContext
    {
        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        IDataStore DataStore { get; }

        /// <summary>
        /// Gets a context for the initialization.
        /// </summary>
        /// <value>
        /// The initialization context.
        /// </value>
        IContext InitializationContext { get; }
    }
}
