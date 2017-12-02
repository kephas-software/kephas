// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistChangesContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IPersistChangesContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Collections.Generic;

    /// <summary>
    /// Context interface for persisting changes operation.
    /// </summary>
    public interface IPersistChangesContext : IDataOperationContext
    {
        /// <summary>
        /// Gets or sets the change set to be persisted.
        /// </summary>
        /// <remarks>
        /// If the change set is <c>null</c>, an automatic change set detection is performed.
        /// </remarks>
        /// <value>
        /// The change set.
        /// </value>
        IEnumerable<object> ChangeSet { get; set; }

        /// <summary>
        /// Gets or sets the iteration during persistence workflow.
        /// </summary>
        /// <value>
        /// The iteration.
        /// </value>
        int Iteration { get; set; }
    }
}