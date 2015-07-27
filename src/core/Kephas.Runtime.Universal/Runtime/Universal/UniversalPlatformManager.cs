// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniversalPlatformManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Platform manager for Windows Store.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.Universal
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Logging;

    /// <summary>
    /// Platform manager for Universal Windows Platform.
    /// </summary>
    public class UniversalPlatformManager : IPlatformManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalPlatformManager"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public UniversalPlatformManager(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.logger = logManager.GetLogger<UniversalPlatformManager>();
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

            return (from file in await folder.GetFilesAsync()
                    where file.FileType == ".dll" || file.FileType == ".exe"
                    select new AssemblyName { Name = file.Name }
                    into name
                    select Assembly.Load(name)).ToList();
        }
    }
}