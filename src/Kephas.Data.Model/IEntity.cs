// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntity.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Declares the IEntity interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model
{
    using System.Collections.Generic;

    using Kephas.Model;

    /// <summary>
    /// An entity denotes classifiers holding metadata about data objects,
    /// typically persisted in the database or transferred to the client tier.
    /// </summary>
    public interface IEntity : IClassifier
    {
        /// <summary>
        /// Gets the entity keys.
        /// </summary>
        /// <value>
        /// The entity keys.
        /// </value>
        IEnumerable<IKey> Keys { get; }
    }
}