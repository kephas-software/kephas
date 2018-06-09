// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataOperationContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
            : base(dataContext?.CompositionContext)
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