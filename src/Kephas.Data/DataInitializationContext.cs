// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataInitializationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// <param name="initializationContext">The initialization context (optional).</param>
        public DataInitializationContext(
            IDataContext dataContext,
            IDataStore dataStore,
            IContext initializationContext = null)
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
        public IContext InitializationContext { get; }
    }
}