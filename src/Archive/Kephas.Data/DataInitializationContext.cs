// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataInitializationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data initialization context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Data.Store;
    using Kephas.Services;

    /// <summary>
    /// A data initialization context.
    /// </summary>
    public class DataInitializationContext : DataOperationContext, IDataInitializationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataInitializationContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="dataStore">The data store.</param>
        /// <param name="initializationContext">Optional. The initialization context.</param>
        public DataInitializationContext(
            IDataContext dataContext,
            IDataStore dataStore,
            IContext? initializationContext = null)
            : base(dataContext)
        {
            this.DataStore = dataStore;
            this.InitializationContext = initializationContext;
        }

        /// <summary>
        /// Gets the data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public IDataStore DataStore { get; }

        /// <summary>
        /// Gets a context for the initialization.
        /// </summary>
        /// <value>
        /// The initialization context.
        /// </value>
        public IContext? InitializationContext { get; }
    }
}