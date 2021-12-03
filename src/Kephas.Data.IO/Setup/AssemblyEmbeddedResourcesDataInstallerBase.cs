// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyEmbeddedResourcesDataInstallerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the assembly embedded resources initial data handler base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.IO.Setup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Data.IO.Resources;
    using Kephas.Net.Mime;
    using Kephas.Services;

    /// <summary>
    /// An initial data handler base using the assembly embedded resources.
    /// </summary>
    public abstract class AssemblyEmbeddedResourcesDataInstallerBase : DataIOInstallerBase
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AssemblyEmbeddedResourcesDataInstallerBase"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="dataImportService">The data import service.</param>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        /// <remarks>
        /// The assembly containing the resources is the assembly where the data installer class is
        /// declared.
        /// </remarks>
        protected AssemblyEmbeddedResourcesDataInstallerBase(
            IContextFactory contextFactory,
            IDataImportService dataImportService,
            IExportFactory<IDataSpace> dataSpaceFactory)
            : base(contextFactory, dataImportService, dataSpaceFactory)
        {
            this.ResourcesAssembly = this.GetType().Assembly;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AssemblyEmbeddedResourcesDataInstallerBase"/> class.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <param name="dataImportService">The data import service.</param>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        /// <param name="assembly">The assembly containing the resources.</param>
        protected AssemblyEmbeddedResourcesDataInstallerBase(
            IContextFactory contextFactory,
            IDataImportService dataImportService,
            IExportFactory<IDataSpace> dataSpaceFactory,
            Assembly assembly)
            : base(contextFactory, dataImportService, dataSpaceFactory)
        {
            assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));

            this.ResourcesAssembly = assembly;
        }

        /// <summary>
        /// Gets or sets the assembly holding the resources.
        /// </summary>
        protected Assembly ResourcesAssembly { get; set; }

        /// <summary>
        /// Gets the data file resource namespace.
        /// </summary>
        /// <returns>
        /// The data file resource namespace.
        /// </returns>
        protected virtual string GetDataFileResourceNamespace()
        {
            var assemblyNamespace = this.ResourcesAssembly.GetName().Name;
            var resourceNamespace = $"{assemblyNamespace}.Data.";
            return resourceNamespace;
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
        /// <param name="operationNamespace">The operation namespace.</param>
        /// <param name="fileNames">List of names of the files.</param>
        /// <returns>
        /// An enumeration of file paths.
        /// </returns>
        protected virtual IEnumerable<string> GetDataFilePaths(string operationNamespace, IEnumerable<string> fileNames)
        {
            var resourceNamespace = $"{this.GetDataFileResourceNamespace()}{operationNamespace}.";

            // if file names provided, use them by prepending the resource namespace to them.
            if (fileNames != null)
            {
                return fileNames.Select(f => resourceNamespace + f);
            }

            // if file names not provided, read all files from the resource namespace.
            // CAUTION: when providing the file names, they should be sorted in the proper import order.
            var assembly = this.ResourcesAssembly;
            var names = assembly.GetManifestResourceNames()
                .Where(n => n.StartsWith(resourceNamespace))
                .OrderBy(n => n)
                .ToArray();
            return names;
        }

        /// <summary>
        /// Creates an output data source.
        /// </summary>
        /// <param name="dataFilePath">The data file path.</param>
        /// <returns>
        /// The new data source.
        /// </returns>
        protected override DataStream CreateDataSource(string dataFilePath)
        {
            var assembly = this.ResourcesAssembly;
            var data = assembly.GetManifestResourceStream(dataFilePath);
            if (data == null)
            {
                throw new ArgumentException(string.Format(Strings.AssemblyEmbeddedResourcesInitialDataHandlerBase_NotFound_Exception, dataFilePath));
            }

            return new DataStream(data, dataFilePath, MediaTypeNames.Application.Json, ownsStream: true);
        }
    }
}