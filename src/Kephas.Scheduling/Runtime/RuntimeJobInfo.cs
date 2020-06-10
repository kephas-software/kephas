// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RuntimeJobInfo.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the runtime job information class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Kephas.Dynamic;
    using Kephas.Runtime;
    using Kephas.Scheduling.Jobs;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Triggers;
    using Kephas.Workflow;
    using Kephas.Workflow.Runtime;

    /// <summary>
    /// Information about the runtime job.
    /// </summary>
    public class RuntimeJobInfo : RuntimeActivityInfo, IJobInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuntimeJobInfo"/> class.
        /// </summary>
        /// <param name="typeRegistry">The type registry.</param>
        /// <param name="type">The type.</param>
        protected internal RuntimeJobInfo(IRuntimeTypeRegistry typeRegistry, Type type)
            : base(typeRegistry, type)
        {
        }

        /// <summary>
        /// Gets the job information ID.
        /// </summary>
        public object Id { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets the job triggers.
        /// </summary>
        /// <value>
        /// The job triggers.
        /// </value>
        public IEnumerable<ITrigger> Triggers { get; } = new HashSet<ITrigger>();

        /// <summary>
        /// Adds a trigger to the collection of triggers.
        /// </summary>
        /// <param name="trigger">The trigger to add.</param>
        /// <returns>
        /// A value indicating whether the trigger was added to the collection.
        /// </returns>
        public bool AddTrigger(ITrigger trigger)
        {
            return ((HashSet<ITrigger>)this.Triggers).Add(trigger);
        }

        /// <summary>
        /// Removes a trigger from the collection of triggers.
        /// </summary>
        /// <param name="trigger">The trigger to remove.</param>
        /// <returns>
        /// A value indicating whether the trigger was removed from the collection.
        /// </returns>
        public bool RemoveTrigger(ITrigger trigger)
        {
            return ((HashSet<ITrigger>)this.Triggers).Remove(trigger);
        }

        /// <summary>
        /// Executes the job asynchronously.
        /// </summary>
        /// <param name="job">The job to execute.</param>
        /// <param name="target">The job target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the output.
        /// </returns>
        public virtual Task<object?> ExecuteAsync(
            IJob job,
            object? target,
            IExpando? arguments,
            IActivityContext context,
            CancellationToken cancellationToken = default)
        {
            return base.ExecuteAsync(job, target, arguments, context, cancellationToken);
        }

        /// <summary>
        /// Executes the activity asynchronously.
        /// </summary>
        /// <param name="activity">The activity to execute.</param>
        /// <param name="target">The activity target.</param>
        /// <param name="arguments">The execution arguments.</param>
        /// <param name="context">The execution context.</param>
        /// <param name="cancellationToken">Optional. The cancellation token.</param>
        /// <returns>
        /// An asynchronous result that yields the output.
        /// </returns>
        public override Task<object?> ExecuteAsync(IActivity activity, object? target, IExpando? arguments, IActivityContext context, CancellationToken cancellationToken = default)
        {
            if (activity is IJob job)
            {
                return this.ExecuteAsync(job, target, arguments, context, cancellationToken);
            }

            throw new ArgumentException($"The provided activity must be an {nameof(IJob)} to execute.", nameof(activity));
        }
    }
}