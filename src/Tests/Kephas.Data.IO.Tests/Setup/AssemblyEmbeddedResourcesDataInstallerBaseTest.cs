﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyEmbeddedResourcesDataInstallerBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the assembly embedded resources initial data handler base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Kephas.Composition;
    using Kephas.Composition.ExportFactories;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Data.IO.Setup;
    using Kephas.Data.Setup;
    using Kephas.Services;
    using Kephas.Testing;
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class AssemblyEmbeddedResourcesDataInstallerBaseTest : TestBase
    {
        [Test]
        public void GetInstallDataFilePaths_explicit_file_names()
        {
            var handler = new TestAssemblyEmbeddedResourcesDataInstaller(this.GetContextFactory(), installFileNames: new[] { "my-embedded-data.json" });
            var filePaths = handler.GetInstallDataFilePaths().ToList();
            Assert.AreEqual(1, filePaths.Count);
            Assert.AreEqual("Kephas.Data.IO.Tests.Data.Install.my-embedded-data.json", filePaths[0]);
        }

        [Test]
        public void GetInstallDataFilePaths_implicit_all_file_names()
        {
            var handler = new TestAssemblyEmbeddedResourcesDataInstaller(this.GetContextFactory());
            var filePaths = handler.GetInstallDataFilePaths().ToList();
            Assert.AreEqual(2, filePaths.Count);
            Assert.AreEqual("Kephas.Data.IO.Tests.Data.Install.my-embedded-data.json", filePaths[0]);
            Assert.AreEqual("Kephas.Data.IO.Tests.Data.Install.my-embedded-data-2.json", filePaths[1]);
        }

        [Test]
        public void GetUninstallDataFilePaths_explicit_file_names()
        {
            var handler = new TestAssemblyEmbeddedResourcesDataInstaller(this.GetContextFactory(), uninstallFileNames: new[] { "my-u-embedded-data.json" });
            var filePaths = handler.GetUninstallDataFilePaths().ToList();
            Assert.AreEqual(1, filePaths.Count);
            Assert.AreEqual("Kephas.Data.IO.Tests.Data.Uninstall.my-u-embedded-data.json", filePaths[0]);
        }

        [Test]
        public void GetUninstallDataFilePaths_implicit_all_file_names()
        {
            var handler = new TestAssemblyEmbeddedResourcesDataInstaller(this.GetContextFactory());
            var filePaths = handler.GetUninstallDataFilePaths().ToList();
            Assert.AreEqual(2, filePaths.Count);
            Assert.AreEqual("Kephas.Data.IO.Tests.Data.Uninstall.my-u-embedded-data.json", filePaths[0]);
            Assert.AreEqual("Kephas.Data.IO.Tests.Data.Uninstall.my-u-embedded-data-2.json", filePaths[1]);
        }

        [Test]
        public void CreateDataSource_missing_resource()
        {
            var handler = new TestAssemblyEmbeddedResourcesDataInstaller(this.GetContextFactory());
            Assert.Throws<ArgumentException>(() => handler.CreateDataSource("Kephas.Data.IO.Tests.Data.my-data.json"));
        }

        private IContextFactory GetContextFactory()
        {
            return this.CreateContextFactoryMock(() => new DataSetupContext(Substitute.For<ICompositionContext>()));
        }

        public class TestAssemblyEmbeddedResourcesDataInstaller : AssemblyEmbeddedResourcesDataInstallerBase
        {
            private readonly IEnumerable<string> installFileNames;

            private readonly IEnumerable<string> uninstallFileNames;

            public TestAssemblyEmbeddedResourcesDataInstaller(
                IContextFactory contextFactory,
                IDataImportService dataImportService = null,
                IDataSpace dataSpace = null,
                IEnumerable<string> installFileNames = null,
                IEnumerable<string> uninstallFileNames = null)
                : base(
                    contextFactory,
                    dataImportService ?? Substitute.For<IDataImportService>(),
                    new ExportFactory<IDataSpace>(() => dataSpace ?? GetDataSpace()))
            {
                this.installFileNames = installFileNames;
                this.uninstallFileNames = uninstallFileNames;
            }

            private static IDataSpace GetDataSpace()
            {
                return Substitute.For<IDataSpace>();
            }

            protected override IEnumerable<string> GetInstallDataFileNames()
            {
                return this.installFileNames;
            }

            protected override IEnumerable<string> GetUninstallDataFileNames()
            {
                return this.uninstallFileNames;
            }

            public new IEnumerable<string> GetInstallDataFilePaths()
            {
                return base.GetInstallDataFilePaths();
            }

            public new IEnumerable<string> GetUninstallDataFilePaths()
            {
                return base.GetUninstallDataFilePaths();
            }

            public new DataStream CreateDataSource(string dataFilePath)
            {
                return base.CreateDataSource(dataFilePath);
            }
        }
    }
}