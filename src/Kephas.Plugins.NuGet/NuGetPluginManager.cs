// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPluginManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the nu get plugin manager class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.NuGet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using global::NuGet.Configuration;
    using global::NuGet.Frameworks;
    using global::NuGet.Packaging;
    using global::NuGet.Packaging.Core;
    using global::NuGet.Protocol.Core.Types;
    using global::NuGet.Resolver;
    using global::NuGet.Versioning;
    using Kephas.Application;
    using Kephas.Collections;
    using Kephas.Configuration;
    using Kephas.Diagnostics;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Plugins;
    using Kephas.Plugins.Reflection;
    using Kephas.Services;
    using Kephas.Threading.Tasks;

    /// <summary>
    /// Manager for plugins based on the NuGet infrastructure.
    /// </summary>
    [OverridePriority(Priority.Low)]
    public class NuGetPluginManager : PluginManagerBase
    {
        /// <summary>
        /// The default packages folder.
        /// </summary>
        public const string DefaultPackagesFolder = ".packages";

        // check the following resource for documentation
        // https://martinbjorkstrom.com/posts/2018-09-19-revisiting-nuget-client-libraries

        private ISettings settings;
        private PluginsSettings pluginsSettings;
        private global::NuGet.Common.ILogger nativeLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPluginManager"/> class.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="pluginsConfig">The plugins configuration.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public NuGetPluginManager(
            IAppRuntime appRuntime,
            IContextFactory contextFactory,
            IConfiguration<PluginsSettings> pluginsConfig,
            ILogManager logManager = null)
            : base(appRuntime, contextFactory, logManager)
        {
            this.nativeLogger = new NuGetLogger(this.Logger);
            this.pluginsSettings = pluginsConfig.Settings;
        }

        /// <summary>
        /// Gets the available plugins asynchronously.
        /// </summary>
        /// <param name="filter">Optional. Specifies the filter.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the available plugins.
        /// </returns>
        public override async Task<IOperationResult<IEnumerable<IPluginInfo>>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default)
        {
            var searchContext = this.CreateSearchContext(filter);
            var repositories = this.GetSourceRepositories();

            cancellationToken.ThrowIfCancellationRequested();

            var availablePackages = new HashSet<IPackageSearchMetadata>();
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    using (var cacheContext = new SourceCacheContext())
                    {
                        foreach (var sourceRepository in repositories)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>().PreserveThreadContext();
                            var searchFilter = new SearchFilter(includePrerelease: searchContext.IncludePrerelease);
                            searchFilter.OrderBy = SearchOrderBy.Id;
                            searchFilter.IncludeDelisted = false;

                            try
                            {
                                var packages = await searchResource.SearchAsync(
                                    searchContext.SearchTerm ?? this.pluginsSettings.SearchTerm ?? "plugin",
                                    searchFilter,
                                    searchContext.Skip,
                                    searchContext.Take,
                                    this.nativeLogger,
                                    cancellationToken).PreserveThreadContext();

                                availablePackages.AddRange(packages);
                            }
                            catch (Exception ex)
                            {
                                this.Logger.Warn(ex, "Could not access source repository '{repository}'.", sourceRepository.PackageSource.Source);
                            }
                        }
                    }
                }).PreserveThreadContext();

            var result = new OperationResult<IEnumerable<IPluginInfo>>(availablePackages.Select(this.ToPluginInfo))
                                .MergeResult(opResult)
                                .Elapsed(opResult.Elapsed);
            return result;
        }

        /// <summary>
        /// Installs the plugin asynchronously.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the requested operation is invalid.</exception>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="options">Optional. Options for controlling the operation.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the install operation result.
        /// </returns>
        public override async Task<IOperationResult<IPlugin>> InstallPluginAsync(AppIdentity plugin, Action<IPluginContext> options = null, CancellationToken cancellationToken = default)
        {
            var (_, state, pid) = this.GetInstalledPluginData(plugin);
            if (state != PluginState.None)
            {
                throw new InvalidOperationException($"Plugin {plugin} is already installed. State: '{state}', version: '{pid.Version}'.");
            }

            IPluginInfo pluginInfo = null;
            IPlugin pluginData = null;
            var context = this.CreatePluginContext(options)
                .Operation(PluginOperation.Install, overwrite: false)
                .Plugin(plugin);
            var opResult = await Profiler.WithInfoStopwatchAsync(
                async () =>
                {
                    var repositories = this.GetSourceRepositories();
                    using (var cacheContext = new SourceCacheContext())
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var currentFramework = this.AppRuntime.GetAppFramework();
                        var nugetFramework = NuGetFramework.ParseFolder(currentFramework);

                        var (pluginPackageIdentity, packageReaders) = await this.GetPackageReadersAsync(plugin, repositories, cacheContext, nugetFramework, cancellationToken).PreserveThreadContext();

                        pluginInfo = new PluginInfo(pluginPackageIdentity.Id, pluginPackageIdentity.Version.ToString());
                        context.Plugin(plugin = pluginInfo.GetIdentity());

                        var pluginFolder = Path.Combine(this.AppRuntime.GetPluginsFolder(), pluginPackageIdentity.Id);
                        if (!Directory.Exists(pluginFolder))
                        {
                            Directory.CreateDirectory(pluginFolder);
                        }

                        try
                        {
                            var frameworkReducer = new FrameworkReducer();
                            foreach (var packageReader in packageReaders)
                            {
                                await this.InstallPluginBinAsync(plugin, context, nugetFramework, pluginFolder, frameworkReducer, packageReader, cancellationToken).PreserveThreadContext();
                                await this.InstallPluginContentAsync(plugin, context, nugetFramework, pluginFolder, frameworkReducer, packageReader, cancellationToken).PreserveThreadContext();
                            }

                            PluginHelper.SetPluginData(pluginFolder, PluginState.PendingInitialization, pluginPackageIdentity.Version.ToString());
                        }
                        catch
                        {
                            Directory.Delete(pluginFolder, recursive: true);
                            throw;
                        }

                        pluginData = new Plugin(pluginInfo) { FolderPath = pluginFolder };
                    }
                }).PreserveThreadContext();

            this.Logger.Info("Plugin {plugin} successfully installed, awaiting initialization. Elapsed: {elapsed:c}.", plugin, opResult.Elapsed);

            return new OperationResult<IPlugin>(pluginData)
                    .MergeResult(opResult)
                    .MergeMessage($"Plugin {plugin} successfully installed, awaiting initialization. Elapsed: {opResult.Elapsed:c}.")
                    .Elapsed(opResult.Elapsed);
        }

        /// <summary>
        /// Installs the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the plugin data.
        /// </returns>
        protected override async Task<IPlugin> InstallPluginCoreAsync(AppIdentity plugin, IPluginContext context, CancellationToken cancellationToken = default)
        {
            var repositories = this.GetSourceRepositories();
            using (var cacheContext = new SourceCacheContext())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var currentFramework = this.AppRuntime.GetAppFramework();
                var nugetFramework = NuGetFramework.ParseFolder(currentFramework);

                var (pluginPackageIdentity, packageReaders) = await this.GetPackageReadersAsync(plugin, repositories, cacheContext, nugetFramework, cancellationToken).PreserveThreadContext();

                var pluginInfo = new PluginInfo(pluginPackageIdentity.Id, pluginPackageIdentity.Version.ToString());
                context.Plugin(plugin = pluginInfo.GetIdentity());

                var pluginFolder = Path.Combine(this.AppRuntime.GetPluginsFolder(), pluginPackageIdentity.Id);
                if (!Directory.Exists(pluginFolder))
                {
                    Directory.CreateDirectory(pluginFolder);
                }

                try
                {
                    var frameworkReducer = new FrameworkReducer();
                    foreach (var packageReader in packageReaders)
                    {
                        await this.InstallPluginBinAsync(plugin, context, nugetFramework, pluginFolder, frameworkReducer, packageReader, cancellationToken).PreserveThreadContext();
                        await this.InstallPluginContentAsync(plugin, context, nugetFramework, pluginFolder, frameworkReducer, packageReader, cancellationToken).PreserveThreadContext();
                    }

                    PluginHelper.SetPluginData(pluginFolder, PluginState.PendingInitialization, pluginPackageIdentity.Version.ToString());
                }
                catch
                {
                    Directory.Delete(pluginFolder, recursive: true);
                    throw;
                }

                return new Plugin(pluginInfo) { FolderPath = pluginFolder };
            }
        }

        /// <summary>
        /// Gets the packages folder.
        /// </summary>
        /// <param name="defaultPackagesFolder">Optional. The default packages folder.</param>
        /// <returns>
        /// The packages folder.
        /// </returns>
        protected virtual string GetPackagesFolder(string defaultPackagesFolder = null)
        {
            defaultPackagesFolder = defaultPackagesFolder ?? DefaultPackagesFolder;
            if (string.IsNullOrEmpty(this.pluginsSettings.PackagesFolder))
            {
                var settings = this.GetSettings();
                var repositoryPathSettings = settings.GetSection("config")?.GetFirstItemWithAttribute<AddItem>("key", "repositoryPath");
                var repositoryPath = repositoryPathSettings?.Value;
                var fullRepositoryPath = string.IsNullOrEmpty(repositoryPath)
                    ? this.AppRuntime.GetFullPath(defaultPackagesFolder)
                    : Path.IsPathRooted(repositoryPath)
                        ? repositoryPath
                        : Path.GetFullPath(Path.Combine(this.GetSettingsFolderPath(), repositoryPath));
                return fullRepositoryPath;
            }

            return Path.IsPathRooted(this.pluginsSettings.PackagesFolder)
                ? this.pluginsSettings.PackagesFolder
                : Path.Combine(this.AppRuntime.GetAppLocation(), this.pluginsSettings.PackagesFolder);
        }

        /// <summary>
        /// Gets the settings folder path.
        /// </summary>
        /// <returns>
        /// The settings folder path.
        /// </returns>
        protected virtual string GetSettingsFolderPath()
        {
            return string.IsNullOrEmpty(this.pluginsSettings.NuGetConfigPath)
                ? this.AppRuntime.GetAppConfigFullPath()
                : this.AppRuntime.GetFullPath(this.pluginsSettings.NuGetConfigPath);
        }

        /// <summary>
        /// Gets the NuGet settings.
        /// </summary>
        /// <returns>
        /// The NuGet settings.
        /// </returns>
        protected virtual ISettings GetSettings()
        {
            if (this.settings == null)
            {
                string root = this.GetSettingsFolderPath();
                this.settings = new Settings(root);
            }

            return this.settings;
        }

        /// <summary>
        /// Gets the source repository provider.
        /// </summary>
        /// <returns>
        /// The source repository provider.
        /// </returns>
        protected virtual SourceRepositoryProvider GetSourceRepositoryProvider()
        {
            var settings = this.GetSettings();
            var sourceProvider = new PackageSourceProvider(settings);
            var sourceRepositoryProvider = new SourceRepositoryProvider(sourceProvider, Repository.Provider.GetCoreV3());
            return sourceRepositoryProvider;
        }

        /// <summary>
        /// Gets the source repositories.
        /// </summary>
        /// <returns>
        /// The source repositories.
        /// </returns>
        protected virtual IList<SourceRepository> GetSourceRepositories()
        {
            var sourceRepositoryProvider = this.GetSourceRepositoryProvider();
            var repositories = sourceRepositoryProvider.GetRepositories();
            // TODO test whether repository is available
            return repositories.ToList();
        }

        /// <summary>
        /// Converts a searchMetadata to a plugin information.
        /// </summary>
        /// <param name="searchMetadata">The search metadata.</param>
        /// <returns>
        /// SearchMetadata as an IPluginInfo.
        /// </returns>
        protected virtual IPluginInfo ToPluginInfo(IPackageSearchMetadata searchMetadata)
        {
            return new PluginInfo(
                searchMetadata.Identity.Id,
                searchMetadata.Identity.Version.ToString(),
                searchMetadata.Description,
                searchMetadata.Tags?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Converts a plugin identity to a package identity.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <returns>
        /// AppIdentity as a PackageIdentity.
        /// </returns>
        protected virtual PackageIdentity ToPackageIdentity(AppIdentity pluginIdentity)
        {
            return new PackageIdentity(pluginIdentity.Id, NuGetVersion.Parse(pluginIdentity.Version));
        }

        /// <summary>
        /// Gets the package path resolver.
        /// </summary>
        /// <returns>
        /// The package path resolver.
        /// </returns>
        protected virtual PackagePathResolver GetPackagePathResolver()
        {
            return new PackagePathResolver(this.GetPackagesFolder());
        }

        /// <summary>
        /// Installs the plugin library items asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="nugetFramework">The nuget framework.</param>
        /// <param name="pluginFolder">Pathname of the plugin folder.</param>
        /// <param name="frameworkReducer">The framework reducer.</param>
        /// <param name="packageReader">The package reader.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task InstallPluginBinAsync(AppIdentity plugin, IContext context, NuGetFramework nugetFramework, string pluginFolder, FrameworkReducer frameworkReducer, PackageReaderBase packageReader, CancellationToken cancellationToken)
        {
            const string libFolderName = "lib";

            var libItems = packageReader.GetLibItems();
            var nearestLibItemFwk = frameworkReducer.GetNearest(nugetFramework, libItems.Select(x => x.TargetFramework));

            var libItem = libItems.FirstOrDefault(l => l.TargetFramework == nearestLibItemFwk);
            if (!(libItem?.HasEmptyFolder ?? true))
            {
                await packageReader.CopyFilesAsync(pluginFolder, libItem.Items, (src, target, stream) => this.ExtractPackageFile(src, target, libFolderName, flatten: true), this.nativeLogger, cancellationToken).PreserveThreadContext();
            }

            var libFolder = Path.Combine(pluginFolder, libFolderName);
            if (Directory.Exists(libFolder))
            {
                Directory.Delete(libFolder, recursive: true);
            }
        }

        /// <summary>
        /// Installs the plugin content items asynchronously.
        /// </summary>
        /// <param name="plugin">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="nugetFramework">The nuget framework.</param>
        /// <param name="pluginFolder">Pathname of the plugin folder.</param>
        /// <param name="frameworkReducer">The framework reducer.</param>
        /// <param name="packageReader">The package reader.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task InstallPluginContentAsync(AppIdentity plugin, IContext context, NuGetFramework nugetFramework, string pluginFolder, FrameworkReducer frameworkReducer, PackageReaderBase packageReader, CancellationToken cancellationToken)
        {
            const string contentFolderName = "content";

            var contentItems = packageReader.GetContentItems();
            var nearestLibItemFwk = frameworkReducer.GetNearest(nugetFramework, contentItems.Select(x => x.TargetFramework));

            var contentItem = contentItems.FirstOrDefault(l => l.TargetFramework == nearestLibItemFwk);
            if (!(contentItem?.HasEmptyFolder ?? true))
            {
                await packageReader.CopyFilesAsync(pluginFolder, contentItem.Items, (src, target, stream) => this.ExtractPackageFile(src, target, contentFolderName, flatten: false), this.nativeLogger, cancellationToken).PreserveThreadContext();
            }

            var contentFolder = Path.Combine(pluginFolder, contentFolderName);
            if (Directory.Exists(contentFolder))
            {
                Directory.Delete(contentFolder, recursive: true);
            }
        }

        private async Task<(PackageIdentity pluginPackageIdentity, IList<PackageReaderBase> packageReaders)> GetPackageReadersAsync(AppIdentity plugin, IList<SourceRepository> repositories, SourceCacheContext cacheContext, NuGetFramework nugetFramework, CancellationToken cancellationToken)
        {
            var packagesFolder = this.GetPackagesFolder();

            var pluginPackageIdentity = this.ToPackageIdentity(plugin);
            var downloadResult = await this.DownloadPackageAsync(pluginPackageIdentity, packagesFolder, repositories, cacheContext, cancellationToken).PreserveThreadContext();
            if (downloadResult.Status != DownloadResourceResultStatus.Available)
            {
                throw new InvalidOperationException($"Plugin package {pluginPackageIdentity} not available ({downloadResult.Status}).");
            }

            var dependencies = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
            await this.GetPackageDependenciesAsync(
                pluginPackageIdentity,
                nugetFramework,
                cacheContext,
                this.nativeLogger,
                repositories,
                dependencies).PreserveThreadContext();

            cancellationToken.ThrowIfCancellationRequested();

            var resolverContext = new PackageResolverContext(
                this.pluginsSettings.ResolverDependencyBehavior,
                new[] { plugin.Id },
                Enumerable.Empty<string>(),
                Enumerable.Empty<PackageReference>(),
                Enumerable.Empty<PackageIdentity>(),
                dependencies,
                repositories.Select(s => s.PackageSource),
                this.nativeLogger);

            var resolver = new PackageResolver();
            var dependenciesToInstall = resolver.Resolve(resolverContext, cancellationToken)
                .Select(p => dependencies.Single(x => PackageIdentityComparer.Default.Equals(x, p)));

            cancellationToken.ThrowIfCancellationRequested();

            var packageReaders = await this.GetPackageReadersAsync(
                repositories,
                cacheContext,
                packagesFolder,
                dependenciesToInstall,
                cancellationToken).PreserveThreadContext();

            // get the right casing of the package ID, the provided casing might not be the right one.
            pluginPackageIdentity = dependenciesToInstall.FirstOrDefault(d => d.Equals(pluginPackageIdentity)) ?? pluginPackageIdentity;
            return (pluginPackageIdentity, packageReaders);
        }

        private string ExtractPackageFile(string sourceFile, string targetPath, string subFolder, bool flatten)
        {
            targetPath = targetPath.Replace('/', Path.DirectorySeparatorChar);
            var fileName = Path.GetFileName(targetPath);
            var searchTerm = Path.DirectorySeparatorChar + subFolder + Path.DirectorySeparatorChar;
            var indexSearchTerm = targetPath.IndexOf(searchTerm);
            if (indexSearchTerm >= 0)
            {
                targetPath = flatten
                    ? targetPath.Substring(0, indexSearchTerm + 1) + fileName
                    : targetPath.Substring(0, indexSearchTerm + 1) + targetPath.Substring(indexSearchTerm + searchTerm.Length);
            }

            var directory = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.Copy(sourceFile, targetPath, overwrite: true);
            return targetPath;
        }

        private async Task<List<PackageReaderBase>> GetPackageReadersAsync(IList<SourceRepository> repositories, SourceCacheContext cacheContext, string packagesFolder, IEnumerable<SourcePackageDependencyInfo> dependenciesToInstall, CancellationToken cancellationToken)
        {
            var downloadContext = new PackageDownloadContext(cacheContext);
            var downloadResources = await this.GetDownloadResourcesAsync(repositories, cancellationToken).PreserveThreadContext();

            var packageReaders = new List<PackageReaderBase>();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var dependency in dependenciesToInstall)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var packagePathResolver = this.GetPackagePathResolver();
                var installedPath = packagePathResolver.GetInstalledPath(dependency);
                if (installedPath != null)
                {
                    packageReaders.Add(new PackageFolderReader(installedPath));
                }
                else
                {
                    var downloadResult = await this.DownloadPackageAsync(
                            dependency,
                            packagesFolder,
                            downloadResources,
                            downloadContext,
                            cancellationToken).PreserveThreadContext();
                    packageReaders.Add(downloadResult.PackageReader);
                }
            }

            return packageReaders;
        }

        private async Task<List<DownloadResource>> GetDownloadResourcesAsync(IList<SourceRepository> repositories, CancellationToken cancellationToken = default)
        {
            var downloadResources = new List<DownloadResource>();
            foreach (var sourceRepository in repositories)
            {
                var downloadResource = await sourceRepository.GetResourceAsync<DownloadResource>(cancellationToken).PreserveThreadContext();
                downloadResources.Add(downloadResource);
            }

            return downloadResources;
        }

        private async Task<DownloadResourceResult> DownloadPackageAsync(PackageIdentity packageId, string packagesFolder, IList<SourceRepository> repositories, SourceCacheContext cacheContext, CancellationToken cancellationToken)
        {
            var downloadContext = new PackageDownloadContext(cacheContext);
            var downloadResources = await this.GetDownloadResourcesAsync(repositories, cancellationToken).PreserveThreadContext();
            return await this.DownloadPackageAsync(packageId, packagesFolder, downloadResources, downloadContext, cancellationToken).PreserveThreadContext();
        }

        private async Task<DownloadResourceResult> DownloadPackageAsync(PackageIdentity packageId, string packagesFolder, IEnumerable<DownloadResource> downloadResources, PackageDownloadContext downloadContext, CancellationToken cancellationToken)
        {
            DownloadResourceResult downloadResult = null;
            if (packageId is SourcePackageDependencyInfo dependencyInfo)
            {
                var downloadResource = await dependencyInfo.Source.GetResourceAsync<DownloadResource>(cancellationToken).PreserveThreadContext();
                downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                    packageId,
                    downloadContext,
                    packagesFolder,
                    this.nativeLogger,
                    default).PreserveThreadContext();
            }
            else
            {
                foreach (var downloadResource in downloadResources)
                {
                    downloadResult = await downloadResource.GetDownloadResourceResultAsync(
                        packageId,
                        downloadContext,
                        packagesFolder,
                        this.nativeLogger,
                        default).PreserveThreadContext();

                    if (downloadResult.Status == DownloadResourceResultStatus.Available)
                    {
                        break;
                    }
                }
            }

            if (downloadResult?.Status != DownloadResourceResultStatus.Available)
            {
                throw new InvalidOperationException($"Package {packageId} not available ({downloadResult?.Status}).");
            }

            return downloadResult;
        }

        private async Task GetPackageDependenciesAsync(
            PackageIdentity package,
            NuGetFramework framework,
            SourceCacheContext cacheContext,
            global::NuGet.Common.ILogger logger,
            IList<SourceRepository> repositories,
            ISet<SourcePackageDependencyInfo> availablePackages)
        {
            if (availablePackages.Contains(package))
            {
                return;
            }

            foreach (var sourceRepository in repositories)
            {
                var dependencyInfoResource = await sourceRepository.GetResourceAsync<DependencyInfoResource>().PreserveThreadContext();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                    package, framework, cacheContext, logger, CancellationToken.None).PreserveThreadContext();

                if (dependencyInfo == null)
                {
                    continue;
                }

                availablePackages.Add(dependencyInfo);
                foreach (var dependency in dependencyInfo.Dependencies)
                {
                    await this.GetPackageDependenciesAsync(
                        new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                        framework,
                        cacheContext,
                        logger,
                        repositories,
                        availablePackages);
                }
            }
        }
    }
}
