// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PausedTriggerGroup.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the paused trigger group class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Models
{
    using Kephas.Activation;
    using Kephas.Scheduling.Quartz.JobStore.Model;

    /// <summary>
    /// A paused trigger group.
    /// </summary>
    [ImplementationFor(typeof(IPausedTriggerGroup))]
    public class PausedTriggerGroup : QuartzEntityBase, IPausedTriggerGroup
    {
        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public string Group { get; set; }
    }
}