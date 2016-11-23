// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeStateTrackable.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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