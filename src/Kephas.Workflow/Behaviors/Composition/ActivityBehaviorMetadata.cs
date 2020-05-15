// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivityBehaviorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the workflow execution behavior metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Workflow.Behaviors.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata class for activity behaviors.
    /// </summary>
    public class ActivityBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ActivityBehaviorMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.ActivityType = (Type)metadata.TryGetValue(nameof(this.ActivityType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="activityType">The type of the activity.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. The name of the service.</param>
        public ActivityBehaviorMetadata(Type activityType, int processingPriority = 0, int overridePriority = 0, string? serviceName = null)
            : base(processingPriority, overridePriority, serviceName)
        {
            Requires.NotNull(activityType, nameof(activityType));

            this.ActivityType = activityType;
        }

        /// <summary>
        /// Gets the type of the activity.
        /// </summary>
        /// <value>
        /// The type of the activity.
        /// </value>
        public Type ActivityType { get; }
    }
}