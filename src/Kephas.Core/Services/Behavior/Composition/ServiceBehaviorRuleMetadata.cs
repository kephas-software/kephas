// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceBehaviorRuleMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
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
        /// Gets the type of the service contract.
        /// </summary>
        /// <value>
        /// The type of the service contract.
        /// </value>
        public Type ServiceContractType { get; }
    }
}