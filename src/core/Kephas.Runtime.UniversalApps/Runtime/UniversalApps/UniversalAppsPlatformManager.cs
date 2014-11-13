// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UniversalAppsPlatformManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Platform manager for Windows Store.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.UniversalApps
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// Platform manager for Windows Store.
    /// </summary>
    public class UniversalAppsPlatformManager : IPlatformManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalAppsPlatformManager"/> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public UniversalAppsPlatformManager(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.logger = logManager.GetLogger(typeof(UniversalAppsPlatformManager));
        }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        public async Task<IEnumerable<Assembly>> GetAppAssembliesAsync()
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            var assemblies = new List<Assembly>();
            foreach (var file in await folder.GetFilesAsync())
            {
                if (file.FileType == ".dll" || file.FileType == ".exe")
                {
                    var name = new AssemblyName { Name = file.Name };
                    var asm = Assembly.Load(name);
                    assemblies.Add(asm);
                }
            }

            return assemblies;
        }
    }
}