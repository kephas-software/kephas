// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeStateTrackable.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IChangeStateTrackable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    /// <summary>
    /// Contract for an entity's ability of being tracked with respect to its change state.
    /// </summary>
    public interface IChangeStateTrackable
    {
        /// <summary>
        /// Gets or sets the change state of the entity.
        /// </summary>
        /// <value>
        /// The change state.
        /// </value>
        ChangeState ChangeState { get; set; }
    }
}