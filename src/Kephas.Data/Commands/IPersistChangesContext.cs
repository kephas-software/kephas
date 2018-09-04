// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistChangesContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPersistChangesContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Collections.Generic;

    using Kephas.Data.Capabilities;

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
        /// Gets or sets the change set to be persisted within an iteration.
        /// </summary>
        /// <value>
        /// The change set within an iteration.
        /// </value>
        IEnumerable<IEntityInfo> IterationChangeSet { get; set; }

        /// <summary>
        /// Gets or sets the iteration during persistence workflow.
        /// </summary>
        /// <value>
        /// The iteration.
        /// </value>
        int Iteration { get; set; }
    }
}