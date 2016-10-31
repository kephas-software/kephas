// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetStandardAppEnvironment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the .NET Standard 1.5 application environment class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Reflection;

    /// <summary>
    /// A .NET Standard 1.5 application environment.
    /// </summary>
    public class NetStandardAppEnvironment : AppEnvironmentBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetStandardAppEnvironment"/> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader.</param>
        /// <param name="assemblyFilter">A filter for loaded assemblies.</param>
        public NetStandardAppEnvironment(IAssemblyLoader assemblyLoader = null, Func<AssemblyName, bool> assemblyFilter = null)
            : base(assemblyLoader, assemblyFilter)
        {
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
        /// Adds additional assemblies to the ones already collected.
        /// </summary>
        /// <param name="assemblies">The collected assemblies.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected override Task AddAdditionalAssembliesAsync(IList<Assembly> assemblies, CancellationToken cancellationToken)
        {
            // load all the assemblies found in the application directory which are not already loaded.
            var directory = this.GetAppLocation();
            var loadedAssemblyFiles = assemblies.Select(this.GetFileName).Select(f => f.ToLowerInvariant());
            var assemblyFiles = Directory.EnumerateFiles(directory, "*.dll", SearchOption.TopDirectoryOnly).Select(Path.GetFileName);
            var assemblyFilesToLoad = assemblyFiles.Where(f => !loadedAssemblyFiles.Contains(f.ToLowerInvariant()));
            var loadContext = AssemblyLoadContext.GetLoadContext(Assembly.GetEntryAssembly());
            assemblies.AddRange(assemblyFilesToLoad.Select(f => loadContext.LoadFromAssemblyPath(Path.Combine(directory, f))).Where(a => this.AssemblyFilter(a.GetName())));

            return Task.FromResult((IEnumerable<Assembly>)assemblies);
        }

        /// <summary>
        /// Gets the assembly location.
        /// </summary>
        /// <returns>
        /// A path.
        /// </returns>
        private string GetAppLocation()
        {
            return System.AppContext.BaseDirectory;
        }

        /// <summary>
        /// Gets file name.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The file name.
        /// </returns>
        private string GetFileName(Assembly assembly)
        {
            var codebaseUri = new Uri(assembly.CodeBase);
            return Path.GetFileName(Uri.UnescapeDataString(codebaseUri.AbsolutePath));
        }
    }
}