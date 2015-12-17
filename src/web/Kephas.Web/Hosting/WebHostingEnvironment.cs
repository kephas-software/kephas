// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebHostingEnvironment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The hosting environment for .NET 4.5 applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.Hosting
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Hosting;
    using Kephas.Logging;

    /// <summary>
    /// The hosting environment for .NET 4.5 applications.
    /// </summary>
    public class WebHostingEnvironment : Expando, IHostingEnvironment
    {
        /// <summary>
        /// The application environment.
        /// </summary>
        private readonly IApplicationEnvironment applicationEnvironment;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHostingEnvironment" /> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        /// <param name="applicationEnvironment">The application environment.</param>
        public WebHostingEnvironment(ILogManager logManager, IApplicationEnvironment applicationEnvironment)
        {
            // TODO support code contracts
            // Contract.Requires(logManager != null);
            // Contract.Requires(logManager != null);

            this.logger = logManager.GetLogger<WebHostingEnvironment>();
            this.applicationEnvironment = applicationEnvironment;
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
                   where (file.FileType == ".dll" || file.FileType == ".exe")
                   select new AssemblyName { Name = file.Name }
                    into name
                   where !name.IsSystemAssembly()
                   select Assembly.Load(name);
        }
    }
}