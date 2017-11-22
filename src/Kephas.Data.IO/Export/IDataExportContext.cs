// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataExportContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataExportContext interface.
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
    /// Interface for data export context.
    /// </summary>
    public interface IDataExportContext : IDataIOContext
    {
        /// <summary>
        /// Gets or sets the default root target type.
        /// </summary>
        /// <value>
        /// The default root target type.
        /// </value>
        Type DefaultRootTargetType { get; set; }

        /// <summary>
        /// Gets the query used to retrieve the data to be exported.
        /// </summary>
        /// <value>
        /// The query to retrieve data.
        /// </value>
        ClientQuery Query { get; }

        /// <summary>
        /// Gets the data to be exported.
        /// </summary>
        /// <value>
        /// The data to be exported.
        /// </value>
        IEnumerable<object> Data { get; }

        /// <summary>
        /// Gets or sets the export output.
        /// </summary>
        /// <value>
        /// The export output.
        /// </value>
        DataStream Output { get; set; }

        /// <summary>
        /// Gets or sets the client query execution context configuration.
        /// </summary>
        /// <value>
        /// The client query execution context configuration.
        /// </value>
        Action<IClientQueryExecutionContext> ClientQueryExecutionContextConfig { get; set; }
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

            if (!(dataExportContext[ResultKey] is IDataIOResult result))
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