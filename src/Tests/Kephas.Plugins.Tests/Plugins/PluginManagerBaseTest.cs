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
                var pluginManager = this.CreatePluginManager(ctx);

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

        private IPluginManager CreatePluginManager(PluginsTestContext context)
        {
            var pluginsDataStore = new TestPluginDataStore();
            var appRuntime = new PluginsAppRuntime(appFolder: context.AppLocation, pluginsFolder: context.PluginsFolder, pluginDataStore: pluginsDataStore);
            return new TestPluginManager(
                context,
                appRuntime,
                this.CreateContextFactoryMock(() => new PluginContext(Substitute.For<ICompositionContext>())),
                this.CreateEventHubMock(),
                pluginsDataStore);
        }

        public class TestPluginDataStore : IPluginDataStore
        {
            private ConcurrentDictionary<string, PluginData> cache = new ConcurrentDictionary<string, PluginData>();

            public (PluginState state, string version) GetPluginData(string pluginLocation)
            {
                if (this.cache.TryGetValue(pluginLocation, out var pluginData))
                {
                    return (pluginData.State, pluginData.Version);
                }

                return (PluginState.None, null);
            }

            public void StorePluginData(string pluginLocation, PluginState state, string version)
            {
                this.cache.AddOrUpdate(pluginLocation, new PluginData { State = state, Version = version }, (_, __) => new PluginData { State = state, Version = version });
            }

            private class PluginData
            {
                public PluginState State { get; set; }

                public string Version { get; set; }
            }
        }

        public class TestPluginManager : PluginManagerBase
        {
            private readonly PluginsTestContext ctx;

            public TestPluginManager(PluginsTestContext ctx, IAppRuntime appRuntime, IContextFactory contextFactory, IEventHub eventHub, IPluginDataStore pluginDataStore, ILogManager logManager = null)
                : base(appRuntime, contextFactory, eventHub, pluginDataStore, logManager)
            {
                this.ctx = ctx;
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
                pluginInfo.GetIdentity().Returns(pluginId);
                pluginInfo.Name.Returns(pluginId.Id);
                pluginInfo.Version.Returns(pluginId.Version);
                var plugin = Substitute.For<IPlugin>();
                plugin.Id.Returns(pluginId.Id);
                plugin.GetTypeInfo().Returns(pluginInfo);
                plugin.Location.Returns(Path.Combine(this.ctx.PluginsLocation, pluginId.Id));
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
