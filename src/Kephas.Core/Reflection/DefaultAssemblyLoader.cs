// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAssemblyLoader.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net assembly loader class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#nullable enable

namespace Kephas.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Loader;

    using Kephas.Services;

    /// <summary>
    /// The default assembly loader.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAssemblyLoader : IAssemblyLoader
    {
        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>
        /// The loaded assemblies.
        /// </returns>
        public IEnumerable<Assembly> GetAssemblies()
        {
            // TODO AssemblyLoadContext.Default.Assemblies;
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        public Assembly LoadAssemblyFromName(AssemblyName assemblyName)
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
        }

        /// <summary>
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyFilePath">The file path of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        public Assembly LoadAssemblyFromPath(string assemblyFilePath)
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFilePath);
        }
    }
}