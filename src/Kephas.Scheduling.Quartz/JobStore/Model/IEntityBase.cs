// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IEntityBase interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using Kephas.Data;

    /// <summary>
    /// Interface for entity base.
    /// </summary>
    public interface IEntityBase : IIdentifiable
    {
        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        /// <value>
        /// The name of the instance.
        /// </value>
        string InstanceName { get; set; }
    }
}