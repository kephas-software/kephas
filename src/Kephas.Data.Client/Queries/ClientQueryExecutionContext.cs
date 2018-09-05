// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientQueryExecutionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client query execution context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Queries
{
    using System;

    using Kephas.Composition;
    using Kephas.Data.Client.Queries.Conversion;
    using Kephas.Data.Conversion;
    using Kephas.Services;

    /// <summary>
    /// A client query execution context.
    /// </summary>
    public class ClientQueryExecutionContext : Context, IClientQueryExecutionContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientQueryExecutionContext"/> class.
        /// </summary>
        /// <param name="compositionContext">The composition context.</param>
        public ClientQueryExecutionContext(ICompositionContext compositionContext = null)
            : base(compositionContext)
        {
        }

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the type of the client entity.
        /// </summary>
        /// <value>
        /// The type of the client entity.
        /// </value>
        public Type ClientEntityType { get; set; }

        /// <summary>
        /// Gets or sets the client query conversion context configuration.
        /// </summary>
        /// <value>
        /// The client query conversion context configuration.
        /// </value>
        public Action<IClientQueryConversionContext> ClientQueryConversionContextConfig { get; set; }

        /// <summary>
        /// Gets or sets the data conversion context configuration.
        /// </summary>
        /// <value>
        /// The data conversion context configuration.
        /// </value>
        public Action<object, IDataConversionContext> DataConversionContextConfig { get; set; }
    }
}