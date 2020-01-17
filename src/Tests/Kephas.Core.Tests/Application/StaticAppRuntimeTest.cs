// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAppRuntimeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net 46 application runtime test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Core.Tests.Application
{
    using System.Linq;

    using Kephas.Application;
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class StaticAppRuntimeTest
    {
        [Test]
        public void GetAppAssemblies_filter()
        {
            var appEnv = new StaticAppRuntime(assemblyLoader: new DefaultAssemblyLoader());
            var assemblies = appEnv.GetAppAssemblies(n => !n.IsSystemAssembly() && !n.FullName.StartsWith("JetBrains"));
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(2, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
            Assert.AreEqual(0, assemblyList.Count(a => a.FullName.StartsWith("JetBrains")));
        }

        [Test]
        public void GetAppAssemblies_no_filter()
        {
            var appEnv = new StaticAppRuntime(assemblyLoader: new DefaultAssemblyLoader());
            var assemblies = appEnv.GetAppAssemblies();
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(2, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
        }

        [Test]
        public void GetAppId_and_version()
        {
            var appEnv = new StaticAppRuntime(appId: "hello-app", appVersion: "1.0.0-beta");

            Assert.AreEqual("hello-app", appEnv.GetAppId());
            Assert.AreEqual("1.0.0-beta", appEnv.GetAppVersion());
        }

        [Test]
        public void GetAppFramework()
        {
            var appEnv = new StaticAppRuntime(assemblyLoader: new DefaultAssemblyLoader());
            var appFramework = appEnv.GetAppFramework();

#if NET45
            Assert.AreEqual("net45", appFramework);
#else
            Assert.AreEqual("netcoreapp1.0", appFramework);
#endif
        }
    }
}