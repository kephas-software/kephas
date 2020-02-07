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
    using Kephas;
    using Kephas.Application;
    using Kephas.Application.Reflection;
    using Kephas.Collections;
    using Kephas.Configuration;
    using Kephas.Diagnostics;
    using Kephas.Interaction;
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
        /// <param name="eventHub">The event hub.</param>
        /// <param name="pluginsConfig">The plugins configuration.</param>
        /// <param name="logManager">Optional. Manager for log.</param>
        public NuGetPluginManager(
            IAppRuntime appRuntime,
            IContextFactory contextFactory,
            IEventHub eventHub,
            IConfiguration<PluginsSettings> pluginsConfig,
            ILogManager logManager = null)
            : base(appRuntime, contextFactory, eventHub, logManager)
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
        public override async Task<IOperationResult<IEnumerable<IAppInfo>>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default)
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

            var result = new OperationResult<IEnumerable<IAppInfo>>(availablePackages.Select(this.ToPluginInfo))
                                .MergeMessages(opResult)
                                .Complete(opResult.Elapsed);
            return result;
        }

        /// <summary>
        /// Installs the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">Optional. A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the plugin data.
        /// </returns>
        protected override async Task<IOperationResult<IPlugin>> InstallPluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken = default)
        {
            var repositories = this.GetSourceRepositories();
            using (var cacheContext = new SourceCacheContext())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = new OperationResult<IPlugin>();
                var currentFramework = this.AppRuntime.GetAppFramework();
                var nugetFramework = NuGetFramework.ParseFolder(currentFramework);

                var (pluginPackageIdentity, packageReaders) = await this.GetPackageReadersAsync(pluginIdentity, repositories, cacheContext, nugetFramework, cancellationToken).PreserveThreadContext();
                pluginIdentity = new AppIdentity(pluginPackageIdentity.Id, pluginPackageIdentity.Version.ToString());

                var pluginInfo = new PluginInfo(this.AppRuntime, this.PluginDataStore, pluginIdentity);
                context.PluginIdentity(pluginIdentity);

                var pluginFolder = Path.Combine(this.AppRuntime.GetPluginsLocation(), pluginPackageIdentity.Id);
                if (!Directory.Exists(pluginFolder))
                {
                    Directory.CreateDirectory(pluginFolder);
                }

                try
                {
                    var frameworkReducer = new FrameworkReducer();
                    foreach (var (packageId, packageReader) in packageReaders)
                    {
                        result.MergeMessages(
                            await this.InstallBinAsync(pluginIdentity, pluginFolder, context, packageId, nugetFramework, frameworkReducer, packageReader, cancellationToken)
                            .PreserveThreadContext());
                        result.MergeMessages(
                            await this.InstallContentAsync(pluginIdentity, pluginFolder, context, packageId, nugetFramework, frameworkReducer, packageReader, cancellationToken)
                            .PreserveThreadContext());
                    }

                    result.MergeMessages(
                        await this.InstallConfigAsync(pluginIdentity, pluginFolder, context, cancellationToken)
                        .PreserveThreadContext());

                    result.MergeMessages(
                        await this.InstallDataAsync(pluginIdentity, pluginFolder, context, cancellationToken)
                        .PreserveThreadContext());
                }
                catch
                {
                    Directory.Delete(pluginFolder, recursive: true);
                    throw;
                }

                result.ReturnValue = new Plugin(pluginInfo) { Location = pluginFolder };
                return result;
            }
        }

        /// <summary>
        /// Installs the configuration asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="pluginLocation">Pathname of the plugin folder.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task<IOperationResult> InstallConfigAsync(AppIdentity pluginIdentity, string pluginLocation, IPluginContext context, CancellationToken cancellationToken)
        {
            var sourceConfigFilesFolder = Path.Combine(pluginLocation, this.pluginsSettings.PackageConfigFolder);
            var targetConfigFilesFolder = this.AppRuntime.GetAppConfigLocations().First();

            var result = new OperationResult();
            if (!Directory.Exists(sourceConfigFilesFolder))
            {
                return result.MergeMessage($"No source configuration folder at '{sourceConfigFilesFolder}'.");
            }

            if (!Directory.Exists(targetConfigFilesFolder))
            {
                Directory.CreateDirectory(targetConfigFilesFolder);
            }

            var adjustment = sourceConfigFilesFolder.EndsWith(Path.DirectorySeparatorChar.ToString())
                || sourceConfigFilesFolder.EndsWith(Path.AltDirectorySeparatorChar.ToString())
                    ? 0 : 1;
            foreach (var configFile in Directory.GetFiles(sourceConfigFilesFolder, "*.*", SearchOption.AllDirectories))
            {
                var sourceFile = configFile.Substring(sourceConfigFilesFolder.Length + adjustment);
                var targetFile = Path.Combine(targetConfigFilesFolder, sourceFile);
                var targetDirectory = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }

                if (File.Exists(targetFile))
                {
                    if (context.Operation == PluginOperation.Update)
                    {
                        result.MergeMessage($"Configuration file '{targetFile}' exists already, will not be overwritten with '{configFile}'.");
                        this.Logger.Info("Configuration file '{targetFile}' exists already, will not be overwritten with '{sourceFile}'.", targetFile, configFile);
                    }
                    else
                    {
                        var extension = Path.GetExtension(targetFile);
                        var targetFileRaw = targetFile.Substring(0, targetFile.Length - extension.Length);
                        var renamedTargetFile = $"{targetFileRaw}-{DateTime.Now:yyyyMMddhhmmss}{extension}";
                        File.Move(targetFile, renamedTargetFile);
                        File.Copy(configFile, targetFile);

                        result.MergeMessage($"Configuration file '{targetFile}' exists already, will be renamed to '{Path.GetFileName(renamedTargetFile)}'. Check whether it should be changed.");
                        this.Logger.Warn("Configuration file '{targetFile}' exists already, will be renamed to '{sourceFile}'. Check whether it should be changed.", targetFile, Path.GetFileName(renamedTargetFile));
                    }
                }
                else
                {
                    File.Copy(configFile, targetFile);
                }
            }

            return result;
        }

        /// <summary>
        /// Installs the data asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="pluginLocation">Pathname of the plugin folder.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        protected virtual Task<IOperationResult> InstallDataAsync(AppIdentity pluginIdentity, string pluginLocation, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult());
        }

        /// <summary>
        /// Uninstall the plugin asynchronously (core implementation).
        /// </summary>
        /// <param name="pluginIdentity">Identifier for the plugin.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected override async Task<IOperationResult<IPlugin>> UninstallPluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
        {
            var result = new OperationResult<IPlugin>();

            result.MergeMessages(
                await this.UninstallDataAsync(pluginIdentity, context.Plugin.Location, context, cancellationToken)
                .PreserveThreadContext());

            result.MergeMessages(
                await this.UninstallConfigAsync(pluginIdentity, context.Plugin.Location, context, cancellationToken)
                .PreserveThreadContext());

            var baseResult = await base.UninstallPluginCoreAsync(pluginIdentity, context, cancellationToken)
                .PreserveThreadContext();

            result.ReturnValue = baseResult.ReturnValue;
            return result.MergeMessages(baseResult);
        }

        /// <summary>
        /// Uninstalls the configuration asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="pluginLocation">Pathname of the plugin folder.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task<IOperationResult> UninstallConfigAsync(AppIdentity pluginIdentity, string pluginLocation, IPluginContext context, CancellationToken cancellationToken)
        {
            var result = new OperationResult();
            if (context.Operation == PluginOperation.Update)
            {
                return result.MergeMessage($"Skipping configuration file uninstallation during update of {pluginIdentity}.");
            }

            var sourceConfigFilesFolder = Path.Combine(pluginLocation, this.pluginsSettings.PackageConfigFolder);
            var targetConfigFilesFolder = this.AppRuntime.GetAppConfigLocations().First();

            if (!Directory.Exists(sourceConfigFilesFolder))
            {
                return result.MergeMessage($"No source configuration folder at '{sourceConfigFilesFolder}'.");
            }

            if (!Directory.Exists(targetConfigFilesFolder))
            {
                return result.MergeMessage($"No target configuration folder at '{targetConfigFilesFolder}'.");
            }

            var adjustment = sourceConfigFilesFolder.EndsWith(Path.DirectorySeparatorChar.ToString())
                || sourceConfigFilesFolder.EndsWith(Path.AltDirectorySeparatorChar.ToString())
                    ? 0 : 1;
            foreach (var configFile in Directory.GetFiles(sourceConfigFilesFolder, "*.*", SearchOption.AllDirectories))
            {
                var sourceFile = configFile.Substring(sourceConfigFilesFolder.Length + adjustment);
                var targetFile = Path.Combine(targetConfigFilesFolder, sourceFile);
                var targetDirectory = Path.GetDirectoryName(targetFile);
                if (!Directory.Exists(targetDirectory))
                {
                    continue;
                }

                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                    result.MergeMessage($"Deleted configuration file '{targetFile}'.");
                }
            }

            return result;
        }

        /// <summary>
        /// Uninstalls data asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="pluginLocation">Pathname of the plugin folder.</param>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result that yields the operation result.
        /// </returns>
        protected virtual Task<IOperationResult> UninstallDataAsync(AppIdentity pluginIdentity, string pluginLocation, IPluginContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<IOperationResult>(new OperationResult());
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
                ? this.AppRuntime.GetAppConfigLocations().FirstOrDefault() ?? this.AppRuntime.GetAppLocation()
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
        /// SearchMetadata as an IAppInfo.
        /// </returns>
        protected virtual IAppInfo ToPluginInfo(IPackageSearchMetadata searchMetadata)
        {
            return new PluginInfo(
                this.AppRuntime,
                this.PluginDataStore,
                new AppIdentity(searchMetadata.Identity.Id, searchMetadata.Identity.Version.ToString()),
                searchMetadata.Description,
                searchMetadata.Tags?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Converts a plugin identity to a package identity.
        /// </summary>
        /// <param name="pluginId">The plugin identity.</param>
        /// <returns>
        /// AppIdentity as a PackageIdentity.
        /// </returns>
        protected virtual PackageIdentity ToPackageIdentity(AppIdentity pluginId)
        {
            return new PackageIdentity(pluginId.Id, NuGetVersion.Parse(pluginId.Version));
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
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="pluginLocation">Pathname of the plugin folder.</param>
        /// <param name="context">The context.</param>
        /// <param name="packageId">Identifier for the package being installed.</param>
        /// <param name="nugetFramework">The nuget framework.</param>
        /// <param name="frameworkReducer">The framework reducer.</param>
        /// <param name="packageReader">The package reader.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task<IOperationResult> InstallBinAsync(AppIdentity pluginIdentity, string pluginLocation, IPluginContext context, PackageIdentity packageId, NuGetFramework nugetFramework, FrameworkReducer frameworkReducer, PackageReaderBase packageReader, CancellationToken cancellationToken)
        {
            const string libFolderName = "lib";

            var libItems = packageReader.GetLibItems();
            var nearestLibItemFwk = frameworkReducer.GetNearest(nugetFramework, libItems.Select(x => x.TargetFramework));

            var libItem = libItems.FirstOrDefault(l => l.TargetFramework == nearestLibItemFwk);
            if (!(libItem?.HasEmptyFolder ?? true))
            {
                await packageReader.CopyFilesAsync(pluginLocation, libItem.Items, (src, target, stream) => this.ExtractPackageFile(src, target, libFolderName, flatten: true), this.nativeLogger, cancellationToken).PreserveThreadContext();
            }

            var libFolder = Path.Combine(pluginLocation, libFolderName);
            if (Directory.Exists(libFolder))
            {
                Directory.Delete(libFolder, recursive: true);
            }

            return new OperationResult().MergeMessage($"Binaries of {packageId} installed successfully.");
        }

        /// <summary>
        /// Installs the plugin content items asynchronously.
        /// </summary>
        /// <param name="pluginIdentity">The plugin identity.</param>
        /// <param name="pluginLocation">Pathname of the plugin folder.</param>
        /// <param name="context">The context.</param>
        /// <param name="packageId">Identifier for the package being installed.</param>
        /// <param name="nugetFramework">The nuget framework.</param>
        /// <param name="frameworkReducer">The framework reducer.</param>
        /// <param name="packageReader">The package reader.</param>
        /// <param name="cancellationToken">A token that allows processing to be cancelled.</param>
        /// <returns>
        /// An asynchronous result.
        /// </returns>
        protected virtual async Task<IOperationResult> InstallContentAsync(AppIdentity pluginIdentity, string pluginLocation, IPluginContext context, PackageIdentity packageId, NuGetFramework nugetFramework, FrameworkReducer frameworkReducer, PackageReaderBase packageReader, CancellationToken cancellationToken)
        {
            const string contentFolderName = "content";

            var contentItems = packageReader.GetContentItems();
            var nearestLibItemFwk = frameworkReducer.GetNearest(nugetFramework, contentItems.Select(x => x.TargetFramework));

            var result = new OperationResult();
            var contentItem = contentItems.FirstOrDefault(l => l.TargetFramework == nearestLibItemFwk);
            if (!(contentItem?.HasEmptyFolder ?? true))
            {
                var copiedFiles = await packageReader.CopyFilesAsync(pluginLocation, contentItem.Items, (src, target, stream) => this.ExtractPackageFile(src, target, contentFolderName, flatten: false), this.nativeLogger, cancellationToken).PreserveThreadContext();
                result.MergeMessage($"Copied {copiedFiles.Count()} files into {pluginLocation}: '{string.Join("', '", copiedFiles)}'.");
            }

            var contentFolder = Path.Combine(pluginLocation, contentFolderName);
            if (Directory.Exists(contentFolder))
            {
                Directory.Delete(contentFolder, recursive: true);
                result.MergeMessage($"Deleted non-empty directory {contentFolder}.");
            }

            return result.MergeMessage($"Content of {packageId} installed successfully.");
        }

        private async Task<(PackageIdentity pluginPackageIdentity, IList<(PackageIdentity packageId, PackageReaderBase packageReader)> packageReaders)> GetPackageReadersAsync(AppIdentity plugin, IList<SourceRepository> repositories, SourceCacheContext cacheContext, NuGetFramework nugetFramework, CancellationToken cancellationToken)
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

        private async Task<List<(PackageIdentity packageId, PackageReaderBase packageReader)>> GetPackageReadersAsync(IList<SourceRepository> repositories, SourceCacheContext cacheContext, string packagesFolder, IEnumerable<SourcePackageDependencyInfo> dependenciesToInstall, CancellationToken cancellationToken)
        {
            var downloadContext = new PackageDownloadContext(cacheContext);
            var downloadResources = await this.GetDownloadResourcesAsync(repositories, cancellationToken).PreserveThreadContext();

            var packageReaders = new List<(PackageIdentity packageId, PackageReaderBase packageReader)>();

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var dependency in dependenciesToInstall)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var packagePathResolver = this.GetPackagePathResolver();
                var installedPath = packagePathResolver.GetInstalledPath(dependency);
                if (installedPath != null)
                {
                    packageReaders.Add((dependency, new PackageFolderReader(installedPath)));
                }
                else
                {
                    var downloadResult = await this.DownloadPackageAsync(
                            dependency,
                            packagesFolder,
                            downloadResources,
                            downloadContext,
                            cancellationToken).PreserveThreadContext();
                    packageReaders.Add((dependency, downloadResult.PackageReader));
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
