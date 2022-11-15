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

    using Kephas.Dynamic;
    using Kephas.Services;
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
            this.Scope.Merge(parentContext.Scope);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityContext"/> class.
        /// </summary>
        /// <param name="serviceProvider">The injector.</param>
        /// <param name="workflowProcessor">The workflow processor.</param>
        /// <param name="isThreadSafe">Optional. True if is thread safe, false if not.</param>
        public ActivityContext(IServiceProvider serviceProvider, IWorkflowProcessor workflowProcessor, bool isThreadSafe = false)
            : base(serviceProvider, isThreadSafe)
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

        /// <summary>
        /// Gets or sets the activity scope. Typically it holds working variables.
        /// </summary>
        public IDynamic Scope { get; set; } = new Expando();
    }
}