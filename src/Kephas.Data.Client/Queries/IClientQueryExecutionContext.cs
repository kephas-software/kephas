// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientQueryExecutionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IClientQueryExecutionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries
{
    using System;

    using Kephas.Data.Client.Queries.Conversion;
    using Kephas.Data.Conversion;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Interface for client query execution context.
    /// </summary>
    public interface IClientQueryExecutionContext : IContext
    {
        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the type of the client entity.
        /// </summary>
        /// <value>
        /// The type of the client entity.
        /// </value>
        Type ClientEntityType { get; set; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        object Options { get; set; }

        /// <summary>
        /// Gets or sets the query conversion options configuration.
        /// </summary>
        /// <value>
        /// The query conversion options configuration.
        /// </value>
        Action<IClientQueryConversionContext> QueryConversionConfig { get; set; }

        /// <summary>
        /// Gets or sets the data conversion options configuration.
        /// </summary>
        /// <value>
        /// The data conversion options configuration.
        /// </value>
        Action<object, IDataConversionContext> DataConversionConfig { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="IClientQueryExecutionContext"/>.
    /// </summary>
    public static class ClientQueryExecutionContextExtensions
    {
        /// <summary>
        /// Sets the client query conversion context configuration.
        /// </summary>
        /// <param name="clientQueryExecutionContext">The client query execution context to act on.</param>
        /// <param name="optionsConfig">The client query conversion context configuration.</param>
        /// <returns>
        /// The client query execution context.
        /// </returns>
        public static IClientQueryExecutionContext QueryConversionConfig(
            this IClientQueryExecutionContext clientQueryExecutionContext,
            Action<IClientQueryConversionContext> optionsConfig)
        {
            Requires.NotNull(clientQueryExecutionContext, nameof(clientQueryExecutionContext));

            clientQueryExecutionContext.QueryConversionConfig = optionsConfig;

            return clientQueryExecutionContext;
        }

        /// <summary>
        /// Sets the client query execution context configuration.
        /// </summary>
        /// <param name="clientQueryExecutionContext">The client query execution context to act on.</param>
        /// <param name="optionsConfig">The data conversion context configuration.</param>
        /// <returns>
        /// The client query execution context.
        /// </returns>
        public static IClientQueryExecutionContext DataConversionConfig(
            this IClientQueryExecutionContext clientQueryExecutionContext,
            Action<object, IDataConversionContext> optionsConfig)
        {
            Requires.NotNull(clientQueryExecutionContext, nameof(clientQueryExecutionContext));

            clientQueryExecutionContext.DataConversionConfig = optionsConfig;

            return clientQueryExecutionContext;
        }
    }
}