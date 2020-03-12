// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlugnManagerBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugn manager base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using Kephas.Composition;
    using Kephas.Interaction;
    using Kephas.Logging;
    using Kephas.Operations;
    using Kephas.Plugins;
    using Kephas.Plugins.Application;
    using Kephas.Plugins.Reflection;
    using Kephas.Plugins.Transactions;
    using Kephas.Services;
    using Kephas.Testing.Composition;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class PluginManagerBaseTest : CompositionTestBase
    {
        [Test]
        public void GetInstalledPlugins_empty()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(ctx);

                var plugins = pluginManager.GetInstalledPlugins();

                CollectionAssert.IsEmpty(plugins);
            }
        }

        [Test]
        public async Task InstallPluginAsync_p1()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(ctx, (p, pctx) => pctx.Transaction.AddCommand(new TestUndoCommand("h:i", "param|1", "param\n2")));

                var result = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4"));

                Assert.IsNotNull(result);
                Assert.AreEqual(PluginState.Enabled, result.ReturnValue.State);
                Assert.AreEqual("p1:1.2.3.4\nEnabled\nEmbedded\n-inst-undo-1:test|h:i|param&pipe;1|param\\n2\n139272387", result.ReturnValue.GetPluginData().ToString());
            }
        }

        [Test]
        public async Task InstallPluginAsync_p1_cannot_install()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(
                    ctx,
                    canInstall: (d, c) => false);

                Assert.ThrowsAsync<PluginOperationException>(() => pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4")));
            }
        }

        [Test]
        public async Task InstallPluginAsync_p1_cannot_initialize()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(
                    ctx,
                    canInitialize: (d, c) => false);

                var result = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4"));

                Assert.IsNotNull(result);
                Assert.AreEqual(PluginState.PendingInitialization, result.ReturnValue.State);
                Assert.AreEqual("p1:1.2.3.4\nPendingInitialization\nEmbedded\n\n-374817233", result.ReturnValue.GetPluginData().ToString());
            }
        }

        [Test]
        public async Task InstallPluginAsync_p1_cannot_enable()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(
                    ctx,
                    canEnable: (d, c) => false);

                var result = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4"));

                Assert.IsNotNull(result);
                Assert.AreEqual(PluginState.Disabled, result.ReturnValue.State);
                Assert.AreEqual("p1:1.2.3.4\nDisabled\nEmbedded\n\n-237160451", result.ReturnValue.GetPluginData().ToString());
            }
        }

        [Test]
        public async Task UninstallPluginAsync_p1()
        {
            using (var ctx = new PluginsTestContext())
            {
                var callbackCalls = 0;
                var pluginManager = this.CreatePluginManager(ctx, (p, pctx) => pctx.Transaction.AddCommand(new TestUndoCommand("h:i", "param|1", "param\n2")));

                var instResult = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4"));
                var uninstResult = await pluginManager.UninstallPluginAsync(new AppIdentity("p1"), pctx => pctx["callback"] = (Action)(() => callbackCalls++));

                Assert.IsNotNull(uninstResult);
                Assert.AreEqual(PluginState.None, uninstResult.ReturnValue.State);
                Assert.AreEqual("p1:1.2.3.4\nNone\nEmbedded\n\n-1166008255", uninstResult.ReturnValue.GetPluginData().ToString());
                Assert.AreEqual(1, callbackCalls);
            }
        }

        [Test]
        public async Task UninstallPluginAsync_p1_not_installed()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(
                    ctx,
                    canUninstall: (d, c) => false);

                Assert.ThrowsAsync<PluginOperationException>(() => pluginManager.UninstallPluginAsync(new AppIdentity("p1")));
            }
        }

        [Test]
        public async Task UninstallPluginAsync_p1_cannot_uninstall()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(
                    ctx,
                    canUninstall: (d, c) => false);

                var instResult = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4"));
                var uninstResult = await pluginManager.UninstallPluginAsync(new AppIdentity("p1"));

                Assert.IsNotNull(uninstResult);
                Assert.AreEqual(PluginState.PendingUninstallation, uninstResult.ReturnValue.State);
                Assert.AreEqual("p1:1.2.3.4\nPendingUninstallation\nEmbedded\n\n-858986955", uninstResult.ReturnValue.GetPluginData().ToString());
            }
        }

        [Test]
        public async Task UninstallPluginAsync_p1_cannot_uninitialize()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(
                    ctx,
                    canUninitialize: (d, c) => false);

                var instResult = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4"));
                Assert.ThrowsAsync<PluginOperationException>(() => pluginManager.UninstallPluginAsync(new AppIdentity("p1")));
            }
        }

        [Test]
        public async Task UninstallPluginAsync_p1_cannot_disable()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(
                    ctx,
                    canDisable: (d, c) => false);

                var instResult = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4"));
                Assert.ThrowsAsync<PluginOperationException>(() => pluginManager.UninstallPluginAsync(new AppIdentity("p1", "1.2.3.4")));
            }
        }

        [Test]
        public async Task UpdatePluginAsync_p1()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(ctx);

                var result = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4"));

                result = await pluginManager.UpdatePluginAsync(new AppIdentity("p1", "2.0.0.0"));

                Assert.IsNotNull(result);
                Assert.AreEqual(PluginState.Enabled, result.ReturnValue.State);
                Assert.AreEqual("p1:1.2.3.4\nEnabled\nEmbedded\n\n-1236731413", result.ReturnValue.GetPluginData().ToString());
            }
        }

        private TestPluginManager CreatePluginManager(
            PluginsTestContext context,
            Action<IPlugin, IPluginContext> onInstall = null,
            Func<PluginData, IPluginContext, bool> canInstall = null,
            Func<PluginData, IPluginContext, bool> canUninstall = null,
            Func<PluginData, IPluginContext, bool> canInitialize = null,
            Func<PluginData, IPluginContext, bool> canUninitialize = null,
            Func<PluginData, IPluginContext, bool> canEnable = null,
            Func<PluginData, IPluginContext, bool> canDisable = null)
        {
            var pluginsDataStore = new TestPluginRepository();
            var appRuntime = new PluginsAppRuntime(appFolder: context.AppLocation, pluginsFolder: context.PluginsFolder, pluginRepository: pluginsDataStore);
            return new TestPluginManager(
                context,
                appRuntime,
                this.CreateContextFactoryMock(() => new PluginContext(Substitute.For<ICompositionContext>())),
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

        public class TestUndoCommand : UndoCommandBase
        {
            public TestUndoCommand(params string[] args)
                : base("test", args)
            {
            }

            public override void Execute(IPluginContext context)
            {
                var callback = context["callback"] as Action;
                callback?.Invoke();
            }
        }

        public class TestPluginRepository : IPluginRepository
        {
            private ConcurrentDictionary<string, PluginData> cache = new ConcurrentDictionary<string, PluginData>();

            public PluginData GetPluginData(AppIdentity pluginIdentity)
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
        }

        public class TestPluginManager : PluginManagerBase
        {
            private readonly PluginsTestContext ctx;
            private readonly Action<IPlugin, IPluginContext> onInstall;
            private readonly Func<PluginData, IPluginContext, bool> canInstall;
            private readonly Func<PluginData, IPluginContext, bool> canUninstall;
            private readonly Func<PluginData, IPluginContext, bool> canInitialize;
            private readonly Func<PluginData, IPluginContext, bool> canUninitialize;
            private readonly Func<PluginData, IPluginContext, bool> canEnable;
            private readonly Func<PluginData, IPluginContext, bool> canDisable;

            public TestPluginManager(
                PluginsTestContext ctx,
                IAppRuntime appRuntime,
                IContextFactory contextFactory,
                IEventHub eventHub,
                IPluginRepository pluginRepository,
                ILogManager logManager = null,
                Action<IPlugin, IPluginContext> onInstall = null,
                Func<PluginData, IPluginContext, bool> canInstall = null,
                Func<PluginData, IPluginContext, bool> canUninstall = null,
                Func<PluginData, IPluginContext, bool> canInitialize = null,
                Func<PluginData, IPluginContext, bool> canUninitialize = null,
                Func<PluginData, IPluginContext, bool> canEnable = null,
                Func<PluginData, IPluginContext, bool> canDisable = null)
                : base(appRuntime, contextFactory, eventHub, pluginRepository, logManager)
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
            public PluginsTestContext(string pluginsFolder = null)
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
