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
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// A context for data operations.
    /// </summary>
    public class DataOperationContext : Context, IDataOperationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataOperationContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public DataOperationContext(IDataContext dataContext)
            : base(dataContext.AmbientServices)
        {
            Requires.NotNull(dataContext, nameof(dataContext));

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