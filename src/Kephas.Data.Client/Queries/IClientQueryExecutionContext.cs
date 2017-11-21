// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientQueryExecutionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
    using Kephas.Services;

    /// <summary>
    /// Interface for client query execution context.
    /// </summary>
    public interface IClientQueryExecutionContext : IContext
    {
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
}