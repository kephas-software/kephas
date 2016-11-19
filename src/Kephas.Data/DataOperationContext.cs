// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataOperationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data operation context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System.Diagnostics.Contracts;

    using Kephas.Services;

    /// <summary>
    /// A context for data operations.
    /// </summary>
    public class DataOperationContext : ContextBase, IDataOperationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataOperationContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public DataOperationContext(IDataContext dataContext)
        {
            Contract.Requires(dataContext != null);

            this.DataContext = dataContext;
        }

        /// <summary>
        /// Gets the data context.
        /// </summary>
        /// <value>
        /// The data context.
        /// </value>
        public IDataContext DataContext { get; }
    }
}