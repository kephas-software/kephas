// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataContextQueryProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataContextQueryProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    using System.Linq;

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