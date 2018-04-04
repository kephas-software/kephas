// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetAppRuntime.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the .NET Standard 1.5 application runtime class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// A .NET Standard 1.5 application runtime.
    /// </summary>
    public class NetAppRuntime : AppRuntimeBase
    {
        /// <summary>
        /// The application location.
        /// </summary>
        private readonly string appLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetAppRuntime"/> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader (optional).</param>
        /// <param name="logManager">The log manager (optional).</param>
        /// <param name="assemblyFilter">A filter for loaded assemblies (optional).</param>
        /// <param name="appLocation">
        /// The application location (optional). If not specified, the current application location is considered.
        /// </param>
        public NetAppRuntime(IAssemblyLoader assemblyLoader = null, ILogManager logManager = null, Func<AssemblyName, bool> assemblyFilter = null, string appLocation = null)
            : base(assemblyLoader, logManager, assemblyFilter)
        {
            this.appLocation = appLocation;
        }

        /// <summary>
        /// Gets the assembly location.
        /// </summary>
        /// <returns>
        /// A path.
        /// </returns>
        public override string GetAppLocation()
        {
            if (!string.IsNullOrEmpty(this.appLocation))
            {
                return Path.GetFullPath(this.appLocation);
            }

            return System.AppContext.BaseDirectory;
        }

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>
        /// The loaded assemblies.
        /// </returns>
        protected override IList<Assembly> GetLoadedAssemblies()
        {
            return new List<Assembly> { Assembly.GetEntryAssembly() };
        }

        /// <summary>
        /// Gets the referenced assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// An array of assembly name.
        /// </returns>
        protected override AssemblyName[] GetReferencedAssemblies(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies();
        }

        /// <summary>
        /// Gets file name.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The file name.
        /// </returns>
        protected override string GetFileName(Assembly assembly)
        {
            var codebaseUri = new Uri(assembly.CodeBase);
            return Path.GetFileName(Uri.UnescapeDataString(codebaseUri.AbsolutePath));
        }
    }
}