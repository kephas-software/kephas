// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorRuleFlowControlBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Base class controlling the flow of a behavior rule.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Services.Metadata;
using Microsoft.Extensions.Logging;

namespace Kephas.Behaviors
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// Base class controlling the flow of a behavior rule.
    /// </summary>
    public abstract class BehaviorRuleFlowControlBase : IBehaviorRuleFlowControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BehaviorRuleFlowControlBase"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        protected BehaviorRuleFlowControlBase(ILogger<BehaviorRuleFlowControlBase> logger)
        {
            // ReSharper disable VirtualMemberCallInConstructor
            var metadata = this.ComputeMetadata();
            this.Priority = (Priority)metadata.TryGetValue(nameof(IHasPriority.Priority), Priority.Normal)!;
            this.IsEndRule = (bool)metadata.TryGetValue(nameof(IBehaviorRuleFlowControl.IsEndRule), false)!;
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets or sets the processing priority.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        public Priority Priority { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this rule ends the processing flow.
        /// </summary>
        /// <value>
        /// <c>true</c> if this rule ends the processing flow, <c>false</c> if not.
        /// </value>
        public bool IsEndRule { get; protected set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; private set; }

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
                .OfType<IServiceMetadataProvider>()
                .SelectMany(p => p.GetMetadata())
                .ForEach(kv => metadata[kv.name] = kv.value);
            return metadata;
        }
    }
}
