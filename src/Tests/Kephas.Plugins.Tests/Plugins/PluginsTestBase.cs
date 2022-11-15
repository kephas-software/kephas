// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsTestBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;
using Kephas.Plugins.Application;
using Kephas.Plugins.Reflection;

namespace Kephas.Tests.Plugins
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Application.Reflection;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Plugins;
    using Kephas.Services;
    using Kephas.Testing;
    using Kephas.Testing.Services;
    using NSubstitute;

    public abstract class PluginsTestBase : TestBase
    {
        protected TestPluginManager CreatePluginManager(
            PluginsTestContext context,
            Action<IPlugin, IPluginContext>? onInstall = null,
            Func<PluginData, IPluginContext, bool>? canInstall = null,
            Func<PluginData, IPluginContext, bool>? canUninstall = null,
            Func<PluginData, IPluginContext, bool>? canInitialize = null,
            Func<PluginData, IPluginContext, bool>? canUninitialize = null,
            Func<PluginData, IPluginContext, bool>? canEnable = null,
            Func<PluginData, IPluginContext, bool>? canDisable = null)
        {
            var pluginsDataStore = new TestPluginStore();
            var appRuntime = new PluginsAppRuntime(new PluginsAppRuntimeSettings
            {
                AppFolder = context.AppLocation,
                PluginsFolder = context.PluginsFolder,
                PluginRepository = pluginsDataStore,
            });
            return new TestPluginManager(
                context,
                appRuntime,
                this.CreateInjectableFactoryMock(() => new PluginContext(Substitute.For<IServiceProvider>())),
                this.CreateEventHubMock(),
                pluginsDataStore,
                onInstall: onInstall,
                canInstall: canInstall,
                canUninstall: canUninstall,
                canInitialize: canInitialize,
                canUninitialize: canUninitialize,
                canEnable: canEnable,
                canDisable: canDisable);
        }

        public class TestPluginStore : IPluginStore
        {
            private ConcurrentDictionary<string, PluginData> cache = new ConcurrentDictionary<string, PluginData>();

            public PluginData GetPluginData(AppIdentity pluginIdentity, bool throwOnInvalid = true)
            {
                if (this.cache.TryGetValue(pluginIdentity.Id.ToLower(), out var pluginData))
                {
                    return pluginData;
                }

                return new PluginData(pluginIdentity, PluginState.None);
            }

            public void StorePluginData(PluginData pluginData)
            {
                this.cache.AddOrUpdate(pluginData.Identity.Id.ToLower(), pluginData, (_, __) => pluginData);
            }

            public void RemovePluginData(PluginData pluginData)
            {
                this.cache.TryRemove(pluginData.Identity.Id.ToLower(), out _);
            }

            public IEnumerable<PluginData> GetInstalledPlugins() => this.cache.Values;
        }

        public class TestPluginManager : PluginManagerBase
        {
            private readonly PluginsTestContext ctx;
            private readonly Action<IPlugin, IPluginContext>? onInstall;
            private readonly Func<PluginData, IPluginContext, bool>? canInstall;
            private readonly Func<PluginData, IPluginContext, bool>? canUninstall;
            private readonly Func<PluginData, IPluginContext, bool>? canInitialize;
            private readonly Func<PluginData, IPluginContext, bool>? canUninitialize;
            private readonly Func<PluginData, IPluginContext, bool>? canEnable;
            private readonly Func<PluginData, IPluginContext, bool>? canDisable;

            public TestPluginManager(
                PluginsTestContext ctx,
                IAppRuntime appRuntime,
                IInjectableFactory injectableFactory,
                IEventHub eventHub,
                IPluginStore pluginStore,
                ILogManager? logManager = null,
                Action<IPlugin, IPluginContext>? onInstall = null,
                Func<PluginData, IPluginContext, bool>? canInstall = null,
                Func<PluginData, IPluginContext, bool>? canUninstall = null,
                Func<PluginData, IPluginContext, bool>? canInitialize = null,
                Func<PluginData, IPluginContext, bool>? canUninitialize = null,
                Func<PluginData, IPluginContext, bool>? canEnable = null,
                Func<PluginData, IPluginContext, bool>? canDisable = null)
                : base(appRuntime, injectableFactory, eventHub, pluginStore, logManager)
            {
                this.ctx = ctx;
                this.onInstall = onInstall;
                this.canInstall = canInstall;
                this.canUninstall = canUninstall;
                this.canInitialize = canInitialize;
                this.canUninitialize = canUninitialize;
                this.canEnable = canEnable;
                this.canDisable = canDisable;
            }

            public override IEnumerable<IPlugin> GetInstalledPlugins()
            {
                var repo = this.PluginStore as TestPluginStore;
                if (repo == null)
                {
                    return base.GetInstalledPlugins();
                }

                return repo.GetInstalledPlugins().Select(d =>
                    new Plugin(new PluginInfo(Substitute.For<IAppRuntime>(), repo, d.Identity), d));
            }

            public override Task<IOperationResult<IEnumerable<IAppInfo>>> GetAvailablePluginsAsync(Action<ISearchContext>? filter = null, CancellationToken cancellationToken = default)
            {
                var plugin1 = Substitute.For<IAppInfo>();
                var plugin2 = Substitute.For<IAppInfo>();

                return Task.FromResult<IOperationResult<IEnumerable<IAppInfo>>>(new OperationResult<IEnumerable<IAppInfo>>(new[] { plugin1, plugin2 }));
            }

            protected override Task<IOperationResult<IPlugin>> InstallPluginCoreAsync(AppIdentity pluginId, IPluginContext context, CancellationToken cancellationToken = default)
            {
                var pluginInfo = Substitute.For<IAppInfo>();
                pluginInfo.Identity.Returns(pluginId);
                pluginInfo.Name.Returns(pluginId.Id);

                var plugin = Substitute.For<IPlugin>();
                plugin.Id.Returns(pluginId.Id);
                plugin.Identity.Returns(pluginId);
                plugin.GetTypeInfo().Returns(pluginInfo);
                plugin.Location.Returns(Path.Combine(this.ctx.PluginsLocation, pluginId.Id));
                var pluginData = context?.PluginData ?? new PluginData(pluginId, PluginState.None);
                plugin.GetPluginData().Returns(pluginData);
                plugin.State.Returns(ci => pluginData.State);
                this.onInstall?.Invoke(plugin, context);
                return Task.FromResult<IOperationResult<IPlugin>>(new OperationResult<IPlugin>(plugin));
            }

            protected override bool CanInstallPlugin(PluginData pluginData, IPluginContext context)
            {
                return (this.canInstall?.Invoke(pluginData, context) ?? true)
                    && base.CanInstallPlugin(pluginData, context);
            }

            protected override bool CanUninstallPlugin(PluginData pluginData, IPluginContext context)
            {
                return (this.canUninstall?.Invoke(pluginData, context) ?? true)
                    && base.CanUninstallPlugin(pluginData, context);
            }

            protected override bool CanInitializePlugin(PluginData pluginData, IPluginContext context)
            {
                return (this.canInitialize?.Invoke(pluginData, context) ?? true)
                    && base.CanInitializePlugin(pluginData, context);
            }

            protected override bool CanUninitializePlugin(PluginData pluginData, IPluginContext context)
            {
                return (this.canUninitialize?.Invoke(pluginData, context) ?? true)
                    && base.CanUninitializePlugin(pluginData, context);
            }

            protected override bool CanEnablePlugin(PluginData pluginData, IPluginContext context)
            {
                return (this.canEnable?.Invoke(pluginData, context) ?? true)
                    && base.CanEnablePlugin(pluginData, context);
            }

            protected override bool CanDisablePlugin(PluginData pluginData, IPluginContext context)
            {
                return (this.canDisable?.Invoke(pluginData, context) ?? true)
                    && base.CanDisablePlugin(pluginData, context);
            }

            protected override Task<IOperationResult> DownloadPluginCoreAsync(AppIdentity pluginIdentity, IPluginContext context, CancellationToken cancellationToken)
            {
                return Task.FromResult<IOperationResult>(new OperationResult());
            }
        }

        public class PluginsTestContext : IDisposable
        {
            public PluginsTestContext(string? pluginsFolder = null)
            {
                var tempFolder = Path.GetFullPath(Path.GetTempPath());
                this.AppLocation = Path.Combine(tempFolder, "_unit_test_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(this.AppLocation);
                this.PluginsFolder = pluginsFolder ?? "myPlugins";
                this.PluginsLocation = Path.Combine(this.AppLocation, this.PluginsFolder);
                Directory.CreateDirectory(this.PluginsLocation);
            }

            public string AppLocation { get; }

            public string PluginsFolder { get; }

            public string PluginsLocation { get; }

            public void Dispose()
            {
                Directory.Delete(this.AppLocation, recursive: true);
            }
        }
    }
}