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
    using System.Reflection;
    using System.Runtime.Loader;

    using Kephas.Collections;
    using Kephas.Dynamic;
    using Kephas.IO;
    using Kephas.Logging;
    using Kephas.Reflection;
    using Kephas.Resources;
    using Kephas.Services;
    using Kephas.Services.Transitions;

    /// <summary>
    /// An application application runtime providing only assemblies loaded by the runtime.
    /// </summary>
    public abstract class AppRuntimeBase : Expando, IAppRuntime, IInitializable, IDisposable
    {
        /// <summary>
        /// A pattern specifying the assembly file search.
        /// </summary>
        protected static readonly string AssemblyFileSearchPattern = "*.dll";

        /// <summary>
        /// The assembly file extension.
        /// </summary>
        protected static readonly string AssemblyFileExtension = ".dll";

        private readonly Func<string, ILogger> getLogger;

        private readonly string? appFolder;
        private readonly IEnumerable<string>? configFolders;
        private readonly IEnumerable<string>? licenseFolders;
        private readonly IEnumerable<Assembly>? appAssemblies;

        private IEnumerable<Assembly>? assemblyCache;
        private string? appLocation;
        private ILocations? configLocations;
        private ILocations? licenseLocations;
        private bool isDisposed = false; // To detect redundant calls

        /// <summary>
        /// Initializes a new instance of the <see cref="AppRuntimeBase"/> class.
        /// </summary>
        /// <param name="getLogger">Optional. The get logger delegate.</param>
        /// <param name="appAssemblies">Optional. The application assemblies. If not provided, the loaded assemblies are considered.</param>
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
        /// <param name="getLocations">Optional. Function for getting application locations.</param>
        protected AppRuntimeBase(
            Func<string, ILogger>? getLogger = null,
            IEnumerable<Assembly>? appAssemblies = null,
            string? appFolder = null,
            IEnumerable<string>? configFolders = null,
            IEnumerable<string>? licenseFolders = null,
            bool? isRoot = null,
            string? appId = null,
            string? appInstanceId = null,
            string? appVersion = null,
            IDynamic? appArgs = null,
            Func<string, string, IEnumerable<string>, ILocations>? getLocations = null)
            : base(isThreadSafe: true)
        {
            this.AppArgs = appArgs == null
                ? new AppArgs()
                : appArgs as IAppArgs ?? new AppArgs(appArgs);
            this.getLogger = getLogger ?? NullLogManager.GetNullLogger;
            this.appAssemblies = appAssemblies;
            this.appFolder = appFolder;
            this.configFolders = configFolders;
            this.licenseFolders = licenseFolders;
            this.GetLocations = getLocations 
                ?? ((name, basePath, relativePaths) => new FolderLocations(relativePaths, basePath, name));

            this.InitializationMonitor = new InitializationMonitor<IAppRuntime>(this.GetType());
            this.InitializeAppProperties(
                Assembly.GetEntryAssembly()!,
                isRoot ?? this.AppArgs.RunAsRoot,
                appId ?? this.AppArgs.AppId,
                appInstanceId ?? this.AppArgs.AppInstanceId,
                appVersion,
                this.AppArgs.Environment);

            this[IAppRuntime.AppIdentityKey] = new AppIdentity((string)this[IAppRuntime.AppIdKey]!,
                this[IAppRuntime.AppVersionKey] as string);
        }

        /// <summary>
        /// Gets the application arguments.
        /// </summary>
        public IAppArgs AppArgs { get; }

        /// <summary>
        /// Gets a value indicating whether the application is the root of an application hierarchy.
        /// </summary>
        /// <returns>
        /// A value indicating whether the application is the root of an application hierarchy.
        /// </returns>
        public bool IsRoot { get; private set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected virtual ILogger Logger => this.getLogger(this.GetType().FullName!);

        /// <summary>
        /// Gets a function for getting application locations.
        /// </summary>
        protected Func<string, string, IEnumerable<string>, ILocations> GetLocations { get; }

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
            IAppRuntime appRuntime = this;
            if (appIdentity == null || appIdentity.Equals(appRuntime.GetAppIdentity()))
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
        public IEnumerable<string> GetAppConfigLocations() => this.configLocations ??=
            this.ComputeLocations(this.configFolders, IAppRuntime.DefaultConfigFolder);

        /// <summary>
        /// Gets the application directories where license files are stored.
        /// </summary>
        /// <returns>
        /// The application configuration directories.
        /// </returns>
        public IEnumerable<string> GetAppLicenseLocations() => this.licenseLocations ??=
            this.ComputeLocations(this.licenseFolders, IAppRuntime.DefaultLicenseFolder);

        /// <summary>
        /// Gets the application assemblies.
        /// </summary>
        /// <returns>
        /// An enumeration of application assemblies.
        /// </returns>
        public virtual IEnumerable<Assembly> GetAppAssemblies()
        {
            this.InitializationMonitor.AssertIsCompletedSuccessfully();

            return this.assemblyCache ??= this.ComputeAppAssemblies(this.IsAppAssembly);
        }

        /// <summary>
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyName">The name of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        protected Assembly LoadAssemblyFromName(AssemblyName assemblyName)
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyName(assemblyName);
        }

        /// <summary>
        /// Attempts to load an assembly.
        /// </summary>
        /// <param name="assemblyFilePath">The file path of the assembly to be loaded.</param>
        /// <returns>
        /// The resolved assembly reference.
        /// </returns>
        protected Assembly LoadAssemblyFromPath(string assemblyFilePath)
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFilePath);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            IAppRuntime appRuntime = this;
            return $"{appRuntime.GetAppIdentity()}/{appRuntime.GetAppInstanceId()} ({this.GetType().Name})";
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
                AssemblyLoadContext.Default.Resolving -= this.HandleAssemblyResolving;
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
        /// <param name="environment">The environment.</param>
        protected virtual void InitializeAppProperties(
            Assembly entryAssembly,
            bool? isRoot,
            string? appId,
            string? appInstanceId,
            string? appVersion,
            string? environment)
        {
            this.IsRoot = isRoot ??= string.IsNullOrEmpty(appId);
            this[IAppRuntime.AppIdKey] = appId = this.GetAppId(entryAssembly, appId);
            this[IAppRuntime.AppInstanceIdKey] = appInstanceId = string.IsNullOrEmpty(appInstanceId)
                ? isRoot.Value
                    ? appId
                    : $"{appId}-{Guid.NewGuid():N}"
                : appInstanceId;
            this[IAppRuntime.AppVersionKey] = string.IsNullOrEmpty(appVersion)
                ? (entryAssembly?.GetName()?.Version?.ToString() ?? "0.0.0.0")
                : appVersion;
            this[IAppRuntime.EnvKey] = environment;
        }

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
                (assembly, match) = appAssemblies
                    .Select(a => (assembly: a,
                        match: this.IsAssemblyMatch(a.GetName(), name!, version!, publicKeyToken!)))
                    .FirstOrDefault(m => m.match != false);

                if (assembly == null)
                {
                    var fileName = $"{name}.dll";
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
                }
                else if (match == null)
                {
                    // a match only by name is not accepted.
                    this.Logger.Warn(
                        "The best match for assembly '{assembly}' is '{resolvedAssembly}' from '{assemblyLocation}', which is not acceptable.",
                        assemblyFullName, assembly, assembly.Location);
                    assembly = null;
                }
            }

            if (assembly != null)
            {
                this.Logger.Debug(
                    "Assembly '{assembly}' was resolved using '{resolvedAssembly}' from '{assemblyLocation}'.",
                    assemblyFullName, assembly, assembly.Location);
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
        protected virtual bool? IsAssemblyMatch(AssemblyName assemblyName, string name, Version version,
            byte[]? publicKeyToken)
        {
            if (assemblyName.Name != name)
            {
                return false;
            }

            // TODO: what if the assembly matches the name, but not the version or the public key token?
            var assemblyPublicKeyToken = assemblyName.GetPublicKeyToken();
            if (assemblyPublicKeyToken == publicKeyToken)
            {
                return true;
            }

            if (assemblyPublicKeyToken?.Length != publicKeyToken?.Length)
            {
                return null;
            }

            return publicKeyToken!.SequenceEqual(assemblyPublicKeyToken!) ? true : null;
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

            var assemblyName = entryAssembly?.GetName();
            if (assemblyName == null || assemblyName.Name!.Contains("TestRunner"))
            {
                return "App";
            }

            var titleAttr = entryAssembly!.GetCustomAttribute<AssemblyTitleAttribute>();
            if (titleAttr != null)
            {
                return titleAttr.Title;
            }

            var productAttr = entryAssembly!.GetCustomAttribute<AssemblyProductAttribute>();
            if (productAttr != null)
            {
                return productAttr.Product;
            }

            return assemblyName.Name;
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
            var loadedAssemblyRefs = new HashSet<string>(loadedAssemblies.Select(a => a.GetName().Name!));
            var assembliesToCheck = new List<Assembly>(assemblies);

            while (assembliesToCheck.Count > 0)
            {
                var assemblyRefsToLoad = new HashSet<AssemblyName>();
                foreach (var referencesToLoad in assembliesToCheck
                             .Select(assembly => this.GetReferencedAssemblies(assembly)
                                 .Where(a => !loadedAssemblyRefs.Contains(a.Name!) && assemblyFilter(a))
                                 .ToList()))
                {
                    loadedAssemblyRefs.AddRange(referencesToLoad.Select(an => an.Name!));
                    assemblyRefsToLoad.AddRange(referencesToLoad);
                }

                assembliesToCheck = new List<Assembly>();
                foreach (var an in assemblyRefsToLoad)
                {
                    var assembly = this.TryLoadAssembly(an);
                    if (assembly != null)
                    {
                        assembliesToCheck.Add(assembly);
                    }
                }

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
            AssemblyLoadContext.Default.Resolving += this.HandleAssemblyResolving;
        }

        private IEnumerable<Assembly> GetAppAssembliesRaw()
        {
            // TODO AssemblyLoadContext.Default.Assemblies;
            return this.appAssemblies ?? AppDomain.CurrentDomain.GetAssemblies();
        }

        private Assembly? TryLoadAssembly(AssemblyName n)
        {
            try
            {
                return this.LoadAssemblyFromName(n);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, AbstractionStrings.AppRuntimeBase_CannotLoadAssembly_Exception, n);
                return null;
            }
        }

        private string ComputeAppLocation(string? basePath)
        {
            if (!string.IsNullOrEmpty(basePath))
            {
                return Path.GetFullPath(basePath);
            }

            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            return assembly.GetLocationDirectory();
        }

        private ILocations ComputeLocations(IEnumerable<string>? folders, string defaultFolder)
        {
            var foldersArray = folders?.ToArray() ?? new[] { defaultFolder };
            if (foldersArray.Length == 0)
            {
                foldersArray = new[] { defaultFolder };
            }

            return this.GetLocations(defaultFolder, this.GetAppLocation(), foldersArray);
        }
    }
}