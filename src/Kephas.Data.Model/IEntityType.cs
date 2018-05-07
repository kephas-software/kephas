// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityType.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    public interface IEntityType : IClassifier
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