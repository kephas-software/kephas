// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesktopHostingEnvironment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The platform manager for desktop applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Hosting.Desktop
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// The platform manager for desktop applications.
    /// </summary>
    public class DesktopHostingEnvironment : IHostingEnvironment
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopHostingEnvironment" /> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public DesktopHostingEnvironment(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.logger = logManager.GetLogger<DesktopHostingEnvironment>();
        }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        public Task<IEnumerable<Assembly>> GetAppAssembliesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}