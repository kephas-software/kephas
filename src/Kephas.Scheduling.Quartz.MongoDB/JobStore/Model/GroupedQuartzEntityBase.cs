// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupedQuartzEntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the grouped quartz entity base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// A grouped quartz entity base.
    /// </summary>
    [ExcludeFromModel]
    public abstract class GroupedQuartzEntityBase : QuartzEntityBase, IGroupedEntityBase, INamedEntityBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>
        /// The group.
        /// </value>
        public string Group { get; set; }
    }
}