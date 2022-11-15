// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataExportContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data export context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Injection;

namespace Kephas.Data.IO.Export
{
    using System;
    using System.Collections.Generic;
    using Kephas.Data.Client.Queries;
    using Kephas.Data.IO;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Services;

    /// <summary>
    /// A data export context.
    /// </summary>
    public class DataExportContext : DataIOContext, IDataExportContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataExportContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public DataExportContext(IInjector injector)
            : base(injector)
        {
        }

        /// <summary>
        /// Gets or sets the default root target type.
        /// </summary>
        /// <value>
        /// The default root target type.
        /// </value>
        public Type DefaultRootTargetType { get; set; }

        /// <summary>
        /// Gets or sets the query used to retrieve the data to be exported.
        /// </summary>
        /// <value>
        /// The query to retrieve data.
        /// </value>
        public ClientQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the data to be exported.
        /// </summary>
        /// <value>
        /// The data to be exported.
        /// </value>
        public IEnumerable<object> Data { get; set; }

        /// <summary>
        /// Gets or sets the export output.
        /// </summary>
        /// <value>
        /// The export output.
        /// </value>
        public DataStream Output { get; set; }

        /// <summary>
        /// Gets or sets the client query execution context configuration.
        /// </summary>
        /// <value>
        /// The client query execution context configuration.
        /// </value>
        public Action<IClientQueryExecutionContext> QueryExecutionConfig { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception if no data is found to export.
        /// </summary>
        /// <value>
        /// <c>true</c>true to throw an exception if no data is found to export, otherwise <c>false</c>.
        /// </value>
        public bool ThrowOnNotFound { get; set; }
    }
}