// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBehaviorRuleMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service behavior rule metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behaviors
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A service behavior rule metadata.
    /// </summary>
    public class ServiceBehaviorRuleMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorRuleMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ServiceBehaviorRuleMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.ServiceContractType = (Type)metadata.TryGetValue(nameof(this.ServiceContractType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorRuleMetadata"/> class.
        /// </summary>
        /// <param name="serviceContractType">Type of the service contract.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public ServiceBehaviorRuleMetadata(Type serviceContractType, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            Requires.NotNull(serviceContractType, nameof(serviceContractType));

            this.ServiceContractType = serviceContractType;
        }

        /// <summary>
        /// Gets the type of the service contract.
        /// </summary>
        /// <value>
        /// The type of the service contract.
        /// </value>
        public Type ServiceContractType { get; }
    }
}