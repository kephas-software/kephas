// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICompositionContainerBuilderContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the ICompositionContainerBuilderContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Composition.Hosting
{
    using Kephas.Configuration;
    using Kephas.Hosting;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// Context for the composition container builder.
    /// </summary>
    public interface ICompositionContainerBuilderContext : IContext
    {
        /// <summary>
        /// Gets the log manager.
        /// </summary>
        /// <value>
        /// The log manager.
        /// </value>
        ILogManager LogManager { get; }

        /// <summary>
        /// Gets the configuration manager.
        /// </summary>
        /// <value>
        /// The configuration manager.
        /// </value>
        IConfigurationManager ConfigurationManager { get; }

        /// <summary>
        /// Gets the hosting environment.
        /// </summary>
        /// <value>
        /// The hosting environment.
        /// </value>
        IHostingEnvironment HostingEnvironment { get; }
    }
}