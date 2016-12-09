// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetAppRuntime.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the .NET 4.6 application runtime class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Kephas.Reflection;

    /// <summary>
    /// A .NET 4.6 application runtime.
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
        /// <param name="assemblyLoader">(Optional) The assembly loader.</param>
        /// <param name="assemblyFilter">(Optional) A filter for loaded assemblies.</param>
        /// <param name="appLocation">(Optional) the application location. If not specified, the current application location is considered.</param>
        public NetAppRuntime(IAssemblyLoader assemblyLoader = null, Func<AssemblyName, bool> assemblyFilter = null, string appLocation = null)
            : base(assemblyLoader, assemblyFilter)
        {
            this.appLocation = appLocation;
        }

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>
        /// The loaded assemblies.
        /// </returns>
        protected override IList<Assembly> GetLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
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
        /// Gets the application location.
        /// </summary>
        /// <returns>
        /// A path indicating the application location.
        /// </returns>
        protected override string GetAppLocation()
        {
            if (!string.IsNullOrEmpty(this.appLocation))
            {
                return Path.GetFullPath(this.appLocation);
            }

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();

            var codebaseUri = new Uri(assembly.CodeBase);
            var location = Path.GetDirectoryName(Uri.UnescapeDataString(codebaseUri.AbsolutePath));
            return location;
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