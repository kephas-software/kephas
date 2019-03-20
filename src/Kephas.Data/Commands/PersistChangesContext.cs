// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the persist changes operationContext class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Collections.Generic;

    using Kephas.Data.Capabilities;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// An operation context for persisting changes.
    /// </summary>
    public class PersistChangesContext : DataOperationContext, IPersistChangesContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistChangesContext"/> class.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        public PersistChangesContext(IDataContext dataContext)
            : base(dataContext)
        {
            Requires.NotNull(dataContext, nameof(dataContext));
        }

        /// <summary>
        /// Gets or sets the change set to be persisted.
        /// </summary>
        /// <remarks>
        /// If the change set is <c>null</c>, an automatic change set detection is performed.
        /// </remarks>
        /// <value>
        /// The change set.
        /// </value>
        public IEnumerable<object> ChangeSet { get; set; }

        /// <summary>
        /// Gets or sets the change set to be persisted within an iteration.
        /// </summary>
        /// <value>
        /// The change set within an iteration.
        /// </value>
        public IEnumerable<IEntityEntry> IterationChangeSet { get; set; }

        /// <summary>
        /// Gets or sets the iteration during persistence workflow.
        /// </summary>
        /// <value>
        /// The iteration.
        /// </value>
        public int Iteration { get; set; }
    }
}