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
            }
        }

        [Test]
        public async Task UninstallPluginAsync_p1()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(ctx);

                var instResult = await pluginManager.InstallPluginAsync(new AppIdentity("p1", "1.2.3.4"));
                var uninstResult = await pluginManager.UninstallPluginAsync(new AppIdentity("p1"));

                Assert.IsNotNull(uninstResult);
                Assert.AreEqual(PluginState.None, uninstResult.ReturnValue.State);
            }
        }

        private IPluginManager CreatePluginManager(PluginsTestContext context, Action<IPlugin, IPluginContext> onInstall = null)
        {
            var pluginsDataStore = new TestPluginRepository();
            var appRuntime = new PluginsAppRuntime(appFolder: context.AppLocation, pluginsFolder: context.PluginsFolder, pluginRepository: pluginsDataStore);
            return new TestPluginManager(
                context,
                appRuntime,
                this.CreateContextFactoryMock(() => new PluginContext(Substitute.For<ICompositionContext>())),
                this.CreateEventHubMock(),
                pluginsDataStore,
                onInstall: onInstall);
        }

        public class TestUndoCommand : UndoCommandBase
        {
            public TestUndoCommand(params string[] args)
                : base("test", args)
            {
            }

            public override void Execute(IPluginContext context)
            {
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

            public TestPluginManager(
                PluginsTestContext ctx,
                IAppRuntime appRuntime,
                IContextFactory contextFactory,
                IEventHub eventHub,
                IPluginRepository pluginRepository,
                ILogManager logManager = null,
                Action<IPlugin, IPluginContext> onInstall = null)
                : base(appRuntime, contextFactory, eventHub, pluginRepository, logManager)
            {
                this.ctx = ctx;
                this.onInstall = onInstall;
            }

            public override Task<IOperationResult<IEnumerable<IAppInfo>>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default)
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
                pluginInfo.Version.Returns(pluginId.Version);
                var plugin = Substitute.For<IPlugin>();
                plugin.Id.Returns(pluginId.Id);
                plugin.GetTypeInfo().Returns(pluginInfo);
                plugin.Location.Returns(Path.Combine(this.ctx.PluginsLocation, pluginId.Id));
                var pluginData = new PluginData(pluginId, PluginState.None);
                plugin.GetPluginData().Returns(pluginData);
                this.onInstall?.Invoke(plugin, context);
                return Task.FromResult<IOperationResult<IPlugin>>(new OperationResult<IPlugin>(plugin));
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
