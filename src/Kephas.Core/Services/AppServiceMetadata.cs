// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Metadata for application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Injection;
    using Kephas.Model.AttributedModel;
    using Kephas.Runtime;

    /// <summary>
    /// Metadata for application services.
    /// </summary>
    public class AppServiceMetadata : InjectionMetadataBase, IHasProcessingPriority
    {
        private static readonly IAppServiceMetadataResolver MetadataResolver = new AppServiceMetadataResolver(RuntimeTypeRegistry.Instance);

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public AppServiceMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.ProcessingPriority = this.GetMetadataValue<ProcessingPriorityAttribute, Priority>(metadata);
            this.OverridePriority = this.GetMetadataValue<OverridePriorityAttribute, Priority>(metadata);
            this.ServiceName = this.GetMetadataValue<ServiceNameAttribute, string>(metadata);
            this.IsOverride = this.GetMetadataValue<OverrideAttribute, bool>(metadata, (bool)metadata.TryGetValue(nameof(this.IsOverride), false)!);
            this.ServiceInstanceType = (Type?)metadata.TryGetValue(nameof(this.ServiceInstanceType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceMetadata" /> class.
        /// </summary>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. The name of the service.</param>
        /// <param name="isOverride">Optional. Indicates whether the service overrides its base.</param>
        public AppServiceMetadata(Priority processingPriority = 0, Priority overridePriority = 0, string? serviceName = null, bool isOverride = false)
            : base(new Dictionary<string, object?>())
        {
            this.ProcessingPriority = (Priority)processingPriority;
            this.OverridePriority = overridePriority;
            this.ServiceName = serviceName;
            this.IsOverride = isOverride;
        }

        /// <summary>
        /// Gets the order in which the services should be processed.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        public Priority ProcessingPriority { get; }

        /// <summary>
        /// Gets the priority of the service in the override chain.
        /// </summary>
        /// <value>
        /// The override priority.
        /// </value>
        public Priority OverridePriority { get; }

        /// <summary>
        /// Gets a value indicating whether the service overrides the
        /// service it specializes.
        /// </summary>
        /// <value>
        /// True if the service overrides the service it specializes, false otherwise.
        /// </value>
        public bool IsOverride { get; }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string? ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the concrete service type implementing the service contract.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type? ServiceInstanceType { get; set; }

        /// <summary>
        /// Gets or sets the service dependencies.
        /// </summary>
        /// <value>
        /// The service dependencies.
        /// </value>
        public IEnumerable<Type> Dependencies { get; set; } = Enumerable.Empty<Type>();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var serviceName = string.IsNullOrEmpty(this.ServiceName) ? string.Empty : $"Name: {this.ServiceName}, ";
            return $"Override#: {this.OverridePriority}, Processing#: {this.ProcessingPriority}, {serviceName}Impl: {this.ServiceInstanceType}";
        }

        /// <summary>
        /// Gets the metadata value for the specific attribute.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>The metadata value if found, otherwise the default value.</returns>
        protected TValue? GetMetadataValue<TAttribute, TValue>(IDictionary<string, object?> metadata, TValue? defaultValue = default)
            where TAttribute : IMetadataValue<TValue>
        {
            var metadataName = MetadataResolver.GetMetadataNameFromAttributeType(typeof(TAttribute));
            var value = metadata.TryGetValue(metadataName, defaultValue);
            if (value == null && !typeof(TValue).IsByRef)
            {
                return defaultValue;
            }

            return (TValue?)value;
        }
    }
}