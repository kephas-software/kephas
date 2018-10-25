// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataIOInstallerBaseTest.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initial data i/o handler base test class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Tests.Setup
{
    using System.Collections.Generic;
    using System.IO;

    using Kephas.Composition.ExportFactories;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Data.IO.Setup;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class DataIOInstallerBaseTest
    {
        [Test]
        public void CreateDataSource_missing_file()
        {
            var handler = new DataInstaller();
            Assert.Throws<IOException>(() => handler.CreateDataSource("dummy file which does not exist"));
        }

        public class DataInstaller : DataIOInstallerBase
        {
            private readonly IEnumerable<string> installFilePaths;

            private readonly IEnumerable<string> uninstallFilePaths;

            public DataInstaller(
                IDataImportService dataImportService = null,
                IDataSpace dataSpace = null,
                IEnumerable<string> installFilePaths = null,
                IEnumerable<string> uninstallFilePaths = null)
                : base(
                    dataImportService ?? Substitute.For<IDataImportService>(),
                    new ExportFactory<IDataSpace>(() => dataSpace ?? Substitute.For<IDataSpace>()))
            {
                this.installFilePaths = installFilePaths;
                this.uninstallFilePaths = uninstallFilePaths;
            }

            /// <summary>
            /// Gets the data files to be imported.
            /// </summary>
            /// <returns>
            /// An enumerator that allows foreach to be used to process the data files in this collection.
            /// </returns>
            protected override IEnumerable<string> GetInstallDataFilePaths()
            {
                return this.installFilePaths;
            }

            /// <summary>
            /// Gets the files containing data to be uninstalled.
            /// </summary>
            /// <returns>
            /// An enumeration of file paths.
            /// collection.
            /// </returns>
            protected override IEnumerable<string> GetUninstallDataFilePaths()
            {
                return this.uninstallFilePaths;
            }

            /// <summary>
            /// Creates a data source for the import operation.
            /// </summary>
            /// <param name="dataFilePath">The data file path.</param>
            /// <returns>
            /// The new data source.
            /// </returns>
            public new DataStream CreateDataSource(string dataFilePath)
            {
                return base.CreateDataSource(dataFilePath);
            }
        }
    }
}