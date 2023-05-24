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
    using Kephas.Configuration;
    using Kephas.Configuration.Providers;
    using Kephas.Diagnostics.Logging;
    using Kephas.Logging;
    using Kephas.Plugins.Application;
    using Kephas.Testing;
    using Kephas.Testing.Services;
    using NUnit.Framework;

    [TestFixture]
    public class NuGetPluginManagerTest : TestBase
    {
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            var assemblies = new List<Assembly>(base.GetAssemblies())
            {
                typeof(IConfiguration<>).Assembly,     // Kephas.Configuration
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
            var container = this.CreateServicesBuilder().BuildWithDependencyInjection();
            var manager = container.Resolve<IPluginManager>();

            Assert.IsInstanceOf<NuGetPluginManager>(manager);
        }

        [Test]
        [TestCase("kephas mailkit exchange online", "Kephas.Mail.MailKit.ExchangeOnline", "11.1.0", false)]
        [TestCase("kephas", "Kephas.Core", "11.0.0", true)]
        public async Task InstallPluginAsync(string tags, string packageId, string packageVersion, bool hasResources)
        {
            var tempFolder = Path.GetTempPath();
            var pluginsFolder = Path.Combine(tempFolder, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(pluginsFolder);

            try
            {
                var appServices = new AppServiceCollection()
                    .Add<ISettingsProvider>(
                        _ => new PluginsSettingsProvider($"tags:{tags}"),
                        b => b.Singleton().AllowMultiple());
                var container = this.CreateServicesBuilder(appServices)
                    .WithAppRuntime(this.CreateAppRuntime(new DebugLogManager(), pluginsFolder))
                    .BuildWithDependencyInjection();
                var manager = container.Resolve<IPluginManager>();

                var result = await manager.InstallPluginAsync(new AppIdentity(packageId, packageVersion));

                var pluginData = result.Value;
                Assert.AreEqual(new AppIdentity(packageId, packageVersion), pluginData.Identity);

                var pluginLocation = Path.Combine(pluginsFolder, packageId);
                Assert.AreEqual(pluginLocation, pluginData.Location);
                Assert.AreEqual(PluginState.Enabled, pluginData.State);

                Assert.IsTrue(File.Exists(Path.Combine(pluginLocation, $"{packageId}.dll")), $"File {packageId}.dll does not exist.");
                if (hasResources)
                {
                    Assert.IsTrue(File.Exists(Path.Combine(pluginLocation, "de-DE", $"{packageId}.resources.dll")), $"File de-DE/{packageId}.resources.dll does not exist.");
                }
            }
            finally
            {
                Directory.Delete(pluginsFolder, recursive: true);
            }
        }

        [Test]
        [TestCase("kismsspplugin", "Kis.Logging.Seq", "4.0.0", true)]
        public async Task InstallPluginAsync_with_contentFiles(string tags, string packageId, string packageVersion, bool hasResources)
        {
            var tempFolder = Path.GetTempPath();
            var pluginsFolder = Path.Combine(tempFolder, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(pluginsFolder);

            try
            {
                var appServices = new AppServiceCollection()
                    .Add<ISettingsProvider>(
                        _ => new PluginsSettingsProvider($"tags:{tags}"),
                        b => b.Singleton().AllowMultiple());
                var container = this.CreateServicesBuilder(appServices)
                    .WithAppRuntime(this.CreateAppRuntime(new DebugLogManager(), pluginsFolder))
                    .BuildWithDependencyInjection();
                var manager = container.Resolve<IPluginManager>();

                var result = await manager.InstallPluginAsync(new AppIdentity(packageId, packageVersion));

                var pluginData = result.Value;
                Assert.AreEqual(new AppIdentity(packageId, packageVersion), pluginData.Identity);

                var pluginLocation = Path.Combine(pluginsFolder, packageId);
                Assert.AreEqual(pluginLocation, pluginData.Location);
                Assert.AreEqual(PluginState.Enabled, pluginData.State);

                Assert.IsTrue(File.Exists(Path.Combine(pluginLocation, $"{packageId}.dll")), $"File {packageId}.dll does not exist.");
                Assert.IsTrue(File.Exists(Path.Combine(pluginLocation, "Config", "logSettings.json")), "File Config/logSettings.json does not exist.");
            }
            finally
            {
                Directory.Delete(pluginsFolder, recursive: true);
            }
        }

        private PluginsAppRuntime CreateAppRuntime(ILogManager logManager, string? pluginsPath = null)
        {
            var appRuntime = new PluginsAppRuntime(new PluginsAppRuntimeSettings
                {
                    GetLogger = logManager.GetLogger,
                    PluginsFolder = pluginsPath,
                    IsAppAssembly = this.IsNotTestAssembly,
                });
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
