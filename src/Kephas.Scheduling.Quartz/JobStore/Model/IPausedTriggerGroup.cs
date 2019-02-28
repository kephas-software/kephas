// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPausedTriggerGroup.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IPausedTriggerGroup interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using Kephas.Data.Model.AttributedModel;

    /// <summary>
    /// Interface for paused trigger group.
    /// </summary>
    [NaturalKey(new[] { nameof(InstanceName), nameof(Group) })]
    public interface IPausedTriggerGroup : IEntityBase
    {
        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        string Group { get; set; }
    }
}