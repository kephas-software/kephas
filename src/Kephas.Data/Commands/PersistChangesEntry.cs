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

    using Kephas.Data.Capabilities;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A modified entry.
    /// </summary>
    public class PersistChangesEntry : EntityInfo, IPersistChangesEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistChangesEntry"/> class.
        /// </summary>
        /// <param name="entity">The modified entity.</param>
        /// <param name="changeState">The change state.</param>
        /// <param name="flattenedEntityGraph">The flattened entity graph.</param>
        public PersistChangesEntry(object entity, ChangeState changeState, IEnumerable<object> flattenedEntityGraph)
            : base(entity, changeState)
        {
            Requires.NotNull(entity, nameof(entity));

            this.FlattenedEntityGraph = flattenedEntityGraph;
        }

        /// <summary>
        /// Gets the parts of an aggregated entity as a flattened graph.
        /// </summary>
        /// <value>
        /// The flattened entity graph.
        /// </value>
        public IEnumerable<object> FlattenedEntityGraph { get; }
    }
}
