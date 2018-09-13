// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientEntityInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the client entity information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Client.Capabilities
{
    using Kephas.Data.Capabilities;

    /// <summary>
    /// DTO holding information about the entity.
    /// </summary>
    public class ClientEntityInfo : IChangeStateTrackableEntityInfo
    {
        /// <summary>
        /// Gets or sets the change state of the entity.
        /// </summary>
        /// <value>
        /// The change state.
        /// </value>
        public ChangeState ChangeState { get; set; }

        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public object Entity { get; set; }

        /// <summary>
        /// Gets or sets the original identifier of the entity.
        /// </summary>
        /// <remarks>
        /// In case of server generated IDs, the client sets a temporary ID
        /// which it needs to synchronize the internal cache after persisting.
        /// </remarks>
        /// <value>
        /// The original identifier of the entity.
        /// </value>
        public object OriginalEntityId { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity type.
        /// </summary>
        /// <value>
        /// The name of the entity type.
        /// </value>
        public string EntityTypeName { get; set; }
    }
}