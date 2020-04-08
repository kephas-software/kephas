// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JobActivityContextExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the job activity context extensions class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using Kephas.Scheduling.Triggers;
    using Kephas.Workflow;

    /// <summary>
    /// A job activity context extensions.
    /// </summary>
    public static class JobActivityContextExtensions
    {
        private const string TriggerKey = "Trigger";

        /// <summary>
        /// Sets the provided trigger to the activity context.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The activity context.</param>
        /// <param name="trigger">The trigger.</param>
        /// <returns>
        /// A TContext.
        /// </returns>
        public static TContext Trigger<TContext>(this TContext context, ITrigger trigger)
            where TContext : IActivityContext
        {
            context[TriggerKey] = trigger;

            return context;
        }

        /// <summary>
        /// Gets the trigger from the activity context.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The activity context.</param>
        /// <returns>
        /// A trigger or <c>null</c>.
        /// </returns>
        public static ITrigger? Trigger<TContext>(this TContext context)
            where TContext : IActivityContext
        {
            return context[TriggerKey] as ITrigger;
        }
    }
}
