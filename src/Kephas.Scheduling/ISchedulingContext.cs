// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchedulingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling
{
    using System;

    using Kephas.Diagnostics.Contracts;
    using Kephas.Dynamic;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Triggers;
    using Kephas.Services;
    using Kephas.Workflow;

    /// <summary>
    /// Provides contextual information for scheduling.
    /// </summary>
    public interface ISchedulingContext : IContext
    {
        /// <summary>
        /// Gets or sets the scheduled job identifier.
        /// </summary>
        object? ScheduledJobId { get; set; }

        /// <summary>
        /// Gets or sets the scheduled job.
        /// </summary>
        IJobInfo? ScheduledJob { get; set; }

        /// <summary>
        /// Gets or sets the activity target.
        /// </summary>
        object? ActivityTarget { get; set; }

        /// <summary>
        /// Gets or sets the activity arguments.
        /// </summary>
        IExpando? ActivityArguments { get; set; }

        /// <summary>
        /// Gets or sets the activity options.
        /// </summary>
        public Action<IActivityContext>? ActivityOptions { get; set; }
    }

    /// <summary>
    /// Extension methods for <see cref="ISchedulingContext"/>.
    /// </summary>
    public static class SchedulingContextExtensions
    {
        private const string TriggerKey = "Trigger";

        /// <summary>
        /// Sets the scheduled job identifier.
        /// </summary>
        /// <param name="context">The scheduling context.</param>
        /// <param name="scheduledJobId">The scheduled job identifier.</param>
        /// <typeparam name="TContext">The scheduling context type.</typeparam>
        /// <returns>This scheduling context.</returns>
        public static TContext ScheduledJobId<TContext>(this TContext context, object scheduledJobId)
            where TContext : class, ISchedulingContext
        {
            Requires.NotNull(context, nameof(context));

            context.ScheduledJobId = scheduledJobId;

            return context;
        }

        /// <summary>
        /// Sets the scheduled job.
        /// </summary>
        /// <param name="context">The scheduling context.</param>
        /// <param name="scheduledJob">The scheduled job.</param>
        /// <typeparam name="TContext">The scheduling context type.</typeparam>
        /// <returns>This scheduling context.</returns>
        public static TContext ScheduledJob<TContext>(this TContext context, IJobInfo scheduledJob)
            where TContext : class, ISchedulingContext
        {
            Requires.NotNull(context, nameof(context));

            context.ScheduledJob = scheduledJob;

            return context;
        }

        /// <summary>
        /// Gets or sets the activity options.
        /// </summary>
        /// <param name="context">The scheduling context.</param>
        /// <param name="target">The activity target.</param>
        /// <typeparam name="TContext">The scheduling context type.</typeparam>
        /// <returns>This scheduling context.</returns>
        public static TContext ActivityTarget<TContext>(this TContext context, object? target)
            where TContext : class, ISchedulingContext
        {
            Requires.NotNull(context, nameof(context));

            context.ActivityTarget = target;

            return context;
        }

        /// <summary>
        /// Gets or sets the activity options.
        /// </summary>
        /// <param name="context">The scheduling context.</param>
        /// <param name="arguments">The activity arguments.</param>
        /// <typeparam name="TContext">The scheduling context type.</typeparam>
        /// <returns>This scheduling context.</returns>
        public static TContext ActivityArguments<TContext>(this TContext context, IExpando? arguments)
            where TContext : class, ISchedulingContext
        {
            Requires.NotNull(context, nameof(context));

            context.ActivityArguments = arguments;

            return context;
        }

        /// <summary>
        /// Gets or sets the activity parameters.
        /// </summary>
        /// <param name="context">The scheduling context.</param>
        /// <param name="target">The activity target.</param>
        /// <param name="arguments">Optional. The activity arguments.</param>
        /// <param name="options">Optional. The activity options.</param>
        /// <typeparam name="TContext">The scheduling context type.</typeparam>
        /// <returns>This scheduling context.</returns>
        public static TContext Activity<TContext>(this TContext context, object? target, IExpando? arguments = null, Action<IActivityContext>? options = null)
            where TContext : class, ISchedulingContext
        {
            Requires.NotNull(context, nameof(context));

            context.ActivityTarget = target;
            context.ActivityArguments = arguments;
            context.ActivityOptions = options;

            return context;
        }

        /// <summary>
        /// Sets the provided trigger.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The scheduling context.</param>
        /// <param name="trigger">The trigger.</param>
        /// <returns>
        /// A TContext.
        /// </returns>
        public static TContext Trigger<TContext>(this TContext context, ITrigger? trigger)
            where TContext : ISchedulingContext
        {
            context[TriggerKey] = trigger;

            return context;
        }

        /// <summary>
        /// Gets the trigger.
        /// </summary>
        /// <typeparam name="TContext">Type of the context.</typeparam>
        /// <param name="context">The scheduling context.</param>
        /// <returns>
        /// A trigger or <c>null</c>.
        /// </returns>
        public static ITrigger? Trigger<TContext>(this TContext context)
            where TContext : ISchedulingContext
        {
            return context[TriggerKey] as ITrigger;
        }
    }
}