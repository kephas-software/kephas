// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowsPlatformHostingEnvironment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Hosting environment for Universal Windows Platform.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Hosting.WindowsPlatform
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// Hosting environment for Universal Windows Platform.
    /// </summary>
    public class WindowsPlatformHostingEnvironment : Expando, IHostingEnvironment
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsPlatformHostingEnvironment"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public WindowsPlatformHostingEnvironment(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.logger = logManager.GetLogger<WindowsPlatformHostingEnvironment>();
        }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        public async Task<IEnumerable<Assembly>> GetAppAssembliesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            return from file in await folder.GetFilesAsync()
                    where file.FileType == ".dll" || file.FileType == ".exe"
                    select new AssemblyName { Name = file.Name }
                    into name
                    select Assembly.Load(name);
        }
    }
}