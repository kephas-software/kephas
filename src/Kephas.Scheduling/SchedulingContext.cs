// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulingContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Kephas.Dynamic;
using Kephas.Injection;
using Kephas.Workflow;

namespace Kephas.Scheduling
{
    using Kephas.Scheduling.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Provides scheduling contextual information.
    /// </summary>
    public class SchedulingContext : Context, ISchedulingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingContext"/> class.
        /// </summary>
        /// <param name="parentContext">The parent context.</param>
        /// <param name="merge">Optional. True to merge the parent context into the new context.</param>
        public SchedulingContext(IContext parentContext, bool merge = false)
            : base(parentContext, isThreadSafe: false, merge)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        public SchedulingContext(IInjector injector)
            : base(injector, isThreadSafe: false)
        {
        }

        /// <summary>
        /// Gets or sets the scheduled job identifier.
        /// </summary>
        public object? ScheduledJobId { get; set; }

        /// <summary>
        /// Gets or sets the scheduled job.
        /// </summary>
        public IJobInfo? ScheduledJob { get; set; }

        /// <summary>
        /// Gets or sets the activity target.
        /// </summary>
        public object? ActivityTarget { get; set; }

        /// <summary>
        /// Gets or sets the activity arguments.
        /// </summary>
        public IDynamic? ActivityArguments { get; set; }

        /// <summary>
        /// Gets or sets the activity options.
        /// </summary>
        public Action<IActivityContext> ActivityOptions { get; set; }
    }
}