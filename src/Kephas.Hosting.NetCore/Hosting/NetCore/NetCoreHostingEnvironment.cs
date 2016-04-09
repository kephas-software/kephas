// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Net45HostingEnvironment.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The hosting environment for .NET 4.5 applications.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Hosting.NetCore.Hosting.NetCore
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Hosting;
    using Kephas.Logging;
    using Kephas.Reflection;

    /// <summary>
    /// The hosting environment for .NET 4.5 applications.
    /// </summary>
    public class NetCoreHostingEnvironment : Expando, IHostingEnvironment
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetCoreHostingEnvironment" /> class.
        /// </summary>
        /// <param name="logManager">The log manager.</param>
        public NetCoreHostingEnvironment(ILogManager logManager)
        {
            Contract.Requires(logManager != null);

            this.logger = logManager.GetLogger<NetCoreHostingEnvironment>();
        }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A promise of an enumeration of application assemblies.
        /// </returns>
        public Task<IEnumerable<Assembly>> GetAppAssembliesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO The assemblies from the current domain do not consider the not loaded
            // but required referenced assemblies. Therefore load all the references recursively.
            // This could be optimized somehow.
            var assemblies = this.GetLoadedAssemblies();

            var loadedAssemblyRefs = new HashSet<string>(assemblies.Select(a => a.GetName().FullName));
            var assembliesToCheck = assemblies.Where(a => !a.IsSystemAssembly()).ToList();
            while (assembliesToCheck.Count > 0)
            {
                var assemblyRefsToLoad = new HashSet<AssemblyName>();
                foreach (var assembly in assembliesToCheck)
                {
                    var referencesToLoad = this.GetReferencedAssemblies(assembly).Where(a => !loadedAssemblyRefs.Contains(a.FullName) && !a.IsSystemAssembly());
                    assemblyRefsToLoad.AddRange(referencesToLoad);
                }

                loadedAssemblyRefs.AddRange(assemblyRefsToLoad.Select(an => an.FullName));
                assembliesToCheck = assemblyRefsToLoad.Select(Assembly.Load).ToList();
                assemblies.AddRange(assembliesToCheck);
            }

            // TODO...
            // load all the assemblies found in the application directory which are not already loaded.
            ////var directory = this.GetAppLocation();
            ////var loadedAssemblyFiles = assemblies.Select(this.GetFileName).Select(f => f.ToLowerInvariant());
            ////var assemblyFiles = Directory.EnumerateFiles(directory, "*.dll", SearchOption.TopDirectoryOnly).Select(Path.GetFileName);
            ////var assemblyFilesToLoad = assemblyFiles.Where(f => !loadedAssemblyFiles.Contains(f.ToLowerInvariant()));
            ////assemblies.AddRange(assemblyFilesToLoad.Select(f => Assembly.LoadFile(Path.Combine(directory, f))).Where(a => !a.IsSystemAssembly()));

            return Task.FromResult((IEnumerable<Assembly>)assemblies);
        }

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>
        /// The loaded assemblies.
        /// </returns>
        private IList<Assembly> GetLoadedAssemblies()
        {
            // TODO use something better than reflection
            var currentdomain = typeof(string).GetTypeInfo().Assembly.GetType("System.AppDomain").GetRuntimeProperty("CurrentDomain").GetMethod.Invoke(null, new object[] { });
            var getassemblies = currentdomain.GetType().GetRuntimeMethod("GetAssemblies", new Type[] { });
            var assemblies = ((Assembly[])getassemblies.Invoke(currentdomain, new object[] { })).ToList();
            return assemblies;
        }

        /// <summary>
        /// Gets the referenced assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// An array of assembly name.
        /// </returns>
        private AssemblyName[] GetReferencedAssemblies(Assembly assembly)
        {
            // TODO use something better than reflection
            var getReferencedAssemblies = assembly.GetType().GetRuntimeMethod("GetReferencedAssemblies", new Type[] {});
            var refAssemblies = (AssemblyName[])getReferencedAssemblies.Invoke(assembly, new object[] { });
            return refAssemblies;
        }

        /// <summary>
        /// Gets the assembly location.
        /// </summary>
        /// <returns>
        /// A path.
        /// </returns>
        private string GetAppLocation()
        {
            ////var assembly = Assembly.GetEntryAssembly();

            ////var codebaseUri = new Uri(assembly.CodeBase);
            ////var location = Path.GetDirectoryName(Uri.UnescapeDataString(codebaseUri.AbsolutePath));
            ////return location;
            
            // TODO find an implementation
            return string.Empty;
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
            ////var codebaseUri = new Uri(assembly.CodeBase);

            // TODO find an implementation
            var codebaseUri = new Uri("dummy:dummy");
            return Path.GetFileName(Uri.UnescapeDataString(codebaseUri.AbsolutePath));
        }
    }
}