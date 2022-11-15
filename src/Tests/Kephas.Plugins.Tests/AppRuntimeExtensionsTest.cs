// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppRuntimeExtensionsTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the application runtime plugins extensions test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Tests
{
    using System;
    using System.IO;
    using System.Linq;

    using Kephas.Dynamic;
    using Kephas.Plugins.Application;
    using NUnit.Framework;

    [TestFixture]
    public class AppRuntimeExtensionsTest
    {
        [Test]
        public void GetPluginsLocation_default()
        {
            var appLocation = Path.GetFullPath("/");
            var appRuntime = new PluginsAppRuntime(appFolder: appLocation);
            var pluginFolder = appRuntime.GetPluginsLocation();

            Assert.AreEqual(Path.Combine(appLocation, PluginsAppRuntime.DefaultPluginsFolder), pluginFolder);
        }

        [Test]
        public void GetPluginsLocation_configured()
        {
            var appLocation = Path.GetFullPath("/");
            var pluginsLocation = Path.Combine(appLocation, "my", "plugins");
            var appRuntime = new PluginsAppRuntime(appFolder: appLocation, appArgs: new Expando { [PluginsAppRuntime.PluginsFolderArgName] = pluginsLocation });
            var pluginFolder = appRuntime.GetPluginsLocation();

            Assert.AreEqual(pluginsLocation, pluginFolder);
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
            var pluginFolders = appRuntime.GetPluginsInstallationLocations().ToList();

            var binFolder = appRuntime.GetAppLocation();
            Assert.AreEqual(Path.Combine(binFolder, "myPlugins", "p1"), pluginFolders[0]);
            Assert.AreEqual(Path.Combine(binFolder, "myPlugins", "p2"), pluginFolders[1]);

            Directory.Delete(appLocation, recursive: true);
        }
    }
}
