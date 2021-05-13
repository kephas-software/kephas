// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeAssociationKind.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    /// <summary>
    /// Values that represent type association kinds.
    /// </summary>
    public enum TypeAssociationKind
    {
        /// <summary>
        /// The simple association between two entities.
        /// </summary>
        Simple,

        /// <summary>
        /// An aggregation is an association that represents a part-whole or part-of relationship.
        /// </summary>
        /// <remarks>
        /// When the container is destroyed, the contents are usually not destroyed.
        /// </remarks>
        Aggregation,

        /// <summary>
        /// A composition is a structural aggregation.
        /// </summary>
        /// <remarks>
        /// When the container is destroyed, the contents are also destroyed.
        /// </remarks>
        Composition,
    }
}