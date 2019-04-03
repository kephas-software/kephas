// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorRuleFlowControlBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class controlling the flow of a behavior rule.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Behaviors
{
    using System.Linq;

    using Kephas.Services;

    /// <summary>
    /// Base class controlling the flow of a behavior rule.
    /// </summary>
    public abstract class BehaviorRuleFlowControlBase : IBehaviorRuleFlowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorRuleFlowControlBase"/> class.
        /// </summary>
        protected BehaviorRuleFlowControlBase()
        {
            // ReSharper disable VirtualMemberCallInContructor
            this.ProcessingPriority = this.ComputeProcessingPriority();
            this.IsEndRule = this.ComputeIsEndRule();
        }

        /// <summary>
        /// Gets or sets the processing priority.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        public int ProcessingPriority { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this rule ends the processing flow.
        /// </summary>
        /// <value>
        /// <c>true</c> if this rule ends the processing flow, <c>false</c> if not.
        /// </value>
        public bool IsEndRule { get; protected set; }

        /// <summary>
        /// Calculates the value of the <see cref="IsEndRule"/> property.
        /// </summary>
        /// <returns>
        /// The calculated value of the <see cref="IsEndRule"/> property.
        /// </returns>
        protected virtual bool ComputeIsEndRule()
        {
            var endRuleAttribute = this.GetRuntimeTypeInfo().Annotations.OfType<EndRuleAttribute>().FirstOrDefault();
            return endRuleAttribute?.Value ?? false;
        }

        /// <summary>
        /// Calculates the value of the <see cref="ProcessingPriority"/> property.
        /// </summary>
        /// <returns>
        /// The calculated value of the <see cref="ProcessingPriority"/> property.
        /// </returns>
        protected virtual int ComputeProcessingPriority()
        {
            var priorityOrderAttribute = this.GetRuntimeTypeInfo().Annotations.OfType<ProcessingPriorityAttribute>().FirstOrDefault();
            return priorityOrderAttribute?.Value ?? 0;
        }
    }
}