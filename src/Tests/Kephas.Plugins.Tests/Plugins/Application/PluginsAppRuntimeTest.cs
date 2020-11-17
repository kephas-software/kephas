// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsAppRuntimeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugins application runtime test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Kephas.Tests.Plugins.Application
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.IO;
    using Kephas.Plugins;
    using Kephas.Plugins.Application;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class PluginsAppRuntimeTest
    {
        [Test]
        public void EnablePlugins_default()
        {
            var appRuntime = new PluginsAppRuntime();
            Assert.IsTrue(appRuntime.EnablePlugins);
        }

        [Test]
        public void EnablePlugins_arg()
        {
            var appRuntime = new PluginsAppRuntime(enablePlugins: false, appArgs: new Expando { [nameof(PluginsAppRuntime.EnablePlugins)] = true });
            Assert.IsFalse(appRuntime.EnablePlugins);
        }

        [Test]
        public void EnablePlugins_appArgs()
        {
            var appRuntime = new PluginsAppRuntime(appArgs: new Expando { [nameof(PluginsAppRuntime.EnablePlugins)] = false });
            Assert.IsFalse(appRuntime.EnablePlugins);
        }

        [Test]
        public void PluginsLocation_default()
        {
            var tempFolder = Path.GetFullPath(Path.GetTempPath());
            var appLocation = Path.Combine(tempFolder, "_unit_test_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(appLocation);
            var pluginsFolder = Path.Combine(appLocation, "myPlugins");

            var appRuntime = new PluginsAppRuntime(appFolder: appLocation, pluginsFolder: "myPlugins");
            Assert.AreEqual(pluginsFolder, appRuntime.PluginsLocation);

            Directory.Delete(appLocation, recursive: true);
        }

        [Test]
        public void PluginsLocation_appArgs()
        {
            var tempFolder = Path.GetFullPath(Path.GetTempPath());
            var appLocation = Path.Combine(tempFolder, "_unit_test_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(appLocation);
            var pluginsFolder = Path.Combine(appLocation, "myPlugins");

            var appRuntime = new PluginsAppRuntime(appFolder: appLocation, appArgs: new Expando { [PluginsAppRuntime.PluginsFolderArgName] = "myPlugins" });
            Assert.AreEqual(pluginsFolder, appRuntime.PluginsLocation);

            Directory.Delete(appLocation, recursive: true);
        }

        [Test]
        public void GetAppBinLocations()
        {
            var pluginRepository = new TestPluginRepository();

            var tempFolder = Path.GetFullPath(Path.GetTempPath());
            var appLocation = Path.Combine(tempFolder, "_unit_test_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(appLocation);
            var pluginsFolder = Path.Combine(appLocation, "myPlugins");
            Directory.CreateDirectory(pluginsFolder);
            var plugin1Location = Path.Combine(pluginsFolder, "p1");
            Directory.CreateDirectory(plugin1Location);
            pluginRepository.StorePluginData(new PluginData(new AppIdentity("p1"), PluginState.Enabled));
            var plugin2Location = Path.Combine(pluginsFolder, "p2");
            Directory.CreateDirectory(plugin2Location);

            var appRuntime = new PluginsAppRuntime(appFolder: appLocation, pluginsFolder: "myPlugins", pluginRepository: pluginRepository);
            var binFolders = appRuntime.GetAppBinLocations().ToList();

            var binFolder = appRuntime.GetAppLocation();
            Assert.AreEqual(2, binFolders.Count);
            Assert.AreEqual(binFolder, binFolders[0]);
            Assert.AreEqual(Path.Combine(binFolder, "myPlugins", "p1"), binFolders[1]);

            Directory.Delete(appLocation, recursive: true);
        }

        [Test]
        public void LoadAssemblyFromName_common_dependencies()
        {
            var pluginRepository = new TestPluginRepository();

            var thisFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#if NET461
            var thisAppLocation = Path.Combine(thisFolder, "net461");
#else
            var thisAppLocation = Path.Combine(thisFolder, "netstandard2.0");
#endif

            var tempFolder = Path.GetFullPath(Path.GetTempPath());
            var appLocation = Path.Combine(tempFolder, "_unit_test_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(appLocation, "PluginsFolder"));
            FileSystem.CopyDirectory(Path.Combine(thisAppLocation, "PluginsFolder"), Path.Combine(appLocation, "PluginsFolder"));

            pluginRepository.StorePluginData(new PluginData(new AppIdentity("TestConsumerLibrary1"), PluginState.Enabled));
            pluginRepository.StorePluginData(new PluginData(new AppIdentity("TestConsumerLibrary2"), PluginState.Enabled));

            using (var appRuntime = new PluginsAppRuntime(appFolder: appLocation, pluginsFolder: "PluginsFolder", pluginRepository: pluginRepository))
            {
                ServiceHelper.Initialize(appRuntime);

                var binFolders = appRuntime.GetAppBinLocations().ToList();

                var binFolder = appRuntime.GetAppLocation();
                Assert.AreEqual(3, binFolders.Count);
                Assert.AreEqual(binFolder, binFolders[0]);

                var appAssemblies = appRuntime.GetAppAssemblies().OrderBy(a => a.FullName).ToList();
                Assert.AreEqual(1, appAssemblies.Count(a => a.GetName().Name == "TestClassLibrary"));
                Assert.AreEqual(1, appAssemblies.Count(a => a.GetName().Name == "TestConsumerLibrary1"));
                Assert.AreEqual(1, appAssemblies.Count(a => a.GetName().Name == "TestConsumerLibrary2"));
            }

            // TODO cannot delete the directories anymore, the assemblies are loaded.
            // Directory.Delete(appLocation, recursive: true);
        }

        [Test]
        public void GetPluginsInstallationLocations()
        {
            var tempFolder = Path.GetFullPath(Path.GetTempPath());
            var appLocation = Path.Combine(tempFolder, "_unit_test_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(appLocation);
            var pluginsFolder = Path.Combine(appLocation, "myPlugins");
            Directory.CreateDirectory(pluginsFolder);
            var plugin1Location = Path.Combine(pluginsFolder, "p1");
            Directory.CreateDirectory(plugin1Location);
            var plugin2Location = Path.Combine(pluginsFolder, "p2");
            Directory.CreateDirectory(plugin2Location);

            var appRuntime = new PluginsAppRuntime(appFolder: appLocation, pluginsFolder: "myPlugins");
            var binFolders = appRuntime.GetPluginsInstallationLocations().ToList();

            var binFolder = appRuntime.GetAppLocation();
            Assert.AreEqual(2, binFolders.Count);
            Assert.AreEqual(Path.Combine(binFolder, "myPlugins", "p1"), binFolders[0]);
            Assert.AreEqual(Path.Combine(binFolder, "myPlugins", "p2"), binFolders[1]);

            Directory.Delete(appLocation, recursive: true);
        }

        [Test]
        public void GetPluginsLocation_default_plugins_folder()
        {
            var appLocation = Path.GetFullPath("/");
            var appRuntime = new PluginsAppRuntime(appFolder: appLocation);

            var pluginsFolder = Path.Combine(appLocation, "Plugins");
            Assert.AreEqual(pluginsFolder, appRuntime.PluginsLocation);
            Assert.AreEqual(pluginsFolder, appRuntime.GetPluginsLocation());
        }

        [Test]
        public void GetPluginsLocation_custom_plugins_folder()
        {
            var appLocation = Path.GetFullPath("/");
            var appRuntime = new PluginsAppRuntime(appFolder: appLocation, pluginsFolder: "my/folder");

            var pluginsFolder = Path.Combine(appLocation, "my", "folder");
            Assert.AreEqual(pluginsFolder, appRuntime.PluginsLocation);
            Assert.AreEqual(pluginsFolder, appRuntime.GetPluginsLocation());
        }

        [Test]
        public void GetPluginsLocation_custom_plugins_folder_relative_path()
        {
            var appLocation = Path.GetFullPath("/one/two");
            var appRuntime = new PluginsAppRuntime(appFolder: appLocation, pluginsFolder: "../folder");

            var pluginsFolder = Path.GetFullPath(Path.Combine(appLocation, "..", "folder"));
            Assert.AreEqual(pluginsFolder, appRuntime.PluginsLocation);
            Assert.AreEqual(pluginsFolder, appRuntime.GetPluginsLocation());
        }

        public class TestPluginRepository : IPluginRepository
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
        }
    }
}
