// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnqueueMessage.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.Endpoints
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Kephas.Dynamic;
    using Kephas.Messaging;
    using Kephas.Messaging.Messages;
    using Kephas.Operations;
    using Kephas.Scheduling.Reflection;
    using Kephas.Scheduling.Triggers;
    using Kephas.Workflow;

    /// <summary>
    /// Message for enqueuing a job.
    /// </summary>
    [Display(Description = "Enqueues a new job.")]
    public class EnqueueMessage : IMessage
    {
        /// <summary>
        /// Gets or sets the scheduled job.
        /// </summary>
        public IJobInfo? ScheduledJob { get; set; }

        /// <summary>
        /// Gets or sets the ID of the scheduled job.
        /// </summary>
        public object? ScheduledJobId { get; set; }

        /// <summary>
        /// Gets or sets the trigger.
        /// </summary>
        public ITrigger? Trigger { get; set; }

        /// <summary>
        /// Gets or sets the activity target.
        /// </summary>
        public object? Target { get; set; }

        /// <summary>
        /// Gets or sets the activity arguments.
        /// </summary>
        public IExpando? Arguments { get; set; }

        /// <summary>
        /// Gets or sets the activity options.
        /// </summary>
        public Action<IActivityContext>? Options { get; set; }
    }

    /// <summary>
    /// Response for <see cref="EnqueueMessage"/>.
    /// </summary>
    public class EnqueueResponseMessage : ResponseMessage
    {
        /// <summary>
        /// Gets or sets the result of the enqueue operation.
        /// </summary>
        public IOperationResult<IJobInfo?>? Result { get; set; }
    }
}