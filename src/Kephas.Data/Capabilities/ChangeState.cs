// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeState.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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