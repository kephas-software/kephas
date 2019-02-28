// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedEntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the INamedEntityBase interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    /// <summary>
    /// Interface for named entity base.
    /// </summary>
    public interface INamedEntityBase : IEntityBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }
    }
}