// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyEmbeddedResourcesInitialDataHandlerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the assembly embedded resources initial data handler base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Composition;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Data.IO.Import;
    using Kephas.Data.IO.Resources;
    using Kephas.Net.Mime;

    /// <summary>
    /// An initial data handler base using the assembly embedded resources.
    /// </summary>
    public abstract class AssemblyEmbeddedResourcesInitialDataHandlerBase : InitialDataIOHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="AssemblyEmbeddedResourcesInitialDataHandlerBase"/> class.
        /// </summary>
        /// <param name="dataImportService">The data import service.</param>
        /// <param name="dataSpaceFactory">The data space factory.</param>
        protected AssemblyEmbeddedResourcesInitialDataHandlerBase(
            IDataImportService dataImportService,
            IExportFactory<IDataSpace> dataSpaceFactory)
            : base(dataImportService, dataSpaceFactory)
        {
            this.ResourcesAssembly = this.GetType().Assembly;
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
        /// Gets the data file names to be imported.
        /// </summary>
        /// <remarks>
        /// When overridden in a derived class, the file names should not contain the
        /// resource namespace, because it will be prepended by the <see cref="GetDataFilePaths"/>.
        /// Also, the file names should be provided in the right order for the import.
        /// </remarks>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the data file names in this
        /// collection.
        /// </returns>
        protected virtual IEnumerable<string> GetDataFileNames()
        {
            return null;
        }

        /// <summary>
        /// Gets the data files to be imported.
        /// </summary>
        /// <returns>
        /// An enumerator that allows foreach to be used to process the data files in this collection.
        /// </returns>
        protected override IEnumerable<string> GetDataFilePaths()
        {
            var resourceNamespace = this.GetDataFileResourceNamespace();

            // if file names provided, use them by prepending the resource namespace to them.
            var fileNames = this.GetDataFileNames();
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