// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscardChangesContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the discard changes context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// An operation context for discarding changes.
    /// </summary>
    public class DiscardChangesContext : DataOperationContext, IDiscardChangesContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscardChangesContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public DiscardChangesContext(IDataContext dataContext)
            : base(dataContext)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
        }
    }
}