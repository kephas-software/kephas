// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginsAppRuntimeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the plugins application runtime test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Plugins.Tests.Application
{
    using System;
    using System.IO;
    using System.Linq;
    using Kephas.Plugins.Application;
    using NUnit.Framework;

    [TestFixture]
    public class PluginsAppRuntimeTest
    {
        [Test]
        public void GetAppBinDirectories()
        {
            var tempFolder = Path.GetFullPath(Path.GetTempPath());
            var appLocation = Path.Combine(tempFolder, "_unit_test_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(appLocation);
            var pluginsFolder = Path.Combine(appLocation, "myPlugins");
            Directory.CreateDirectory(pluginsFolder);
            var plugin1Location = Path.Combine(pluginsFolder, "p1");
            Directory.CreateDirectory(plugin1Location);
            File.WriteAllText(Path.Combine(plugin1Location, PluginHelper.PluginStateFileName), PluginState.Enabled.ToString());
            var plugin2Location = Path.Combine(pluginsFolder, "p2");
            Directory.CreateDirectory(plugin2Location);

            var appRuntime = new PluginsAppRuntime(appLocation: appLocation, pluginsFolder: "myPlugins");
            var binFolders = appRuntime.GetAppBinDirectories().ToList();

            var binFolder = appRuntime.GetAppLocation();
            Assert.AreEqual(2, binFolders.Count);
            Assert.AreEqual(binFolder, binFolders[0]);
            Assert.AreEqual(Path.Combine(binFolder, "myPlugins", "p1"), binFolders[1]);

            Directory.Delete(appLocation, recursive: true);
        }

        [Test]
        public void EnumeratePluginLocations()
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

            var appRuntime = new PluginsAppRuntime(appLocation: appLocation, pluginsFolder: "myPlugins");
            var binFolders = appRuntime.EnumeratePluginLocations().ToList();

            var binFolder = appRuntime.GetAppLocation();
            Assert.AreEqual(2, binFolders.Count);
            Assert.AreEqual(Path.Combine(binFolder, "myPlugins", "p1"), binFolders[0]);
            Assert.AreEqual(Path.Combine(binFolder, "myPlugins", "p2"), binFolders[1]);

            Directory.Delete(appLocation, recursive: true);
        }
    }
}
