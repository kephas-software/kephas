// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetAppRuntimeTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the net 46 application runtime test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.Tests.Application
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class NetAppRuntimeTest
    {
        [Test]
        public async Task GetAppAssembliesAsync_filter()
        {
            var appEnv = new NetAppRuntime(assemblyLoader: new NetAssemblyLoader());
            var assemblies = await appEnv.GetAppAssembliesAsync(n => !n.IsSystemAssembly() && !n.FullName.StartsWith("JetBrains"));
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(3, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
            Assert.AreEqual(0, assemblyList.Count(a => a.FullName.StartsWith("JetBrains")));
        }

        [Test]
        public async Task GetAppAssembliesAsync_no_filter()
        {
            var appEnv = new NetAppRuntime(assemblyLoader: new NetAssemblyLoader());
            var assemblies = await appEnv.GetAppAssembliesAsync();
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(3, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
        }
    }
}