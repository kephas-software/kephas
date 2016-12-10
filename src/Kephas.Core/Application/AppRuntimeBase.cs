// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application runtime base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Logging;
using Kephas.Resources;

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Base class for the application runtime service.
    /// </summary>
    public abstract class AppRuntimeBase : Expando, IAppRuntime
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// A pattern specifying the assembly file search.
        /// </summary>
        protected const string AssemblyFileSearchPattern = "*.dll";

        /// <summary>
        /// The assembly file extension.
        /// </summary>
        protected const string AssemblyFileExtension = ".dll";

        /// <summary>
        /// Initializes a new instance of the <see cref="AppRuntimeBase"/> class.
        /// </summary>
        /// <param name="assemblyLoader">(Optional) The assembly loader.</param>
        /// <param name="logManager">(Optional) The log manager.</param>
        /// <param name="assemblyFilter">(Optional) A filter for loaded assemblies.</param>
        protected AppRuntimeBase(IAssemblyLoader assemblyLoader = null, ILogManager logManager = null, Func<AssemblyName, bool> assemblyFilter = null)
            : base(isThreadSafe: true)
        {
            this.AssemblyLoader = assemblyLoader ?? new DefaultAssemblyLoader();
            this.AssemblyFilter = assemblyFilter ?? (a => !a.IsSystemAssembly());
            this.logger = logManager?.GetLogger<AppRuntimeBase>();
        }

        /// <summary>
        /// Gets the assembly loader.
        /// </summary>
        /// <value>
        /// The assembly loader.
        /// </value>
        public IAssemblyLoader AssemblyLoader { get; }

        /// <summary>
        /// Gets the assembly filter.
        /// </summary>
        /// <value>
        /// The assembly filter.
        /// </value>
        protected Func<AssemblyName, bool> AssemblyFilter { get; }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">A filter for the assemblies (optional).</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        public virtual async Task<IEnumerable<Assembly>> GetAppAssembliesAsync(Func<AssemblyName, bool> assemblyFilter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO The assemblies from the current domain do not consider the not loaded
            // but required referenced assemblies. Therefore load all the references recursively.
            // This could be optimized somehow.
            var loadedAssemblies = this.GetLoadedAssemblies();
            assemblyFilter = assemblyFilter ?? this.AssemblyFilter;

            // when computing the assemblies, use the Name and not the FullName
            // because for some obscure reasons it is possible to have the same
            // assembly with different versions loaded.
            // TODO log when such cases occur.
            var assemblies = loadedAssemblies.Where(a => assemblyFilter(a.GetName())).ToList();
            var loadedAssemblyRefs = new HashSet<string>(loadedAssemblies.Select(a => a.GetName().Name));
            var assembliesToCheck = new List<Assembly>(assemblies);

            while (assembliesToCheck.Count > 0)
            {
                var assemblyRefsToLoad = new HashSet<AssemblyName>();
                foreach (var assembly in assembliesToCheck)
                {
                    var referencesToLoad =
                        this.GetReferencedAssemblies(assembly)
                            .Where(a => !loadedAssemblyRefs.Contains(a.Name) && assemblyFilter(a))
                            .ToList();
                    loadedAssemblyRefs.AddRange(referencesToLoad.Select(an => an.Name));
                    assemblyRefsToLoad.AddRange(referencesToLoad);
                }

                assembliesToCheck = assemblyRefsToLoad.Select(this.TryLoadAssembly).Where(a => a != null).ToList();
                assemblies.AddRange(assembliesToCheck);
            }

            await this.AddAdditionalAssembliesAsync(assemblies, assemblyFilter, cancellationToken).PreserveThreadContext();
            return assemblies;
        }

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>
        /// The loaded assemblies.
        /// </returns>
        protected abstract IList<Assembly> GetLoadedAssemblies();

        /// <summary>
        /// Gets the referenced assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// An array of assembly name.
        /// </returns>
        protected abstract AssemblyName[] GetReferencedAssemblies(Assembly assembly);

        /// <summary>
        /// Adds additional assemblies to the ones already collected.
        /// </summary>
        /// <param name="assemblies">The collected assemblies.</param>
        /// <param name="assemblyFilter">A filter for the assemblies.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A Task.
        /// </returns>
        protected virtual Task AddAdditionalAssembliesAsync(IList<Assembly> assemblies, Func<AssemblyName, bool> assemblyFilter, CancellationToken cancellationToken)
        {
            // load all the assemblies found in the application directory which are not already loaded.
            var directory = this.GetAppLocation();
            var loadedAssemblyFiles = assemblies.Select(this.GetFileName).Select(f => f.ToLowerInvariant());
            var assemblyFiles = Directory.EnumerateFiles(directory, AssemblyFileSearchPattern, SearchOption.TopDirectoryOnly).Select(Path.GetFileName);
            var assemblyFilesToLoad = assemblyFiles
                                        .Where(f => !loadedAssemblyFiles.Contains(f.ToLowerInvariant()))
                                        .Where(f => assemblyFilter(this.GetAssemblyNameFromAssemblyFileName(f)));
            assemblies.AddRange(assemblyFilesToLoad
                                    .Select(f => this.AssemblyLoader.LoadAssemblyFromPath(Path.Combine(directory, f)))
                                    .Where(a => assemblyFilter(a.GetName())));

            return Task.FromResult((IEnumerable<Assembly>)assemblies);
        }

        /// <summary>
        /// Gets the assembly name from the assembly file name.
        /// </summary>
        /// <param name="f">The format string.</param>
        /// <returns>
        /// The assembly name.
        /// </returns>
        protected AssemblyName GetAssemblyNameFromAssemblyFileName(string f)
        {
            return new AssemblyName(f.Substring(0, f.Length - AssemblyFileExtension.Length));
        }

        /// <summary>
        /// Gets the application location.
        /// </summary>
        /// <returns>
        /// A path indicating the application location.
        /// </returns>
        protected abstract string GetAppLocation();

        /// <summary>
        /// Gets the file name of the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The assembly file name.
        /// </returns>
        protected abstract string GetFileName(Assembly assembly);

        /// <summary>
        /// Tries to load the assembly.
        /// </summary>
        /// <param name="n">The name of the assembly to load.</param>
        /// <returns>
        /// An assembly or <c>null</c>.
        /// </returns>
        private Assembly TryLoadAssembly(AssemblyName n)
        {
            try
            {
                return this.AssemblyLoader.LoadAssembly(n);
            }
            catch (Exception ex)
            {
                this.logger.Warn(ex, string.Format(Strings.AppRuntimeBase_CannotLoadAssembly_Exception, n));
                return null;
            }
        }
    }
}