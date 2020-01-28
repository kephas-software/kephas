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
    using System.Runtime.Versioning;

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
        public const string AppIdentityKey = "AppIdentity";

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
        /// <param name="appLocation">Optional. The application location. If not specified, the current
        ///                           application location is considered.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        protected AppRuntimeBase(
            IAssemblyLoader assemblyLoader = null,
            ILogManager logManager = null,
            Func<AssemblyName, bool> defaultAssemblyFilter = null,
            string appLocation = null,
            string appId = null,
            string appInstanceId = null,
            string appVersion = null,
            IExpando appArgs = null)
            : base(isThreadSafe: true)
        {
            this.logManager = logManager ?? new NullLogManager();
            this.AssemblyLoader = assemblyLoader ?? new DefaultAssemblyLoader();
            this.AssemblyFilter = defaultAssemblyFilter ?? (a => !a.IsSystemAssembly());
            this.appLocation = appLocation;

            this.InitializeAppProperties(Assembly.GetEntryAssembly(), appId, appInstanceId, appVersion);

            this[AppIdentityKey] = new AppIdentity(this[AppIdKey] as string, this[AppVersionKey] as string);

            AppDomain.CurrentDomain.AssemblyResolve += (s, e) => this.HandleAssemblyResolve(s as AppDomain ?? AppDomain.CurrentDomain, e);
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
        /// Gets the application's underlying .NET framework identifier.
        /// </summary>
        /// <returns>
        /// The application's underlying .NET framework identifier.
        /// </returns>
        public virtual string GetAppFramework()
        {
            var fwkName = this.GetAppFrameworkName();
            var fwkId = fwkName.Identifier == ".NETFramework"
                ? "net"
                : fwkName.Identifier == ".NETStandard"
                    ? "netstandard"
                    : fwkName.Identifier == ".NETCoreApp"
                        ? "netcoreapp"
                        : "net";
            var build = fwkName.Version.Build <= 0 ? string.Empty : fwkName.Version.Build.ToString();
            var fwkVersion = fwkId == "net"
                ? $"{fwkName.Version.Major}{fwkName.Version.Minor}{build}"
                : $"{fwkName.Version.Major}.{fwkName.Version.Minor}";
            return fwkId + fwkVersion;
        }

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
        /// Gets the application bin folders from where application is loaded.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the application bin folders in this
        /// collection.
        /// </returns>
        public virtual IEnumerable<string> GetAppBinDirectories()
        {
            yield return this.GetAppLocation();
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
        /// Initializes the application properties: AppId, AppInstanceId, and AppVersion.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly.</param>
        /// <param name="appId">Identifier for the application.</param>
        /// <param name="appInstanceId">Identifier for the application instance.</param>
        /// <param name="appVersion">The application version.</param>
        protected virtual void InitializeAppProperties(Assembly entryAssembly, string appId, string appInstanceId, string appVersion)
        {
            this[AppIdKey] = appId = this.GetAppId(entryAssembly, appId);
            this[AppInstanceIdKey] = string.IsNullOrEmpty(appInstanceId) ? $"{appId}-{Guid.NewGuid():N}" : appInstanceId;
            this[AppVersionKey] = string.IsNullOrEmpty(appVersion) ? (entryAssembly?.GetName().Version.ToString() ?? "0.0.0.0") : appVersion;
        }

        /// <summary>
        /// Handles the assembly resolve event.
        /// </summary>
        /// <param name="appDomain">The application domain.</param>
        /// <param name="args">Resolve event information.</param>
        /// <returns>
        /// The resolved assembly -or- <c>null</c>.
        /// </returns>
        protected virtual Assembly HandleAssemblyResolve(AppDomain appDomain, ResolveEventArgs args)
        {
            var assemblyFullName = args.Name;
            if (!this.IsCodeAssembly(assemblyFullName))
            {
                return null;
            }

            var appAssemblies = appDomain.GetAssemblies();
            var assembly = appAssemblies.FirstOrDefault(a => a.FullName == assemblyFullName);
            if (assembly == null)
            {
                var assemblyName = new AssemblyName(args.Name);
                var name = assemblyName.Name;
                var version = assemblyName.Version;
                var publicKeyToken = assemblyName.GetPublicKeyToken();
                assembly = appAssemblies.FirstOrDefault(a => this.IsAssemblyMatch(a.GetName(), name, version, publicKeyToken));
                if (assembly != null)
                {
                    this.Logger.Warn("Assembly '{assembly}' requested by '{requestingAssembly}' was resolved using {resolvedAssembly}", assemblyFullName, args.RequestingAssembly, assembly);
                }
            }

            return assembly;
        }

        /// <summary>
        /// Query if the assembly name matches the provided values.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="name">The name.</param>
        /// <param name="version">The version.</param>
        /// <param name="publicKeyToken">The public key token.</param>
        /// <returns>
        /// True if assembly match, false if not.
        /// </returns>
        protected virtual bool IsAssemblyMatch(AssemblyName assemblyName, string name, Version version, byte[] publicKeyToken)
        {
            return assemblyName.Name == name && EqualArray(assemblyName.GetPublicKeyToken(), publicKeyToken);
        }

        /// <summary>
        /// Query if 'assemblyName' is a code assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>
        /// True if code assembly, false if not.
        /// </returns>
        protected virtual bool IsCodeAssembly(string assemblyName)
        {
            return !assemblyName.Contains(".resources,");
        }

        /// <summary>
        /// Gets the application's underlying framework name.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <returns>
        /// The application framework name.
        /// </returns>
        protected virtual FrameworkName GetAppFrameworkName()
        {
            var assembly = Assembly.GetEntryAssembly();
            var targetFramework = assembly?.GetCustomAttribute<TargetFrameworkAttribute>();
            if (targetFramework == null)
            {
                assembly = Assembly.GetExecutingAssembly();
                targetFramework = assembly.GetCustomAttribute<TargetFrameworkAttribute>();
                if (targetFramework == null)
                {
                    throw new InvalidOperationException($"Could not identify the current framework from {Assembly.GetEntryAssembly()} and {Assembly.GetExecutingAssembly()}.");
                }
            }

            return new FrameworkName(targetFramework.FrameworkName);
        }

        /// <summary>
        /// Gets the application identifier.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly.</param>
        /// <param name="appId">Identifier for the application.</param>
        /// <returns>
        /// The application identifier.
        /// </returns>
        protected virtual string GetAppId(Assembly entryAssembly, string appId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                return appId;
            }

            if (entryAssembly == null)
            {
                return "App";
            }

            var titleAttr = entryAssembly.GetCustomAttribute<AssemblyTitleAttribute>();
            if (titleAttr != null)
            {
                return titleAttr.Title;
            }

            var productAttr = entryAssembly.GetCustomAttribute<AssemblyProductAttribute>();
            if (productAttr != null)
            {
                return productAttr.Product;
            }

            return entryAssembly.GetName().Name;
        }

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

        private static bool EqualArray(byte[] s1, byte[] s2)
        {
            if (s1 == null)
            {
                return s2 == null;
            }

            if (s2 == null)
            {
                return false;
            }

            if (s1.Length != s2.Length)
            {
                return false;
            }

            for (var i = 0; i < s1.Length; i++)
            {
                if (s1[i] != s2[i])
                {
                    return false;
                }
            }

            return true;
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
                this.Logger.Warn(ex, Strings.AppRuntimeBase_CannotLoadAssembly_Exception, n);
                return null;
            }
        }
    }
}