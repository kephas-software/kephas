// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGroupedEntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IGroupedEntityBase interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using Kephas.Model.AttributedModel;

    /// <summary>
    /// Interface for grouped entity base.
    /// </summary>
    [Mixin]
    public interface IGroupedEntityBase : IEntityBase
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