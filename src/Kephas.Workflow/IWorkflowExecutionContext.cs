// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkflowExecutionContext.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the IWorkflowExecutionContext interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow
{
    using System;

    using Kephas.Dynamic;
    using Kephas.Services;

    /// <summary>
    /// Interface for workflow execution context.
    /// </summary>
    public interface IWorkflowExecutionContext : IContext
    {
        /// <summary>
        /// Gets the message processor.
        /// </summary>
        /// <value>
        /// The message processor.
        /// </value>
        IWorkflowEngine WorkflowEngine { get; }

        /// <summary>
        /// Gets or sets the activity being executed.
        /// </summary>
        /// <remarks>
        /// The activity's input parameters are contained
        /// in the activity itself as members.
        /// </remarks>
        /// <value>
        /// The activity being executed.
        /// </value>
        IActivity Activity { get; set; }

        /// <summary>
        /// Gets or sets the execution result.
        /// </summary>
        /// <remarks>
        /// The result contains as values the activity's output parameters.
        /// </remarks>
        /// <value>
        /// The execution result.
        /// </value>
        IExpando Result { get; set; }

        /// <summary>
        /// Gets or sets the execution exception.
        /// </summary>
        /// <value>
        /// The execution exception.
        /// </value>
        Exception Exception { get; set; }
    }
}