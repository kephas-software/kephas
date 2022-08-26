// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticAppRuntimeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net 46 application runtime test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests.Application
{
    using System.Linq;
    using Kephas.Application;
    using Kephas.Dynamic;
    using Kephas.Reflection;
    using Kephas.Runtime;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class StaticAppRuntimeTest
    {
        [Test]
        public void Constructor_set_app_id_and_instance_id()
        {
            IAppRuntime appRuntime = new StaticAppRuntime(
                appArgs: new Expando
                {
                    [IAppRuntime.AppIdKey] = "test",
                    [IAppRuntime.AppInstanceIdKey] = "test-2",
                });
            Assert.AreEqual("test", appRuntime.GetAppId());
            Assert.AreEqual("test-2", appRuntime.GetAppInstanceId());
        }

        [Test]
        public void GetAppAssemblies_filter()
        {
            var appRuntime = new StaticAppRuntime();
            ((IInitializable)appRuntime).Initialize();
            var assemblies = appRuntime.GetAppAssemblies(n => !n.IsSystemAssembly() && !n.FullName.StartsWith("JetBrains"));
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(16, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
            Assert.AreEqual(0, assemblyList.Count(a => a.FullName.StartsWith("JetBrains")));
        }

        [Test]
        public void GetAppAssemblies_no_filter()
        {
            var appRuntime = new StaticAppRuntime();
            ((IInitializable)appRuntime).Initialize();
            var assemblies = appRuntime.GetAppAssemblies();
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(16, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
        }

        [Test]
        public void GetAppId_and_version()
        {
            IAppRuntime appRuntime = new StaticAppRuntime(appId: "hello-app", appVersion: "1.0.0-beta");

            Assert.AreEqual("hello-app", appRuntime.GetAppId());
            Assert.AreEqual("1.0.0-beta", appRuntime.GetAppVersion());
        }

        [Test]
        public void GetAppConfigLocations_default()
        {
            var appRuntime = new StaticAppRuntime(appFolder: "/root");
            var configLocations = appRuntime.GetAppConfigLocations();

            Assert.AreEqual(1, configLocations.Count());

            if (RuntimeEnvironment.IsWindows())
            {
                Assert.IsTrue(configLocations.Any(l => l.EndsWith($"\\root\\{IAppRuntime.DefaultConfigFolder}")));
            }
            else
            {
                Assert.IsTrue(configLocations.Any(l => l.EndsWith($"/root/{IAppRuntime.DefaultConfigFolder}")));
            }
        }

        [Test]
        public void GetAppConfigLocations_configured()
        {
            var appRuntime = new StaticAppRuntime(appFolder: "/root", configFolders: new[] { "../my/config", "config" });
            var configLocations = appRuntime.GetAppConfigLocations();

            Assert.AreEqual(2, configLocations.Count());

            if (RuntimeEnvironment.IsWindows())
            {
                Assert.IsTrue(configLocations.Any(l => l.EndsWith("\\my\\config") && !l.Contains("..")));
                Assert.IsTrue(configLocations.Any(l => l.EndsWith("\\root\\config")));
            }
            else
            {
                Assert.IsTrue(configLocations.Any(l => l.EndsWith("/my/config") && !l.Contains("..")));
                Assert.IsTrue(configLocations.Any(l => l.EndsWith("/root/config")));
            }
        }

        [Test]
        public void GetAppConfigLocations_configured_distinct()
        {
            var appRuntime = new StaticAppRuntime(appFolder: "/root", configFolders: new[] { "../my/config", "../my/config", "config" });
            var configLocations = appRuntime.GetAppConfigLocations();

            Assert.AreEqual(2, configLocations.Count());

            if (RuntimeEnvironment.IsWindows())
            {
                Assert.IsTrue(configLocations.Any(l => l.EndsWith("\\my\\config") && !l.Contains("..")));
                Assert.IsTrue(configLocations.Any(l => l.EndsWith("\\root\\config")));
            }
            else
            {
                Assert.IsTrue(configLocations.Any(l => l.EndsWith("/my/config") && !l.Contains("..")));
                Assert.IsTrue(configLocations.Any(l => l.EndsWith("/root/config")));
            }
        }

        [Test]
        public void IsRoot_true()
        {
            var appRuntime = new StaticAppRuntime();
            Assert.IsTrue(appRuntime.IsRoot);
        }

        [Test]
        public void IsRoot_true_with_appID()
        {
            var appRuntime = new StaticAppRuntime(isRoot: true, appId: "root");
            Assert.IsTrue(appRuntime.IsRoot);
        }

        [Test]
        public void IsRoot_false()
        {
            var appRuntime = new StaticAppRuntime(isRoot: false);
            Assert.IsFalse(appRuntime.IsRoot);
        }
    }
}