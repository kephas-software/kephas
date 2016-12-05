// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetStandardAssemblyLoader.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the net standard assembly loader class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Reflection
{
    using System.Reflection;
    using System.Runtime.Loader;

    /// <summary>
    /// The assembly loader for the .NET Standard platform.
    /// </summary>
    public class NetStandardAssemblyLoader : IAssemblyLoader
    {
        /// <summary>
        /// The load context.
        /// </summary>
        private readonly AssemblyLoadContext loadContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetStandardAssemblyLoader"/> class.
        /// </summary>
        public NetStandardAssemblyLoader()
        {
            this.loadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetEntryAssembly());
        }

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
            return this.loadContext.LoadFromAssemblyPath(assemblyFilePath);
        }
    }
}