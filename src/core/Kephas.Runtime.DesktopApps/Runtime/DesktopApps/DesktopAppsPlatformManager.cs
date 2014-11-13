// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesktopAppsPlatformManager.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The platform manager for .NET 4.5 or higher.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Runtime.DesktopApps
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using System.Threading.Tasks;

    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// The .NET 4.5 or higher platform.
    /// </summary>
    public class DesktopAppsPlatformManager : IPlatformManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DesktopAppsPlatformManager" /> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public DesktopAppsPlatformManager(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.logger = logManager.GetLogger(typeof(DesktopAppsPlatformManager));
        }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        public Task<IEnumerable<Assembly>> GetAppAssembliesAsync()
        {
            return Task.FromResult((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}