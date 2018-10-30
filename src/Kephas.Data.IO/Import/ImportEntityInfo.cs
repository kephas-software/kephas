// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportEntityInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the import entity information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.IO.Import
{
    using Kephas.Data.Capabilities;

    /// <summary>
    /// Information about the import entity.
    /// </summary>
    public class ImportEntityInfo : IChangeStateTrackableEntityInfo
    {
        /// <summary>
        /// Gets or sets the state of the change.
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
    }
}