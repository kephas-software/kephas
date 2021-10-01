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
        /// Sets the domain entity type.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The client query execution context.</param>
        /// <param name="entityType">The domain entity type.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext EntityType<TContext>(
            this TContext context,
            Type entityType)
            where TContext : class, IClientQueryExecutionContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.EntityType = entityType;

            return context;
        }

        /// <summary>
        /// Sets the client entity type.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The client query execution context.</param>
        /// <param name="clientEntityType">The client entity type.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext ClientEntityType<TContext>(
            this TContext context,
            Type clientEntityType)
            where TContext : class, IClientQueryExecutionContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.ClientEntityType = clientEntityType;

            return context;
        }

        /// <summary>
        /// Sets the custom options.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The client query execution context.</param>
        /// <param name="options">The custom options.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext Options<TContext>(
            this TContext context,
            object options)
            where TContext : class, IClientQueryExecutionContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.Options = options;

            return context;
        }

        /// <summary>
        /// Sets the client query conversion options.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The client query execution context.</param>
        /// <param name="optionsConfig">The client query conversion options.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext SetQueryConversionConfig<TContext>(
            this TContext context,
            Action<IClientQueryConversionContext> optionsConfig)
            where TContext : class, IClientQueryExecutionContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.QueryConversionConfig = optionsConfig;

            return context;
        }

        /// <summary>
        /// Sets the data conversion options.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The client query execution context.</param>
        /// <param name="optionsConfig">The data conversion options.</param>
        /// <returns>
        /// This <paramref name="context"/>.
        /// </returns>
        public static TContext SetDataConversionConfig<TContext>(
            this TContext context,
            Action<object, IDataConversionContext> optionsConfig)
            where TContext : class, IClientQueryExecutionContext
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.DataConversionConfig = optionsConfig;

            return context;
        }
    }
}