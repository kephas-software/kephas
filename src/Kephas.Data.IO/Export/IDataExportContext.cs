// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataExportContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    using Kephas.Operations;

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

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception if no data is found to export.
        /// </summary>
        /// <value>
        /// <c>true</c>true to throw an exception if no data is found to export, otherwise <c>false</c>.
        /// </value>
        bool ThrowOnNotFound { get; set; }
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
        public static IOperationResult EnsureResult(this IDataExportContext dataExportContext, Func<IOperationResult> resultFactory = null)
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            if (!(dataExportContext[ResultKey] is IOperationResult result))
            {
                resultFactory = resultFactory ?? (() => new OperationResult());
                dataExportContext[ResultKey] = result = resultFactory();
            }

            return result;
        }

        /// <summary>
        /// Gets the result from the options.
        /// </summary>
        /// <param name="dataExportContext">The data export context.</param>
        /// <returns>The result, once it is set into the options.</returns>
        public static IOperationResult GetResult(this IDataExportContext dataExportContext)
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            return dataExportContext[ResultKey] as IOperationResult;
        }

        /// <summary>
        /// Sets the client query execution context configuration.
        /// </summary>
        /// <param name="dataExportContext">The data export context to act on.</param>
        /// <param name="clientQueryExecutionContextConfig">The client query execution context configuration.</param>
        /// <returns>
        /// The data export context.
        /// </returns>
        public static IDataExportContext WithClientQueryExecutionContextConfig(
            this IDataExportContext dataExportContext,
            Action<IClientQueryExecutionContext> clientQueryExecutionContextConfig)
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            dataExportContext.ClientQueryExecutionContextConfig = clientQueryExecutionContextConfig;

            return dataExportContext;
        }
    }
}