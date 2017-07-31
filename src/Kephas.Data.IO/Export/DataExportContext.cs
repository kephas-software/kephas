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
            Requires.NotNull(output, nameof(output));

            this.Query = query;
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
        /// Gets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public ClientQuery Query { get; }

        /// <summary>
        /// Gets or sets the export output.
        /// </summary>
        /// <value>
        /// The export output.
        /// </value>
        public DataStream Output { get; set; }
    }

    /// <summary>
    /// Extensions for <see cref="IDataExportContext"/>.
    /// </summary>
    public static class DataExportContextExtensions
    {
        /// <summary>
        /// The result key.
        /// </summary>
        private const string ResultKey = "SYSTEM_Result";

        /// <summary>
        /// Ensures that a result is set in the options.
        /// </summary>
        /// <param name="dataExportContext">The data export context.</param>
        /// <param name="resultFactory">The result factory (optional).</param>
        /// <returns>
        /// The result, once it is set into the context.
        /// </returns>
        public static IDataIOResult EnsureResult(this IDataExportContext dataExportContext, Func<IDataIOResult> resultFactory = null)
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            var result = dataExportContext[ResultKey] as IDataIOResult;
            if (result == null)
            {
                resultFactory = resultFactory ?? (() => new DataIOResult());
                dataExportContext[ResultKey] = result = resultFactory();
            }

            return result;
        }

        /// <summary>
        /// Gets the result from the options.
        /// </summary>
        /// <param name="dataExportContext">The data export context.</param>
        /// <returns>The result, once it is set into the options.</returns>
        public static IDataIOResult GetResult(this IDataExportContext dataExportContext)
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            return dataExportContext[ResultKey] as IDataIOResult;
        }
    }
}