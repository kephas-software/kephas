// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeStateTrackableEntityInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IChangeStateTrackableEntityInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Capabilities
{
    /// <summary>
    /// Interface for trackable entity information.
    /// </summary>
    public interface IChangeStateTrackableEntityInfo : IChangeStateTrackable
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        object Entity { get; }
    }
}