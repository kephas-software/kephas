// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppRuntime.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net application runtime class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// The default application runtime.
    /// </summary>
    public class DefaultAppRuntime : AppRuntimeBase
    {
        /// <summary>
        /// The application location.
        /// </summary>
        private readonly string appLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAppRuntime"/> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader (optional).</param>
        /// <param name="logManager">The log manager (optional).</param>
        /// <param name="assemblyFilter">A filter for loaded assemblies (optional).</param>
        /// <param name="appLocation">
        /// The application location (optional). If not specified, the current application location is considered.
        /// </param>
        public DefaultAppRuntime(IAssemblyLoader assemblyLoader = null, ILogManager logManager = null, Func<AssemblyName, bool> assemblyFilter = null, string appLocation = null)
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

#if NETSTANDARD2_0
            return System.AppContext.BaseDirectory;
#else
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetLocation();
#endif
        }

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>
        /// The loaded assemblies.
        /// </returns>
        protected override IList<Assembly> GetLoadedAssemblies()
        {
#if NETSTANDARD2_0
            return new List<Assembly> { Assembly.GetEntryAssembly() };
#else
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
#endif
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
            return Path.GetFileName(assembly.GetFilePath());
        }

#if NETSTANDARD2_0
#else
        /// <summary>
        /// Enumerates the files in the provided directory.
        /// </summary>
        /// <param name="directory">Pathname of the directory.</param>
        /// <param name="filePattern">A pattern specifying the files to retrieve.</param>
        /// <returns>
        /// An enumeration of file names.
        /// </returns>
        protected override IEnumerable<string> EnumerateFiles(string directory, string filePattern)
        {
            return Directory.EnumerateFiles(directory, filePattern, SearchOption.TopDirectoryOnly);
        }
#endif
    }
}