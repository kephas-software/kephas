// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessStarterFactoryTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the process starter factory test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Orchestration.Tests.Diagnostics
{
    using System;
    using System.IO;

    using Kephas.Application;
    using Kephas.Diagnostics;
    using Kephas.Orchestration.Diagnostics;
    using Kephas.Runtime;
    using NUnit.Framework;

    public class ProcessStarterFactoryTest
    {
        [Test]
        public void ProcessStarterFactory_LD_LIBRARY_PATH_empty_Linux()
        {
            if (RuntimeEnvironment.IsWindows())
            {
                return;
            }

            var appRuntime = CreateAppRuntime(Environment.CurrentDirectory);

            var builder = new ProcessStarterFactory(appRuntime)
                .WithManagedExecutable("a.exe");

            var processInfo = builder.GetProcessStartInfo();
            Assert.True(processInfo.EnvironmentVariables.ContainsKey(RuntimeEnvironment.LibraryPathEnvVariable));
            var ldLibraryPath = processInfo.EnvironmentVariables[RuntimeEnvironment.LibraryPathEnvVariable];
            Assert.AreEqual(appRuntime.GetAppLocation(), ldLibraryPath);
        }

        [Test]
        public void ProcessStarterFactory_LD_LIBRARY_PATH_empty_Windows()
        {
            if (!RuntimeEnvironment.IsWindows())
            {
                return;
            }

            var appRuntime = CreateAppRuntime(Environment.CurrentDirectory);

            var builder = new ProcessStarterFactory(appRuntime)
                .WithManagedExecutable("a.exe");

            var processInfo = builder.GetProcessStartInfo();
            Assert.False(processInfo.EnvironmentVariables.ContainsKey(RuntimeEnvironment.LibraryPathEnvVariable));
        }

        [Test]
        public void ProcessStarterFactory_LD_LIBRARY_PATH_Windows()
        {
            if (!RuntimeEnvironment.IsWindows())
            {
                return;
            }

            var tempFolder = Path.GetFullPath(Path.GetTempPath());
            var appLocation = Path.Combine(tempFolder, "_unit_test_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(appLocation);

            var appRuntime = CreateAppRuntime(appLocation);

            var builder = new ProcessStarterFactory(appRuntime)
                .WithManagedExecutable("a.exe");

            var processInfo = builder.GetProcessStartInfo();
            Assert.False(processInfo.EnvironmentVariables.ContainsKey(RuntimeEnvironment.LibraryPathEnvVariable));

            Directory.Delete(appLocation, recursive: true);
        }

        [Test]
        public void GetProcessStartInfo_executable_not_set()
        {
            var appRuntime = CreateAppRuntime(Environment.CurrentDirectory);

            var builder = new ProcessStarterFactory(appRuntime);

            Assert.Throws<InvalidOperationException>(() => builder.GetProcessStartInfo());
        }

        private IAppRuntime CreateAppRuntime(string appLocation = null)
        {
            var appRuntime = new DynamicAppRuntime(appFolder: appLocation);
            return appRuntime;
        }
    }
}