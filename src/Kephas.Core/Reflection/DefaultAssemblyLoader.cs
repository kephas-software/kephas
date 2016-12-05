// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAssemblyLoader.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the default assembly loader class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System;
    using System.Reflection;

    using Kephas.Services;

    /// <summary>
    /// A default assembly loader.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAssemblyLoader : IAssemblyLoader
    {
        /// <summary>
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        public Assembly LoadAssembly(AssemblyName assemblyName)
        {
            return Assembly.Load(assemblyName);
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
            throw new NotSupportedException("Please use a more specific assembly loader, which can load from a file path, like Net46AssemblyLoader or NetStandardAssemblyLoader.");
        }
    }
}