// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAssemblyLoader.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net assembly loader class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Reflection;

    using Kephas.Services;
#if NETSTANDARD2_0
    using System.Runtime.Loader;
#endif

    /// <summary>
    /// The default assembly loader.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class DefaultAssemblyLoader : IAssemblyLoader
    {
#if NETSTANDARD2_0
        /// <summary>
        /// The load context.
        /// </summary>
        private readonly AssemblyLoadContext loadContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAssemblyLoader"/> class.
        /// </summary>
        public DefaultAssemblyLoader()
        {
            this.loadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetEntryAssembly());
        }
#endif

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
#if NETSTANDARD2_0
            return this.loadContext.LoadFromAssemblyPath(assemblyFilePath);
#else
            return Assembly.LoadFile(assemblyFilePath);
#endif
        }
    }
}