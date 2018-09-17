// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IQueryOperationContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Contract for query contexts.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System;
    using System.Linq;

    using Kephas.Services;

    /// <summary>
    /// Contract for query contexts.
    /// </summary>
    public interface IQueryOperationContext : IDataOperationContext
    {
        /// <summary>
        /// Gets or sets the implementation type resolver.
        /// </summary>
        /// <value>
        /// The implementation type resolver.
        /// </value>
        Func<Type, IContext, Type> ImplementationTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        IQueryable Query { get; set; }
    }
}