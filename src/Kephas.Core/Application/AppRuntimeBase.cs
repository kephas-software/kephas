// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeBase.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the application runtime base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Generic;
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
        /// Initializes a new instance of the <see cref="AppRuntimeBase"/> class.
        /// </summary>
        /// <param name="assemblyLoader">The assembly loader.</param>
        /// <param name="assemblyFilter">A filter for loaded assemblies.</param>
        protected AppRuntimeBase(IAssemblyLoader assemblyLoader = null, Func<AssemblyName, bool> assemblyFilter = null)
            : base(isThreadSafe: true)
        {
            this.AssemblyLoader = assemblyLoader ?? new DefaultAssemblyLoader();
            this.AssemblyFilter = assemblyFilter ?? (a => !a.IsSystemAssembly());
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
        /// <param name="assemblyFilter">(Optional) A filter for the assemblies.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        public virtual async Task<IEnumerable<Assembly>> GetAppAssembliesAsync(Func<AssemblyName, bool> assemblyFilter = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO The assemblies from the current domain do not consider the not loaded
            // but required referenced assemblies. Therefore load all the references recursively.
            // This could be optimized somehow.
            var assemblies = this.GetLoadedAssemblies();

            assemblyFilter = assemblyFilter ?? this.AssemblyFilter;
            var loadedAssemblyRefs = new HashSet<string>(assemblies.Select(a => a.GetName().FullName));
            var assembliesToCheck = assemblies.Where(a => assemblyFilter(a.GetName())).ToList();

            while (assembliesToCheck.Count > 0)
            {
                var assemblyRefsToLoad = new HashSet<AssemblyName>();
                foreach (var assembly in assembliesToCheck)
                {
                    var referencesToLoad = this.GetReferencedAssemblies(assembly).Where(a => !loadedAssemblyRefs.Contains(a.FullName) && assemblyFilter(a));
                    assemblyRefsToLoad.AddRange(referencesToLoad);
                }

                loadedAssemblyRefs.AddRange(assemblyRefsToLoad.Select(an => an.FullName));
                assembliesToCheck = assemblyRefsToLoad.Select(this.AssemblyLoader.LoadAssembly).ToList();
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
            return TaskHelper.CompletedTask;
        }
    }
}