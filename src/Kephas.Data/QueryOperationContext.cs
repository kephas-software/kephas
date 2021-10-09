// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryOperationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   The default implementation of a query operationContext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Linq;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// The default implementation of a query operationContext.
    /// </summary>
    public class QueryOperationContext : DataOperationContext, IQueryOperationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryOperationContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public QueryOperationContext(IDataContext dataContext)
            : base(dataContext)
        {
            dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        /// <summary>
        /// Gets or sets the implementation type resolver.
        /// </summary>
        /// <value>
        /// The implementation type resolver.
        /// </value>
        public Func<Type, IContext, Type> ImplementationTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public object Options { get; set; }
        
        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public IQueryable Query { get; set; }
    }
}