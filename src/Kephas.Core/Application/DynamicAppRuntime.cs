﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicAppRuntime.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the dynamic application runtime class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Licensing;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// An application application runtime loading dynamically assemblies from the application localtion.
    /// </summary>
    public class DynamicAppRuntime : AppRuntimeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicAppRuntime"/> class.
        /// </summary>
        /// <param name="getLogger">Optional. The get logger delegate.</param>
        /// <param name="checkLicense">Optional. The check license delegate.</param>
        /// <param name="defaultAssemblyFilter">Optional. The default assembly filter.</param>
        /// <param name="appFolder">Optional. The application location.</param>
        /// <param name="configFolders">Optional. The configuration folders.</param>
        /// <param name="licenseFolders">Optional. The license folders.</param>
        /// <param name="isRoot">Optional. Indicates whether the application instance is the root.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        public DynamicAppRuntime(
            Func<string, ILogger>? getLogger = null,
            Func<AppIdentity, IContext?, ILicenseCheckResult>? checkLicense = null,
            Func<AssemblyName, bool>? defaultAssemblyFilter = null,
            string? appFolder = null,
            IEnumerable<string>? configFolders = null,
            IEnumerable<string>? licenseFolders = null,
            bool? isRoot = null,
            string? appId = null,
            string? appInstanceId = null,
            string? appVersion = null,
            IDynamic? appArgs = null)
            : base(getLogger, checkLicense, defaultAssemblyFilter, appFolder, configFolders, licenseFolders, isRoot, appId, appInstanceId, appVersion, appArgs)
        {
        }

        /// <summary>
        /// Computes the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">A filter for the assemblies.</param>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        protected override IEnumerable<Assembly> ComputeAppAssemblies(Func<AssemblyName, bool> assemblyFilter)
        {
            var assemblies = base.ComputeAppAssemblies(assemblyFilter).ToList();
            this.AddAdditionalAssemblies(assemblies, assemblyFilter);
            return assemblies;
        }

        /// <summary>
        /// Adds additional assemblies to the ones already collected.
        /// </summary>
        /// <param name="assemblies">The collected assemblies.</param>
        /// <param name="assemblyFilter">A filter for the assemblies.</param>
        protected virtual void AddAdditionalAssemblies(IList<Assembly> assemblies, Func<AssemblyName, bool> assemblyFilter)
        {
            // load all the assemblies found in the application directories which are not already loaded.
            var directories = this.GetAppBinLocations();
            foreach (var directory in directories.Where(d => !string.IsNullOrEmpty(d)))
            {
                var loadedAssemblyFiles = assemblies.Where(a => !a.IsDynamic).Select(this.GetFileName).Select(f => f.ToLowerInvariant());
                var assemblyFiles = this.EnumerateFiles(directory, AssemblyFileSearchPattern).Select(Path.GetFileName);
                var assemblyFilesToLoad = assemblyFiles
                                            .Where(f => !loadedAssemblyFiles.Contains(f.ToLowerInvariant()))
                                            .Where(f => assemblyFilter(this.GetAssemblyName(f)));
                assemblies.AddRange(assemblyFilesToLoad
                                        .Select(f => this.LoadAssemblyFromPath(Path.Combine(directory, f)))
                                        .Where(a => assemblyFilter(a.GetName())));
            }
        }

        /// <summary>
        /// Enumerates the files in the provided directory.
        /// </summary>
        /// <param name="directory">Pathname of the directory.</param>
        /// <param name="filePattern">A pattern specifying the files to retrieve.</param>
        /// <returns>
        /// An enumeration of file names.
        /// </returns>
        protected virtual IEnumerable<string> EnumerateFiles(string directory, string filePattern)
        {
            return Directory.EnumerateFiles(directory, filePattern, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Gets the file name of the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The assembly file name.
        /// </returns>
        protected virtual string GetFileName(Assembly assembly)
        {
            return Path.GetFileName(assembly.Location);
        }

        /// <summary>
        /// Gets the assembly name from the assembly file name.
        /// </summary>
        /// <param name="assemblyFileName">The assembly file name.</param>
        /// <returns>
        /// The assembly name.
        /// </returns>
        protected AssemblyName GetAssemblyName(string assemblyFileName)
        {
#if NETSTANDARD2_0
            return new AssemblyName(assemblyFileName.Substring(0, assemblyFileName.Length - AssemblyFileExtension.Length));
#else
            return new AssemblyName(assemblyFileName[..^AssemblyFileExtension.Length]);
#endif
        }
    }
}