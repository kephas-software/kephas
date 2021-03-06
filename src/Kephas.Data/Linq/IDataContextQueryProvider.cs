﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextQueryProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IDataContextQueryProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    /// <summary>
    /// Interface for query providers bound to a <see cref="IDataContext"/>.
    /// </summary>
    public interface IDataContextQueryProvider : IAsyncQueryProvider
    {
        /// <summary>
        /// Gets the bound data context.
        /// </summary>
        /// <value>
        /// The bound data context.
        /// </value>
        IDataContext DataContext { get; }

        /// <summary>
        /// Gets an operation context for the query.
        /// </summary>
        /// <value>
        /// The query operation context.
        /// </value>
        IQueryOperationContext QueryOperationContext { get; }
    }
}