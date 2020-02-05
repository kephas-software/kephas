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
        public void GetInstalledPlugins()
        {
            using (var ctx = new PluginsTestContext())
            {
                var pluginManager = this.CreatePluginManager(ctx);

                var plugins = pluginManager.GetInstalledPlugins();
            }
        }

        private IPluginManager CreatePluginManager(PluginsTestContext context)
        {
            var pluginsDataService = new TestPluginDataService();
            var appRuntime = new PluginsAppRuntime(appFolder: context.AppLocation, pluginsFolder: context.PluginsFolder, pluginDataService: pluginsDataService);
            return new TestPluginManager(
                appRuntime,
                this.CreateContextFactoryMock(() => new PluginContext(Substitute.For<ICompositionContext>())),
                this.CreateEventHubMock(),
                pluginsDataService);
        }

        public class TestPluginDataService : IPluginDataService
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

            public void SetPluginData(string pluginLocation, PluginState state, string version)
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
            public TestPluginManager(IAppRuntime appRuntime, IContextFactory contextFactory, IEventHub eventHub, IPluginDataService pluginDataService, ILogManager logManager = null)
                : base(appRuntime, contextFactory, eventHub, pluginDataService, logManager)
            {
            }

            public override Task<IOperationResult<IEnumerable<IPluginInfo>>> GetAvailablePluginsAsync(Action<ISearchContext> filter = null, CancellationToken cancellationToken = default)
            {
                var plugin1 = Substitute.For<IPluginInfo>();
                var plugin2 = Substitute.For<IPluginInfo>();

                return Task.FromResult<IOperationResult<IEnumerable<IPluginInfo>>>(new OperationResult<IEnumerable<IPluginInfo>>(new[] { plugin1, plugin2 }));
            }

            protected override Task<IOperationResult<IPlugin>> InstallPluginCoreAsync(AppIdentity pluginId, IPluginContext context, CancellationToken cancellationToken = default)
            {
                var plugin = Substitute.For<IPlugin>();
                return Task.FromResult<IOperationResult<IPlugin>>(new OperationResult<IPlugin>(plugin));
            }
        }

        private class PluginsTestContext : IDisposable
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
