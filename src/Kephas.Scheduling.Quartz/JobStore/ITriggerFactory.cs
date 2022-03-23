// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITriggerFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ITriggerFactory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using Kephas.Services;

    /// <summary>
    /// Contract for a singleton application service factory creating trigger instances out of Quartz triggers.
    /// </summary>
    [SingletonAppServiceContract]
    public interface ITriggerFactory
    {
        /// <summary>
        /// Creates a trigger.
        /// </summary>
        /// <param name="trigger">The trigger.</param>
        /// <param name="state">The state.</param>
        /// <param name="instanceName">Name of the instance.</param>
        /// <returns>
        /// The new trigger.
        /// </returns>
        Model.ITrigger CreateTrigger(global::Quartz.ITrigger trigger, Model.TriggerState state, string instanceName);
    }
}