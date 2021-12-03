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

    using Kephas.Diagnostics.Contracts;
    using Kephas.Injection;
    using Kephas.Services;

    /// <summary>
    /// An activity context.
    /// </summary>
    public class ActivityContext : Context, IActivityContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityContext"/> class.
        /// </summary>
        /// <param name="parentContext">Context for the parent.</param>
        /// <param name="isThreadSafe">Optional. True if is thread safe, false if not.</param>
        public ActivityContext(IActivityContext parentContext, bool isThreadSafe = false)
            : base(parentContext, isThreadSafe)
        {
            parentContext = parentContext ?? throw new ArgumentNullException(nameof(parentContext));

            this.WorkflowProcessor = parentContext.WorkflowProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityContext"/> class.
        /// </summary>
        /// <param name="injector">The injector.</param>
        /// <param name="workflowProcessor">The workflow processor.</param>
        /// <param name="isThreadSafe">Optional. True if is thread safe, false if not.</param>
        public ActivityContext(IInjector injector, IWorkflowProcessor workflowProcessor, bool isThreadSafe = false)
            : base(injector, isThreadSafe)
        {
            workflowProcessor = workflowProcessor ?? throw new System.ArgumentNullException(nameof(workflowProcessor));

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
        public IActivity? Activity { get; set; }

        /// <summary>
        /// Gets or sets the execution result.
        /// </summary>
        /// <value>
        /// The execution result.
        /// </value>
        public object? Result { get; set; }

        /// <summary>
        /// Gets or sets the execution exception.
        /// </summary>
        /// <value>
        /// The execution exception.
        /// </value>
        public Exception? Exception { get; set; }

        /// <summary>
        /// Gets or sets the execution timeout.
        /// </summary>
        /// <value>
        /// The execution timeout.
        /// </value>
        public TimeSpan? Timeout { get; set; }
    }
}