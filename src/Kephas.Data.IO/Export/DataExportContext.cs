// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataExportContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data export context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Export
{
    using System;
    using System.Collections.Generic;

    using Kephas.Data.Client.Queries;
    using Kephas.Data.IO;
    using Kephas.Data.IO.DataStreams;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A data export context.
    /// </summary>
    public class DataExportContext : DataIOContext, IDataExportContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataExportContext"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="output">The export output.</param>
        public DataExportContext(ClientQuery query, DataStream output)
        {
            Requires.NotNull(query, nameof(query));
            Requires.NotNull(output, nameof(output));

            this.Query = query;
            this.Output = output;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataExportContext"/> class.
        /// </summary>
        /// <param name="data">The data to be exported.</param>
        /// <param name="output">The export output.</param>
        public DataExportContext(IEnumerable<object> data, DataStream output)
        {
            Requires.NotNull(data, nameof(data));
            Requires.NotNull(output, nameof(output));

            this.Data = data;
            this.Output = output;
        }

        /// <summary>
        /// Gets or sets the default root target type.
        /// </summary>
        /// <value>
        /// The default root target type.
        /// </value>
        public Type DefaultRootTargetType { get; set; }

        /// <summary>
        /// Gets the query used to retrieve the data to be exported.
        /// </summary>
        /// <value>
        /// The query to retrieve data.
        /// </value>
        public ClientQuery Query { get; }

        /// <summary>
        /// Gets the data to be exported.
        /// </summary>
        /// <value>
        /// The data to be exported.
        /// </value>
        public IEnumerable<object> Data { get; }

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
        public Action<IClientQueryExecutionContext> ClientQueryExecutionContextConfig { get; set; }
    }
}