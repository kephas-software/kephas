// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the activity context class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;

    using Kephas.Services;

    /// <summary>
    /// An activity context.
    /// </summary>
    public class ActivityContext : Context, IActivityContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityContext"/> class.
        /// </summary>
        /// <param name="parentContext">Optional. Context for the parent.</param>
        public ActivityContext(IActivityContext parentContext = null)
            : base(parentContext)
        {
            this.WorkflowProcessor = parentContext?.WorkflowProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityContext"/> class.
        /// </summary>
        /// <param name="workflowProcessor">The workflow processor.</param>
        public ActivityContext(IWorkflowProcessor workflowProcessor)
        {
            this.WorkflowProcessor = workflowProcessor;
        }

        /// <summary>
        /// Gets the workflow processor.
        /// </summary>
        /// <value>
        /// The workflow processor.
        /// </value>
        public IWorkflowProcessor WorkflowProcessor { get; }

        /// <summary>
        /// Gets or sets the activity being executed.
        /// </summary>
        /// <value>
        /// The activity being executed.
        /// </value>
        public IActivity Activity { get; set; }

        /// <summary>
        /// Gets or sets the execution result.
        /// </summary>
        /// <value>
        /// The execution result.
        /// </value>
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets the execution exception.
        /// </summary>
        /// <value>
        /// The execution exception.
        /// </value>
        public Exception Exception { get; set; }
    }
}