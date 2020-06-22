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
    using System.Runtime.CompilerServices;
    using System.Runtime.Versioning;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.Licensing;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Transitions;

#if NET461
#else
    using System.Runtime.Loader;
#endif

    /// <summary>
    /// An application application runtime providing only assemblies loaded by the runtime.
    /// </summary>
    public abstract class AppRuntimeBase : Expando, IAppRuntime, IInitializable, IDisposable
    {
        /// <summary>
        /// The default configuration folder.
        /// </summary>
        public static readonly string DefaultConfigFolder = "Config";

        /// <summary>
        /// The default license folder.
        /// </summary>
        public static readonly string DefaultLicenseFolder = "Licenses";

        /// <summary>
        /// The application identifier key.
        /// </summary>
        public static readonly string AppIdentityKey = "AppIdentity";

        /// <summary>
        /// The application identifier key.
        /// </summary>
        public static readonly string AppIdKey = nameof(IAppArgs.AppId);

        /// <summary>
        /// The application instance identifier key.
        /// </summary>
        public static readonly string AppInstanceIdKey = nameof(IAppArgs.AppInstanceId);

        /// <summary>
        /// The application version key.
        /// </summary>
        public static readonly string AppVersionKey = "AppVersion";

        /// <summary>
        /// The root application identifier key.
        /// </summary>
        public static readonly string IsRootKey = "IsRoot";

        /// <summary>
        /// A pattern specifying the assembly file search.
        /// </summary>
        protected static readonly string AssemblyFileSearchPattern = "*.dll";

        /// <summary>
        /// The assembly file extension.
        /// </summary>
        protected static readonly string AssemblyFileExtension = ".dll";

        private readonly Func<string, ILogger> getLogger;
        private readonly ConcurrentDictionary<object, IEnumerable<Assembly>> assemblyResolutionCache =
            new ConcurrentDictionary<object, IEnumerable<Assembly>>();

        private string? appLocation;
        private string? appFolder;
        private string[]? configLocations;
        private string[]? licenseLocations;
        private IEnumerable<string>? configFolders;
        private IEnumerable<string>? licenseFolders;
        private bool isDisposed = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="AppRuntimeBase"/> class.
        /// </summary>
        /// <param name="getLogger">Optional. The get logger delegate.</param>
        /// <param name="checkLicense">Optional. The check license delegate.</param>
        /// <param name="defaultAssemblyFilter">Optional. A default filter applied when loading
        ///                                     assemblies.</param>
        /// <param name="appFolder">Optional. The application folder. If not specified, the current
        ///                           application location is considered.</param>
        /// <param name="configFolders">Optional. The configuration folders relative to the application
        ///                             location.</param>
        /// <param name="licenseFolders">Optional. The license folders relative to the application
        ///                              location.</param>
        /// <param name="isRoot">Optional. Indicates whether the application instance is the root.</param>
        /// <param name="appId">Optional. Identifier for the application.</param>
        /// <param name="appInstanceId">Optional. Identifier for the application instance.</param>
        /// <param name="appVersion">Optional. The application version.</param>
        /// <param name="appArgs">Optional. The application arguments.</param>
        protected AppRuntimeBase(
            Func<string, ILogger>? getLogger = null,
            Func<AppIdentity, IContext?, ILicenseCheckResult>? checkLicense = null,
            Func<AssemblyName, bool>? defaultAssemblyFilter = null,
            string? appFolder = null,
            IEnumerable<string>? configFolders = null,
            IEnumerable<string>? licenseFolders = null,
            bool? isRoot = null,
            string? appId = null,
            string? appInstanceId = null,
            string? appVersion = null,
            IExpando? appArgs = null)
            : base(isThreadSafe: true)
        {
            this.AppArgs = appArgs == null
                ? new AppArgs()
                : appArgs as IAppArgs ?? new AppArgs(appArgs);
            this.getLogger = getLogger ?? NullLogManager.GetNullLogger;
            this.CheckLicense = checkLicense ?? ((appid, ctx) => new LicenseCheckResult(appid, true));
            this.AssemblyFilter = defaultAssemblyFilter ?? (a => !a.IsSystemAssembly());
            this.appFolder = appFolder;
            this.configFolders = configFolders;
            this.licenseFolders = licenseFolders;

            this.InitializationMonitor = new InitializationMonitor<IAppRuntime>(this.GetType());
            this.InitializeAppProperties(
                Assembly.GetEntryAssembly(),
                isRoot,
                appId ?? this.AppArgs.AppId,
                appInstanceId ?? this.AppArgs.AppInstanceId,
                appVersion);

            this[AppIdentityKey] = new AppIdentity(this[AppIdKey] as string, this[AppVersionKey] as string);
        }

        /// <summary>
        /// Gets the application arguments.
        /// </summary>
        public IAppArgs AppArgs { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected virtual ILogger Logger => this.getLogger(this.GetType().FullName);

        /// <summary>
        /// Gets the check license delegate.
        /// </summary>
        /// <value>
        /// A function delegate that yields an ILicenseCheckResult.
        /// </value>
        protected Func<AppIdentity, IContext?, ILicenseCheckResult> CheckLicense { get; }

        /// <summary>
        /// Gets the assembly filter.
        /// </summary>
        /// <value>
        /// The assembly filter.
        /// </value>
        protected Func<AssemblyName, bool> AssemblyFilter { get; }

        /// <summary>
        /// Gets the initialization monitor.
        /// </summary>
        /// <value>
        /// The initialization monitor.
        /// </value>
        protected InitializationMonitor<IAppRuntime> InitializationMonitor { get; private set; }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        /// <param name="context">An optional context for initialization.</param>
        void IInitializable.Initialize(IContext? context)
        {
            this.InitializationMonitor.AssertIsNotStarted();

            this.InitializationMonitor.Start();

            this.InitializeCore(context);

            this.InitializationMonitor.Complete();
        }

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
        public virtual string GetAppLocation() => this.appLocation ??= this.ComputeAppLocation(this.appFolder);

        /// <summary>
        /// Gets the location of the application with the indicated identity.
        /// </summary>
        /// <param name="appIdentity">The application identity.</param>
        /// <param name="throwOnNotFound">Optional. True to throw if the indicated app is not found.</param>
        /// <returns>
        /// A path indicating the indicated application location.
        /// </returns>
        public virtual string? GetAppLocation(AppIdentity? appIdentity, bool throwOnNotFound = true)
        {
            if (appIdentity == null || appIdentity.Equals(this.GetAppIdentity()))
            {
                return this.GetAppLocation();
            }

            return throwOnNotFound
                ? throw new InvalidOperationException($"App '{appIdentity}' not found.")
                : (string?)null;
        }

        /// <summary>
        /// Gets the application bin folders from where application is loaded.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the application bin folders in this
        /// collection.
        /// </returns>
        public virtual IEnumerable<string> GetAppBinLocations()
        {
            yield return this.GetAppLocation();
        }

        /// <summary>
        /// Gets the application directories where configuration files are stored.
        /// </summary>
        /// <returns>
        /// The application configuration directories.
        /// </returns>
        public IEnumerable<string> GetAppConfigLocations() => this.configLocations ??= this.ComputeConfigLocations(this.configFolders);

        /// <summary>
        /// Gets the application directories where license files are stored.
        /// </summary>
        /// <returns>
        /// The application configuration directories.
        /// </returns>
        public IEnumerable<string> GetAppLicenseLocations() => this.licenseLocations ??= this.ComputeLicenseLocations(this.licenseFolders);

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <param name="assemblyFilter">Optional. A filter for the assemblies.</param>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        public virtual IEnumerable<Assembly> GetAppAssemblies(Func<AssemblyName, bool>? assemblyFilter = null)
        {
            this.InitializationMonitor.AssertIsCompletedSuccessfully();

            // TODO The assemblies from the current domain do not consider the not loaded
            // but required referenced assemblies. Therefore load all the references recursively.
            // This could be optimized somehow.
            assemblyFilter ??= this.AssemblyFilter;
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
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        public Assembly LoadAssemblyFromName(AssemblyName assemblyName)
        {
#if NET461
            return Assembly.Load(assemblyName);
#else
            return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
#endif
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
#if NET461
            return Assembly.LoadFile(assemblyFilePath);
#else
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFilePath);
#endif
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{this.GetAppIdentity()}/{this.GetAppInstanceId()} ({this.GetType().Name})";
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Kephas.Application.AppRuntimeBase and optionally
        /// releases the managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to
        ///                         release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (this.InitializationMonitor.IsCompleted)
            {
#if NET461
                AppDomain.CurrentDomain.AssemblyResolve -= this.HandleAssemblyResolve;
#else
                AssemblyLoadContext.Default.Resolving -= this.HandleAssemblyResolving;
#endif
            }

            this.isDisposed = true;
        }

        /// <summary>
        /// Initializes the application properties: AppId, AppInstanceId, and AppVersion.
        /// </summary>
        /// <param name="entryAssembly">The entry assembly.</param>
        /// <param name="isRoot">Indicates whether the application instance is the root.</param>
        /// <param name="appId">Identifier for the application.</param>
        /// <param name="appInstanceId">Identifier for the application instance.</param>
        /// <param name="appVersion">The application version.</param>
        protected virtual void InitializeAppProperties(Assembly entryAssembly, bool? isRoot, string? appId, string? appInstanceId, string? appVersion)
        {
            this[IsRootKey] = isRoot ??= string.IsNullOrEmpty(appId);
            this[AppIdKey] = appId = this.GetAppId(entryAssembly, appId);
            this[AppInstanceIdKey] = appInstanceId = string.IsNullOrEmpty(appInstanceId)
                                                        ? isRoot.Value
                                                            ? appId
                                                            : $"{appId}-{Guid.NewGuid():N}"
                                                        : appInstanceId;
            this[AppVersionKey] = string.IsNullOrEmpty(appVersion) ? (entryAssembly?.GetName().Version.ToString() ?? "0.0.0.0") : appVersion;
        }

#if NET461
        /// <summary>
        /// Handles the AppDomain.CurrentDomain.AssemblyResolve event.
        /// </summary>
        /// <param name="sender">Source of the event.</param>
        /// <param name="args">Resolve event information.</param>
        /// <returns>
        /// The resolved assembly -or- <c>null</c>.
        /// </returns>
        protected virtual Assembly? HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
#else
        /// <summary>
        /// Handles the AssemblyLoadContext.Resolving event.
        /// </summary>
        /// <param name="loadContext">Context for the load.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>
        /// The resolved assembly -or- <c>null</c>.
        /// </returns>
        protected virtual Assembly? HandleAssemblyResolving(AssemblyLoadContext loadContext, AssemblyName assemblyName)
        {
#endif
            var assemblyFullName = assemblyName.FullName;
            if (!this.IsCodeAssembly(assemblyFullName))
            {
                return null;
            }

            var appAssemblies = this.GetAppAssembliesRaw();
            var assembly = appAssemblies.FirstOrDefault(a => a.FullName == assemblyFullName);
            if (assembly == null)
            {
                var name = assemblyName.Name;
                var version = assemblyName.Version;
                var publicKeyToken = assemblyName.GetPublicKeyToken();
                bool? match;
                (assembly, match) = appAssemblies.Select(a => (assembly: a, match: this.IsAssemblyMatch(a.GetName(), name, version, publicKeyToken))).FirstOrDefault(m => m.match != false);

                if (assembly == null)
                {
                    var fileName = $"{name}.dll";
#if NET461
                    if (!string.IsNullOrEmpty(args.RequestingAssembly?.Location))
                    {
                        var filePath = Path.Combine(args.RequestingAssembly.Location, fileName);
                        if (File.Exists(filePath))
                        {
                            assembly = this.LoadAssemblyFromPath(filePath);
                        }
                    }

                    if (assembly == null)
                    {
#endif
                        var appBinLocations = this.GetAppBinLocations();
                        foreach (var binLocation in appBinLocations)
                        {
                            var filePath = Path.Combine(binLocation, fileName);
                            if (File.Exists(filePath))
                            {
                                assembly = this.LoadAssemblyFromPath(filePath);
                                break;
                            }
                        }
#if NET461
                    }
#endif
                }
                else if (match == null)
                {
                    // a match only by name is not accepted.
                    this.Logger.Warn("The best match for assembly '{assembly}' is '{resolvedAssembly}' from '{assemblyLocation}', which is not acceptable.", assemblyFullName, assembly, assembly.Location);
                    assembly = null;
                }
            }

            if (assembly != null)
            {
                this.Logger.Debug("Assembly '{assembly}' was resolved using '{resolvedAssembly}' from '{assemblyLocation}'.", assemblyFullName, assembly, assembly.Location);
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
        /// True if assembly match, false if not, <c>null</c> if the assembly matches the name but not the version or public key token.
        /// </returns>
        protected virtual bool? IsAssemblyMatch(AssemblyName assemblyName, string name, Version version, byte[] publicKeyToken)
        {
            if (assemblyName.Name != name)
            {
                return false;
            }

            // TODO: what if the assembly matches the name, but not the version or the public key token?
            if (EqualArray(assemblyName.GetPublicKeyToken(), publicKeyToken))
            {
                return true;
            }

            return null;
        }

        /// <summary>
        /// Query if 'assemblyName' is a code assembly.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>
        /// True if code assembly, false if not.
        /// </returns>
        protected virtual bool IsCodeAssembly(string? assemblyName)
        {
            return !string.IsNullOrEmpty(assemblyName) && !assemblyName.Contains(".resources,");
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
        protected virtual string GetAppId(Assembly entryAssembly, string? appId)
        {
            if (!string.IsNullOrEmpty(appId))
            {
                return appId!;
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
        /// Gets the referenced assemblies.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// An array of assembly name.
        /// </returns>
        protected virtual IEnumerable<AssemblyName> GetReferencedAssemblies(Assembly assembly)
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
            var loadedAssemblies = this.GetAppAssembliesRaw().ToList();

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
                foreach (var referencesToLoad in assembliesToCheck
                    .Select(assembly => this.GetReferencedAssemblies(assembly)
                    .Where(a => !loadedAssemblyRefs.Contains(a.Name) && assemblyFilter(a))
                    .ToList()))
                {
                    loadedAssemblyRefs.AddRange(referencesToLoad.Select(an => an.Name));
                    assemblyRefsToLoad.AddRange(referencesToLoad);
                }

                assembliesToCheck = assemblyRefsToLoad.Select(this.TryLoadAssembly).Where(a => a != null).ToList();
                assemblies.AddRange(assembliesToCheck);
            }

            return assemblies;
        }

        /// <summary>
        /// Initializes the service, ensuring that the assembly resolution is properly handled.
        /// </summary>
        /// <param name="context">Optional. An optional context for initialization.</param>
        protected virtual void InitializeCore(IContext? context = null)
        {
#if NET461
            AppDomain.CurrentDomain.AssemblyResolve += this.HandleAssemblyResolve;
#else
            AssemblyLoadContext.Default.Resolving += this.HandleAssemblyResolving;
#endif
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

        private IEnumerable<Assembly> GetAppAssembliesRaw()
        {
            // TODO AssemblyLoadContext.Default.Assemblies;
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        private Assembly? TryLoadAssembly(AssemblyName n)
        {
            try
            {
                return this.LoadAssemblyFromName(n);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, Strings.AppRuntimeBase_CannotLoadAssembly_Exception, n);
                return null;
            }
        }

        private string ComputeAppLocation(string? appFolder)
        {
            if (!string.IsNullOrEmpty(appFolder))
            {
                return Path.GetFullPath(appFolder);
            }

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetLocationDirectory();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string[] ComputeConfigLocations(IEnumerable<string>? configFolders) =>
            this.ComputeLocations(configFolders, DefaultConfigFolder);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string[] ComputeLicenseLocations(IEnumerable<string>? licenseFolders) =>
            this.ComputeLocations(licenseFolders, DefaultLicenseFolder);

        private string[] ComputeLocations(IEnumerable<string>? folders, string defaultFolder)
        {
            if (folders == null)
            {
                return new[] { this.GetFullPath(defaultFolder) };
            }

            var locations = new List<string>();
            locations.AddRange(folders.Select(this.GetFullPath));

            return locations.Count == 0
                ? new[] { this.GetFullPath(defaultFolder) }
                : locations.Distinct().ToArray();
        }
    }
}