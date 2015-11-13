// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Net45HostingEnvironment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The hosting environment for .NET 4.5 applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Hosting.Net45
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// The hosting environment for .NET 4.5 applications.
    /// </summary>
    public class Net45HostingEnvironment : Expando, IHostingEnvironment
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Net45HostingEnvironment" /> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public Net45HostingEnvironment(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.logger = logManager.GetLogger<Net45HostingEnvironment>();
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