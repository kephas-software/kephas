// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPluginManagerTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services;

namespace Kephas.Plugins.NuGet.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Plugins.Application;
    using Kephas.Testing.Composition;
    using NUnit.Framework;

    [TestFixture]
    public class NuGetPluginManagerTest : CompositionTestBase
    {
        public override IEnumerable<Assembly> GetDefaultConventionAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetDefaultConventionAssemblies())
            {
                typeof(PluginManagerBase).Assembly,     // Kephas.Plugins
                typeof(NuGetPluginManager).Assembly,    // Kephas.Plugins.NuGet
            };
            return assemblies;
        }

        protected override IAppRuntime CreateDefaultAppRuntime(ILogManager logManager) =>
            this.CreateAppRuntime(logManager);

        [Test]
        public void Injection()
        {
            var container = this.CreateContainer();
            var manager = container.Resolve<IPluginManager>();

            Assert.IsInstanceOf<NuGetPluginManager>(manager);
        }

        [Test]
        public async Task InstallPluginAsync()
        {
            var tempFolder = Path.GetTempPath();
            var pluginsFolder = Path.Combine(tempFolder, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(pluginsFolder);

            try
            {
                var container = this.CreateContainer(
                    config: b => b.WithFactory<ISettingsProvider>(
                        () => new PluginsSettingsProvider("tags:kephas"),
                        isSingleton: true,
                        allowMultiple: true),
                    appRuntime: this.CreateAppRuntime(new DebugLogManager(), pluginsFolder));
                var manager = container.Resolve<IPluginManager>();

                var result = await manager.InstallPluginAsync(new AppIdentity("Kephas.Core", "9.0.0-dev.0"));

                var pluginData = result.Value;
                Assert.AreEqual(new AppIdentity("Kephas.Core", "9.0.0-dev.0"), pluginData.Identity);

                var pluginLocation = Path.Combine(pluginsFolder, "Kephas.Core");
                Assert.AreEqual(pluginLocation, pluginData.Location);
                Assert.AreEqual(PluginState.Enabled, pluginData.State);

                Assert.IsTrue(File.Exists(Path.Combine(pluginLocation, "Kephas.Core.dll")), "File Kephas.Core.dll does not exist.");
                Assert.IsTrue(File.Exists(Path.Combine(pluginLocation, "de-DE", "Kephas.Core.resources.dll")), "File de-DE/Kephas.Core.resources.dll does not exist.");
            }
            finally
            {
                Directory.Delete(pluginsFolder, recursive: true);
            }
        }

        [Test]
        public async Task InstallPluginAsync_with_contentFiles()
        {
            var tempFolder = Path.GetTempPath();
            var pluginsFolder = Path.Combine(tempFolder, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(pluginsFolder);

            try
            {
                var container = this.CreateContainer(
                    config: b => b.WithFactory<ISettingsProvider>(
                        () => new PluginsSettingsProvider("tags:kismsspplugin"),
                        isSingleton: true,
                        allowMultiple: true),
                    appRuntime: this.CreateAppRuntime(new DebugLogManager(), pluginsFolder));
                var manager = container.Resolve<IPluginManager>();

                var result = await manager.InstallPluginAsync(new AppIdentity("Kis.Logging.Seq", "4.0.0"));

                var pluginData = result.Value;
                Assert.AreEqual(new AppIdentity("Kis.Logging.Seq", "4.0.0"), pluginData.Identity);

                var pluginLocation = Path.Combine(pluginsFolder, "Kis.Logging.Seq");
                Assert.AreEqual(pluginLocation, pluginData.Location);
                Assert.AreEqual(PluginState.Enabled, pluginData.State);

                Assert.IsTrue(File.Exists(Path.Combine(pluginLocation, "Kis.Logging.Seq.dll")), "File Kis.Logging.Seq.dll does not exist.");
                Assert.IsTrue(File.Exists(Path.Combine(pluginLocation, "Config", "logSettings.json")), "File Config/logSettings.json does not exist.");
            }
            finally
            {
                Directory.Delete(pluginsFolder, recursive: true);
            }
        }

        private PluginsAppRuntime CreateAppRuntime(ILogManager logManager, string? pluginsPath = null)
        {
            var appRuntime = new PluginsAppRuntime(
                logManager.GetLogger,
                pluginsFolder: pluginsPath,
                assemblyFilter: this.IsNotTestAssembly);
            return appRuntime;
        }

        private class PluginsSettingsProvider : ISettingsProvider
        {
            private readonly string searchTerm;

            public PluginsSettingsProvider(string searchTerm)
            {
                this.searchTerm = searchTerm;
            }

            public object? GetSettings(Type settingsType, IContext? context)
            {
                if (typeof(NuGetSettings) != settingsType)
                {
                    return null;
                }

                return new NuGetSettings
                {
                    SearchTerm = this.searchTerm,
                    NuGetConfigPath = this.GetNuGetConfigPath(),
                    PackagesFolder = Path.Combine(Path.GetTempPath(), ".test", ".packages"),
                };
            }

            private string? GetNuGetConfigPath()
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }

            public Task UpdateSettingsAsync(object settings,
                IContext? context, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
