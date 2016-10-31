// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPersistChangesEntry.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IPersistChangesEntry interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System.Collections.Generic;

    using Kephas.Data.Behaviors;
    using Kephas.Services;

    /// <summary>
    /// Interface for an entry in the persist changes flow.
    /// </summary>
    public interface IPersistChangesEntry : IContext
    {
        /// <summary>
        /// Gets the change state.
        /// </summary>
        /// <value>
        /// The change state.
        /// </value>
        ChangeState ChangeState { get; }

        /// <summary>
        /// Gets the modified entity.
        /// </summary>
        /// <value>
        /// The modified entity.
        /// </value>
        object ModifiedEntity { get; }

        /// <summary>
        /// Gets the parts of an aggregated entity as a flattened graph.
        /// </summary>
        /// <value>
        /// The flattened entity graph.
        /// </value>
        IEnumerable<object> FlattenedEntityGraph { get; }
    }
}