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

    using Kephas.Data.Capabilities;

    /// <summary>
    /// Interface for an entry in the persist changes flow.
    /// </summary>
    public interface IPersistChangesEntry : IEntityInfo
    {
        /// <summary>
        /// Gets the parts of an aggregated entity as a flattened graph.
        /// </summary>
        /// <value>
        /// The flattened entity graph.
        /// </value>
        IEnumerable<object> FlattenedEntityGraph { get; }
    }
}