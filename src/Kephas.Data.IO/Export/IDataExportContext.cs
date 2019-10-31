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
        /// Gets or sets the query used to retrieve the data to be exported.
        /// </summary>
        /// <value>
        /// The query to retrieve data.
        /// </value>
        ClientQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the data to be exported.
        /// </summary>
        /// <value>
        /// The data to be exported.
        /// </value>
        IEnumerable<object> Data { get; set; }

        /// <summary>
        /// Gets or sets the export output.
        /// </summary>
        /// <value>
        /// The export output.
        /// </value>
        DataStream Output { get; set; }

        /// <summary>
        /// Gets or sets the client query execution options configuration.
        /// </summary>
        /// <value>
        /// The client query execution options configuration.
        /// </value>
        Action<IClientQueryExecutionContext> QueryExecutionConfig { get; set; }

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
        private const string ResultKey = "Result";

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
        /// Sets the client query execution options configuration.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataExportContext">The data export context.</param>
        /// <param name="optionsConfig">The query execution options configuration.</param>
        /// <returns>
        /// This <paramref name="dataExportContext"/>.
        /// </returns>
        public static TContext SetQueryExecutionConfig<TContext>(
            this TContext dataExportContext,
            Action<IClientQueryExecutionContext> optionsConfig)
            where TContext : class, IDataExportContext
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            dataExportContext.QueryExecutionConfig = optionsConfig;

            return dataExportContext;
        }

        /// <summary>
        /// Sets the output.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataExportContext">The data export context.</param>
        /// <param name="output">The export output.</param>
        /// <returns>
        /// This <paramref name="dataExportContext"/>.
        /// </returns>
        public static TContext Output<TContext>(
            this TContext dataExportContext,
            DataStream output)
            where TContext : class, IDataExportContext
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            dataExportContext.Output = output;

            return dataExportContext;
        }

        /// <summary>
        /// Sets the data to export.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataExportContext">The data export context.</param>
        /// <param name="data">The data to export.</param>
        /// <returns>
        /// This <paramref name="dataExportContext"/>.
        /// </returns>
        public static TContext Data<TContext>(
            this TContext dataExportContext,
            IEnumerable<object> data)
            where TContext : class, IDataExportContext
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            dataExportContext.Data = data;

            return dataExportContext;
        }

        /// <summary>
        /// Sets the query used to retrieve the data to export.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataExportContext">The data export context.</param>
        /// <param name="query">The query used to retrieve the data to export.</param>
        /// <returns>
        /// This <paramref name="dataExportContext"/>.
        /// </returns>
        public static TContext Query<TContext>(
            this TContext dataExportContext,
            ClientQuery query)
            where TContext : class, IDataExportContext
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            dataExportContext.Query = query;

            return dataExportContext;
        }

        /// <summary>
        /// Sets the default root target type.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataExportContext">The data export context.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>
        /// This <paramref name="dataExportContext"/>.
        /// </returns>
        public static TContext DefaultRootTargetType<TContext>(
            this TContext dataExportContext,
            Type targetType)
            where TContext : class, IDataExportContext
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            dataExportContext.DefaultRootTargetType = targetType;

            return dataExportContext;
        }

        /// <summary>
        /// Sets a value indicating whether to throw an exception if no data is found to export.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="dataExportContext">The data export context.</param>
        /// <param name="throwOnNotFound">True to throw on not found, false otherwise.</param>
        /// <returns>
        /// This <paramref name="dataExportContext"/>.
        /// </returns>
        public static TContext ThrowOnNotFound<TContext>(
            this TContext dataExportContext,
            bool throwOnNotFound)
            where TContext : class, IDataExportContext
        {
            Requires.NotNull(dataExportContext, nameof(dataExportContext));

            dataExportContext.ThrowOnNotFound = throwOnNotFound;

            return dataExportContext;
        }
    }
}