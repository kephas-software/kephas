// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicAppRuntimeTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net 46 application runtime test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Application.Tests.Application
{
    using System.IO;
    using System.Linq;
    using Kephas.Application;
    using Kephas.Reflection;
    using Kephas.Services;
    using NUnit.Framework;

    [TestFixture]
    public class DynamicAppRuntimeTest
    {
        [Test]
        public void GetAppAssemblies_filter()
        {
            var appEnv = new DynamicAppRuntime();
            ServiceHelper.Initialize(appEnv);
            var assemblies = appEnv.GetAppAssemblies().Where(n => !n.IsSystemAssembly() && !n.FullName.StartsWith("JetBrains") && !n.FullName.StartsWith("ReSharper"));
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(16, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
            Assert.AreEqual(0, assemblyList.Count(a => a.FullName.StartsWith("JetBrains")));
        }

        [Test]
        public void GetAppAssemblies_no_filter()
        {
            var appEnv = new DynamicAppRuntime();
            ServiceHelper.Initialize(appEnv);
            try
            {
                var assemblies = appEnv.GetAppAssemblies();
                var assemblyList = assemblies.ToList();

                Assert.AreEqual(3, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
            }
            catch (FileLoadException fex)
                when (fex.Message.StartsWith("Could not load file or assembly 'ReSharperTestRunner"))
            {
                // no real error in ReSharper
            }
        }

        [Test]
        public void GetAppId_and_version()
        {
            IAppRuntime appEnv = new DynamicAppRuntime(appId: "hello-app", appVersion: "1.0.0-beta");

            Assert.AreEqual("hello-app", appEnv.GetAppId());
            Assert.AreEqual("1.0.0-beta", appEnv.GetAppVersion());
        }
    }
}