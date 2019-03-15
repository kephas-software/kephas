// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullTriggerFactory.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the null trigger factory class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Quartz.JobStore
{
    using Kephas.Scheduling.Quartz.JobStore.Model;
    using Kephas.Services;

    using ITrigger = Kephas.Scheduling.ITrigger;

    /// <summary>
    /// A null trigger factory.
    /// </summary>
    [OverridePriority(Priority.Lowest)]
    public class NullTriggerFactory : ITriggerFactory
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
        public Model.ITrigger CreateTrigger(global::Quartz.ITrigger trigger, TriggerState state, string instanceName)
        {
            throw new NullServiceException(typeof(ITriggerFactory));
        }
    }
}