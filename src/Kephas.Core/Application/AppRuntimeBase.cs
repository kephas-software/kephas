// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application runtime base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;

    /// <summary>
    /// An application application runtime providing only assemblies loaded by the runtime.
    /// </summary>
    public abstract class AppRuntimeBase : Expando, IAppRuntime, ILoggable
    {
        /// <summary>
        /// The application identifier key.
        /// </summary>
        public const string AppIdKey = "AppId";

        /// <summary>
        /// The application instance identifier key.
        /// </summary>
        public const string AppInstanceIdKey = "AppInstanceId";

        /// <summary>
        /// The application version key.
        /// </summary>
        public const string AppVersionKey = "AppVersion";

        /// <summary>
        /// A pattern specifying the assembly file search.
        /// </summary>
        protected const string AssemblyFileSearchPattern = "*.dll";

        /// <summary>
        /// The assembly file extension.
        /// </summary>
        protected const string AssemblyFileExtension = ".dll";

        private readonly string appLocation;

        private readonly ILogManager logManager;

        private readonly ConcurrentDictionary<object, IEnumerable<Assembly>> assemblyResolutionCache =
            new ConcurrentDictionary<object, IEnumerable<Assembly>>();

        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppRuntimeBase"/> class.
        /// </summary>
        /// <param name="assemblyLoader">Optional. The assembly loader.</param>
        /// <param name="logManager">Optional. The log manager.</param>
        /// <param name="defaultAssemblyFilter">Optional. A default filter applied when loading
        ///                                     assemblies.</param>
        /// <param name="appLocation">Optional. The application location. If not specified, the
        ///                           current application location is considered.</param>
        protected AppRuntimeBase(IAssemblyLoader assemblyLoader = null, ILogManager logManager = null, Func<AssemblyName, bool> defaultAssemblyFilter = null, string appLocation = null)
            : base(isThreadSafe: true)
        {
            this.logManager = logManager ?? new NullLogManager();
            this.AssemblyLoader = assemblyLoader ?? new DefaultAssemblyLoader();
            this.AssemblyFilter = defaultAssemblyFilter ?? (a => !a.IsSystemAssembly());
            this.appLocation = appLocation;

            var appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "App";
            this[AppIdKey] = appName;
            this[AppInstanceIdKey] = $"{appName}-{Guid.NewGuid():N}";
        }

        /// <summary>
        /// Gets the assembly loader.
        /// </summary>
        /// <value>
        /// The assembly loader.
        /// </value>
        public IAssemblyLoader AssemblyLoader { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public ILogger Logger
        {
            get => this.logger ?? (this.logger = this.GetLogger());
            protected internal set => this.logger = value;
        }

        /// <summary>
        /// Gets the assembly filter.
        /// </summary>
        /// <value>
        /// The assembly filter.
        /// </value>
        protected Func<AssemblyName, bool> AssemblyFilter { get; }

        /// <summary>
        /// Gets the application location (directory where the application lies).
        /// </summary>
        /// <returns>
        /// A path indicating the application location.
        /// </returns>
        public virtual string GetAppLocation()
        {
            if (!string.IsNullOrEmpty(this.appLocation))
            {
                return Path.GetFullPath(this.appLocation);
            }

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetLocation();
        }

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">Optional. A filter for the assemblies.</param>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        public virtual IEnumerable<Assembly> GetAppAssemblies(Func<AssemblyName, bool> assemblyFilter = null)
        {
            // TODO The assemblies from the current domain do not consider the not loaded
            // but required referenced assemblies. Therefore load all the references recursively.
            // This could be optimized somehow.
            assemblyFilter = assemblyFilter ?? this.AssemblyFilter;
            var assemblies = this.assemblyResolutionCache.GetOrAdd(
                (object)assemblyFilter ?? this,
                _ => this.ComputeAppAssemblies(assemblyFilter));

            return assemblies;
        }

        /// <summary>
        /// Gets host address.
        /// </summary>
        /// <returns>
        /// The host address.
        /// </returns>
        public virtual IPAddress GetHostAddress()
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            return hostEntry.AddressList.Last(add => add.AddressFamily == AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Gets the name of the host where the application process runs.
        /// </summary>
        /// <returns>
        /// The host name.
        /// </returns>
        public virtual string GetHostName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <returns>
        /// The logger.
        /// </returns>
        protected virtual ILogger GetLogger() => this.logManager.GetLogger(this.GetType());

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        /// <returns>
        /// The loaded assemblies.
        /// </returns>
        protected virtual IList<Assembly> GetLoadedAssemblies()
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
        protected virtual AssemblyName[] GetReferencedAssemblies(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies();
        }

        /// <summary>
        /// Computes the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">A filter for the assemblies.</param>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        protected virtual IEnumerable<Assembly> ComputeAppAssemblies(Func<AssemblyName, bool> assemblyFilter)
        {
            var loadedAssemblies = this.GetLoadedAssemblies();

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

            return assemblies;
        }

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
                this.Logger.Warn(ex, string.Format(Strings.AppRuntimeBase_CannotLoadAssembly_Exception, n));
                return null;
            }
        }
    }
}