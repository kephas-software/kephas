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
        /// <param name="changeStateTracker">The entity's change state tracker.</param>
        /// <param name="flattenedEntityGraph">The flattened entity graph (optional). If not provided, the entity's graph consists only of the entity.</param>
        public PersistChangesEntry(object entity, IEntityInfo changeStateTracker, IEnumerable<object> flattenedEntityGraph = null)
            : base(entity, changeStateTracker)
        {
            Requires.NotNull(entity, nameof(entity));
            Requires.NotNull(changeStateTracker, nameof(changeStateTracker));

            this.FlattenedEntityGraph = flattenedEntityGraph ?? new[] { entity };

            // take over the ID from the entity info, to identify it easily later.
            this.Id = changeStateTracker.Id;
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
