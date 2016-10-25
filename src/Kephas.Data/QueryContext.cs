// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of a query context.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The default implementation of a query context.
    /// </summary>
    public class QueryContext : DataContext, IQueryContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryContext"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public QueryContext(IDataRepository repository)
            : base(repository)
        {
            Contract.Requires(repository != null);
        }
    }
}