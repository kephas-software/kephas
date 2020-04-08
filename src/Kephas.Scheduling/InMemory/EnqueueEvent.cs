// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnqueueEvent.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the enqueue event class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scheduling.InMemory
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Workflow;

    /// <summary>
    /// An enqueue event.
    /// </summary>
    public class EnqueueEvent
    {
        /// <summary>
        /// Gets or sets the job information.
        /// </summary>
        public object JobInfo { get; set; }

        /// <summary>
        /// Gets or sets the trigger ID.
        /// </summary>
        public object? TriggerId { get; set; }

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        public object? Target { get; set; }

        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        public IExpando? Arguments { get; set; }

        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        public Action<IActivityContext>? Options { get; set; }
    }
}