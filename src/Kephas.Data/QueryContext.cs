// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of a query operationContext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The default implementation of a query operationContext.
    /// </summary>
    public class QueryContext : DataOperationContext, IQueryContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public QueryContext(IDataContext dataContext)
            : base(dataContext)
        {
            Contract.Requires(dataContext != null);
        }
    }
}