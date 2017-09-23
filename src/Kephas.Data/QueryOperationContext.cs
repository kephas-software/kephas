// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryOperationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   The default implementation of a query operationContext.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using Kephas.Diagnostics.Contracts;

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
            Requires.NotNull(dataContext, nameof(dataContext));
        }
    }
}