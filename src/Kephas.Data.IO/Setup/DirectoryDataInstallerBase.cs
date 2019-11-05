// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryDataInstallerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the directory data installer base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Setup
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Kephas.Composition;
    using Kephas.Data.IO.Import;
    using Kephas.Services;

    /// <summary>
    /// Base class for directory-based data installers.
    /// </summary>
    public abstract class DirectoryDataInstallerBase : DataIOInstallerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryDataInstallerBase" />
        /// class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="dataImportService">The data import service.</param>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        /// <param name="dataPath">The full pathname of the location containing the data files.</param>
        protected DirectoryDataInstallerBase(
            IContextFactory contextFactory,
            IDataImportService dataImportService,
            IExportFactory<IDataSpace> dataSpaceFactory,
            string dataPath)
            : base(contextFactory, dataImportService, dataSpaceFactory)
        {
            this.DataPath = dataPath;
        }

        /// <summary>
        /// Gets the full pathname of the data file.
        /// </summary>
        /// <value>
        /// The full pathname of the data file.
        /// </value>
        public string DataPath { get; }

        /// <summary>
        /// Gets the patterns for the data files.
        /// </summary>
        /// <returns>
        /// An enumeration of data file pattern strings.
        /// </returns>
        protected virtual IEnumerable<string> GetDataFilePatterns()
        {
            yield return "*.json";
            yield return "*.xml";
        }

        /// <summary>
        /// Gets the file names of the data to be installed.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, the file names should not contain the
        /// resource namespace, because it will be prepended by the <see cref="GetInstallDataFilePaths"/>.
        /// Also, the file names should be provided in the right order for the import.
        /// </remarks>
        /// <returns>
        /// An enumeration of file names.
        /// </returns>
        protected virtual IEnumerable<string> GetInstallDataFileNames()
        {
            return null;
        }

        /// <summary>
        /// Gets the data file names of the data to be uninstalled.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, the file names should not contain the
        /// resource namespace, because it will be prepended by the <see cref="GetInstallDataFilePaths"/>.
        /// Also, the file names should be provided in the right order for the import.
        /// </remarks>
        /// <returns>
        /// An enumeration of file names.
        /// </returns>
        protected virtual IEnumerable<string> GetUninstallDataFileNames()
        {
            return null;
        }

        /// <summary>
        /// Gets the files for the data to be installed.
        /// </summary>
        /// <returns>
        /// An enumeration of file paths.
        /// </returns>
        protected override IEnumerable<string> GetInstallDataFilePaths()
        {
            return this.GetDataFilePaths("Install", this.GetInstallDataFileNames());
        }

        /// <summary>
        /// Gets the files for the data to be uninstalled.
        /// </summary>
        /// <returns>
        /// An enumeration of file paths.
        /// </returns>
        protected override IEnumerable<string> GetUninstallDataFilePaths()
        {
            return this.GetDataFilePaths("Uninstall", this.GetUninstallDataFileNames());
        }

        /// <summary>
        /// Gets the data files to be imported for the provided operation namespace.
        /// </summary>
        /// <param name="operationPath">The operation relative path.</param>
        /// <param name="fileNames">List of names of the files.</param>
        /// <returns>
        /// An enumeration of file paths.
        /// </returns>
        protected virtual IEnumerable<string> GetDataFilePaths(string operationPath, IEnumerable<string> fileNames)
        {
            var dataPath = Path.Combine(this.DataPath, operationPath);

            // if file names not provided, read all files from the operation data directory.
            if (fileNames == null)
            {
                fileNames = this.GetDataFilePatterns()
                    .SelectMany(p => Directory.Exists(dataPath) ? Directory.EnumerateFiles(dataPath, p) : new string[0])
                    .OrderBy(n => n)
                    .ToArray();
            }

            return fileNames.Select(f => Path.Combine(dataPath, f));
        }
    }
}