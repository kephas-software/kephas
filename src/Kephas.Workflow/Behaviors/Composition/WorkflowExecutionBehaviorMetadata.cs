// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkflowExecutionBehaviorMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the workflow execution behavior metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Behaviors.Composition
{
    using System.Collections.Generic;
    using Kephas.Services.Composition;

    /// <summary>
    /// A workflow execution behavior metadata.
    /// </summary>
    public class WorkflowExecutionBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowExecutionBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public WorkflowExecutionBehaviorMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowExecutionBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        /// <param name="optionalService"><c>true</c> if the service is optional, <c>false</c> if not (optional).</param>
        /// <param name="serviceName">The name of the service (optional).</param>
        public WorkflowExecutionBehaviorMetadata(int processingPriority = 0, int overridePriority = 0, bool optionalService = false, string serviceName = null)
            : base(processingPriority, overridePriority, optionalService, serviceName)
        {
        }
    }
}