// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeState.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the change state class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    /// <summary>
    /// Enumerates the entity change state.
    /// </summary>
    public enum ChangeState
    {
        /// <summary>
        /// The entity is not changed.
        /// </summary>
        NotChanged,

        /// <summary>
        /// The entity is added.
        /// </summary>
        Added,

        /// <summary>
        /// The entity is changed.
        /// </summary>
        Changed,

        /// <summary>
        /// The entity is added or updated.
        /// </summary>
        AddedOrChanged,

        /// <summary>
        /// The entity is deleted.
        /// </summary>
        Deleted,
    }
}