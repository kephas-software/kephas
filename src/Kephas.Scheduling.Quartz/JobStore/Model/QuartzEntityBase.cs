// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QuartzEntityBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the quartz entity base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    /// <summary>
    /// A quartz entity base.
    /// </summary>
    public abstract class QuartzEntityBase : IEntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzEntityBase"/> class.
        /// </summary>
        protected QuartzEntityBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzEntityBase"/> class.
        /// </summary>
        /// <param name="instanceName">The name of the instance.</param>
        protected QuartzEntityBase(string instanceName)
        {
            this.InstanceName = instanceName;
        }

        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        /// <value>
        /// The name of the instance.
        /// </value>
        public string InstanceName { get; set; }

        /// <summary>
        /// Gets or sets the identifier for this instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        //TODO [BsonId]
        public object Id { get; set; }
    }
}