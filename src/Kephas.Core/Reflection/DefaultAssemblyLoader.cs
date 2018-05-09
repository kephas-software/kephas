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
    using System;
    using System.IO;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAssemblyLoader"/> class.
        /// </summary>
        public DefaultAssemblyLoader()
        {
#if NETSTANDARD2_0
            AssemblyLoadContext.Default.Resolving += this.TryResolveAssembly;
#else
            AppDomain.CurrentDomain.AssemblyResolve += this.TryResolveAssembly;
#endif
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
#if NETSTANDARD2_0
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFilePath);
#else
            return Assembly.LoadFile(assemblyFilePath);
#endif
        }

#if NETSTANDARD2_0
        /// <summary>
        /// Tries to resolve the assembly.
        /// </summary>
        /// <param name="assemblyLoadContext">Context for the assembly load.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>
        /// The resolved assembly or <c>null</c>.
        /// </returns>
        protected virtual Assembly TryResolveAssembly(AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName)
        {
            return null;
        }
#else
        /// <summary>
        /// Tries to resolve the assembly.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="args">Arguments for resolving the assembly.</param>
        /// <returns>
        /// The resolved assembly or <c>null</c>.
        /// </returns>
        protected virtual Assembly TryResolveAssembly(object sender, ResolveEventArgs args)
        {
            var parentLocation = args.RequestingAssembly?.GetLocation();
            if (parentLocation == null)
            {
                return null;
            }

            var fileName = new AssemblyName(args.Name).Name + ".dll";
            var filePath = Path.Combine(parentLocation, fileName);
            return this.LoadAssemblyFromPath(filePath);
        }
#endif
    }
}