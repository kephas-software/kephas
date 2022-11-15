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
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Injection;
    using Kephas.Logging;
    using Kephas.Services;

    /// <summary>
    /// Base class controlling the flow of a behavior rule.
    /// </summary>
    public abstract class BehaviorRuleFlowControlBase : Loggable, IBehaviorRuleFlowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorRuleFlowControlBase"/> class.
        /// </summary>
        /// <param name="logManager">Optional. The log manager.</param>
        protected BehaviorRuleFlowControlBase(ILogManager? logManager = null)
            : base(logManager)
        {
            // ReSharper disable VirtualMemberCallInConstructor
            var metadata = this.ComputeMetadata();
            this.ProcessingPriority = (Priority)metadata.TryGetValue(nameof(IHasProcessingPriority.ProcessingPriority), Priority.Normal)!;
            this.IsEndRule = (bool)metadata.TryGetValue(nameof(IBehaviorRuleFlowControl.IsEndRule), false)!;
        }

        /// <summary>
        /// Gets or sets the processing priority.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        public Priority ProcessingPriority { get; protected set; }

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
        protected virtual IDictionary<string, object?> ComputeMetadata()
        {
            var metadata = new Dictionary<string, object?>();
            this.GetType().GetCustomAttributes(inherit: true)
                .OfType<IMetadataProvider>()
                .SelectMany(p => p.GetMetadata())
                .ForEach(kv => metadata[kv.name] = kv.value);
            return metadata;
        }
    }
}