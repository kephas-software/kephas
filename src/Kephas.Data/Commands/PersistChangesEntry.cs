// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersistChangesEntry.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the persist changes entry class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Collections.Generic;
    using Kephas.Data.Behaviors;
    using Kephas.Services;

    /// <summary>
    /// A modified entry.
    /// </summary>
    public class PersistChangesEntry : ContextBase, IPersistChangesEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistChangesEntry"/> class.
        /// </summary>
        /// <param name="modifiedEntity">The modified entity.</param>
        /// <param name="changeState">The change state.</param>
        /// <param name="flattenedEntityGraph">The flattened entity graph.</param>
        public PersistChangesEntry(object modifiedEntity, ChangeState changeState, IEnumerable<object> flattenedEntityGraph)
        {
            this.ModifiedEntity = modifiedEntity;
            this.ChangeState = changeState;
            this.FlattenedEntityGraph = flattenedEntityGraph;
        }

        /// <summary>
        /// Gets the change state.
        /// </summary>
        /// <value>
        /// The change state.
        /// </value>
        public ChangeState ChangeState { get; }

        /// <summary>
        /// Gets the modified entity.
        /// </summary>
        /// <value>
        /// The modified entity.
        /// </value>
        public object ModifiedEntity { get; }

        /// <summary>
        /// Gets the parts of an aggregated entity as a flattened graph.
        /// </summary>
        /// <value>
        /// The flattened entity graph.
        /// </value>
        public IEnumerable<object> FlattenedEntityGraph { get; }
    }
}
