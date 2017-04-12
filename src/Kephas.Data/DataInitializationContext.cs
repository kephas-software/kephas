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
    using Kephas.Diagnostics.Contracts;
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
        /// <param name="configuration">The configuration.</param>
        /// <param name="initializationContext">The initialization context (optional).</param>
        public DataInitializationContext(
            IDataContext dataContext,
            IDataContextConfiguration configuration,
            IContext initializationContext = null)
            : base(dataContext)
        {
            Requires.NotNull(configuration, nameof(configuration));

            this.Configuration = configuration;
            this.InitializationContext = initializationContext;
        }

        /// <summary>
        /// Gets the data context configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IDataContextConfiguration Configuration { get; }

        /// <summary>
        /// Gets a context for the initialization.
        /// </summary>
        /// <value>
        /// The initialization context.
        /// </value>
        public IContext InitializationContext { get; }
    }
}