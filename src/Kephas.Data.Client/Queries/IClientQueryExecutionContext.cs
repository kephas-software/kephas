// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientQueryExecutionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// Gets or sets the client query conversion context configuration.
        /// </summary>
        /// <value>
        /// The client query conversion context configuration.
        /// </value>
        Action<IClientQueryConversionContext> ClientQueryConversionContextConfig { get; set; }

        /// <summary>
        /// Gets or sets the data conversion context configuration.
        /// </summary>
        /// <value>
        /// The data conversion context configuration.
        /// </value>
        Action<object, IDataConversionContext> DataConversionContextConfig { get; set; }
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
        /// <param name="clientQueryConversionContextConfig">The client query conversion context configuration.</param>
        /// <returns>
        /// The client query execution context.
        /// </returns>
        public static IClientQueryExecutionContext WithClientQueryConversionContextConfig(
            this IClientQueryExecutionContext clientQueryExecutionContext,
            Action<IClientQueryConversionContext> clientQueryConversionContextConfig)
        {
            Requires.NotNull(clientQueryExecutionContext, nameof(clientQueryExecutionContext));

            clientQueryExecutionContext.ClientQueryConversionContextConfig = clientQueryConversionContextConfig;

            return clientQueryExecutionContext;
        }

        /// <summary>
        /// Sets the client query execution context configuration.
        /// </summary>
        /// <param name="clientQueryExecutionContext">The client query execution context to act on.</param>
        /// <param name="dataConversionContextConfig">The data conversion context configuration.</param>
        /// <returns>
        /// The client query execution context.
        /// </returns>
        public static IClientQueryExecutionContext WithDataConversionContextConfig(
            this IClientQueryExecutionContext clientQueryExecutionContext,
            Action<object, IDataConversionContext> dataConversionContextConfig)
        {
            Requires.NotNull(clientQueryExecutionContext, nameof(clientQueryExecutionContext));

            clientQueryExecutionContext.DataConversionContextConfig = dataConversionContextConfig;

            return clientQueryExecutionContext;
        }
    }
}