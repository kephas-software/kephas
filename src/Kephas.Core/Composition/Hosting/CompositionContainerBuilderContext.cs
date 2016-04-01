// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositionContainerBuilderContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the composition container builder context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using System.Diagnostics.Contracts;

    using Kephas.Configuration;
    using Kephas.Hosting;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// A composition container builder context.
    /// </summary>
    public class CompositionContainerBuilderContext : ContextBase, ICompositionContainerBuilderContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionContainerBuilderContext"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        /// <param name="hostingEnvironment">The hosting environment.</param>
        public CompositionContainerBuilderContext(ILogManager logManager, IConfigurationManager configurationManager, IHostingEnvironment hostingEnvironment)
        {
            Contract.Requires(logManager != null);
            Contract.Requires(configurationManager != null);
            Contract.Requires(hostingEnvironment != null);

            this.LogManager = logManager;
            this.ConfigurationManager = configurationManager;
            this.HostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        public ILogManager LogManager { get; }

        /// <summary>
        /// Gets the configuration manager.
        /// </summary>
        /// <value>
        /// The configuration manager.
        /// </value>
        public IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        public IHostingEnvironment HostingEnvironment { get; }
    }
}