// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBehaviorRuleMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the service behavior rule metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Behavior.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// A service behavior rule metadata.
    /// </summary>
    public class ServiceBehaviorRuleMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBehaviorRuleMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ServiceBehaviorRuleMetadata(IDictionary<string, object> metadata)
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
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        /// <param name="optionalService"><c>true</c> if the service is optional, <c>false</c> if not (optional).</param>
        public ServiceBehaviorRuleMetadata(Type serviceContractType, int processingPriority = 0, int overridePriority = 0, bool optionalService = false)
            : base(processingPriority, overridePriority, optionalService)
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