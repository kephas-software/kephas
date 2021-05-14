// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeAssociationMultiplicity.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Model.Associations
{
    /// <summary>
    /// Enumerates multiplicity kinds.
    /// </summary>
    public enum TypeAssociationMultiplicity
    {
        /// <summary>
        /// Multiplicity not provided.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// One instance.
        /// </summary>
        One = 1,

        /// <summary>
        /// Multiple instances.
        /// </summary>
        Many = -1,
    }
}