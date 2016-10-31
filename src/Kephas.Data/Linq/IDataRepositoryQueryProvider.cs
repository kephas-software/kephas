// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataRepositoryQueryProvider.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IDataRepositoryQueryProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Linq
{
    using System.Linq;

    /// <summary>
    /// Interface for query providers bound to a data repository.
    /// </summary>
    public interface IDataRepositoryQueryProvider : IQueryProvider
    {
        /// <summary>
        /// Gets the bound data repository.
        /// </summary>
        /// <value>
        /// The bound data repository.
        /// </value>
        IDataRepository Repository { get; }

        /// <summary>
        /// Gets a context for the query.
        /// </summary>
        /// <value>
        /// The query context.
        /// </value>
        IQueryContext QueryContext { get; }
    }
}