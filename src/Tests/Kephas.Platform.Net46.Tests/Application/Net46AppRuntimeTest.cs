// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Net46AppRuntimeTest.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the net 46 application runtime test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Platform.Net46.Tests.Application
{
    using System.Linq;
    using System.Threading.Tasks;

    using Kephas.Application;
    using Kephas.Reflection;

    using NUnit.Framework;

    [TestFixture]
    public class Net46AppRuntimeTest
    {
        [Test]
        public async Task GetAppAssembliesAsync_success()
        {
            var appEnv = new Net46AppRuntime();
            var assemblies = await appEnv.GetAppAssembliesAsync(n => !n.IsSystemAssembly() && !n.FullName.StartsWith("JetBrains"));
            var assemblyList = assemblies.ToList();

            Assert.AreEqual(3, assemblyList.Count(a => a.FullName.StartsWith("Kephas")));
        }
    }
}