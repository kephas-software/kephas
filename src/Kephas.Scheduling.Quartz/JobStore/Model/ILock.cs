// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILock.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ILock interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore.Model
{
    using System;

    using Kephas.Data.Model.AttributedModel;

    /// <summary>
    /// Values that represent lock types.
    /// </summary>
    /// <seealso/>
    public enum LockType
    {
        /// <summary>
        /// An enum constant representing the trigger access option.
        /// </summary>
        TriggerAccess,

        /// <summary>
        /// An enum constant representing the state access option.
        /// </summary>
        StateAccess,
    }

    /// <summary>
    /// Interface for lock.
    /// </summary>
    [NaturalKey(new[] { nameof(InstanceName), nameof(LockType) })]
    public interface ILock : IEntityBase
    {
        /// <summary>
        /// Gets or sets the type of the lock.
        /// </summary>
        /// <value>
        /// The type of the lock.
        /// </value>
        LockType LockType { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the instance.
        /// </summary>
        /// <value>
        /// The identifier of the instance.
        /// </value>
        string InstanceId { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the acquired at.
        /// </summary>
        /// <value>
        /// The acquired at time.
        /// </value>
        DateTime AcquiredAt { get; set; }
    }
}