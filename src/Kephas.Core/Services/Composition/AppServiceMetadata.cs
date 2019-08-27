// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Metadata for application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Composition.Metadata;

    /// <summary>
    /// Metadata for application services.
    /// </summary>
    public class AppServiceMetadata : ExportMetadataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public AppServiceMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.ProcessingPriority = this.GetMetadataValue<ProcessingPriorityAttribute, int>(metadata);
            this.OverridePriority = this.GetMetadataValue<OverridePriorityAttribute, int>(metadata);
            this.ServiceName = this.GetMetadataValue<ServiceNameAttribute, string>(metadata);
            this.AppServiceImplementationType = (Type)metadata.TryGetValue(nameof(this.AppServiceImplementationType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceMetadata" /> class.
        /// </summary>
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        /// <param name="optionalService"><c>true</c> if the service is optional, <c>false</c> if not (optional).</param>
        /// <param name="serviceName">The name of the service (optional).</param>
        public AppServiceMetadata(int processingPriority = 0, int overridePriority = 0, bool optionalService = false, string serviceName = null)
            : base(null)
        {
            this.ProcessingPriority = processingPriority;
            this.OverridePriority = overridePriority;
            this.OptionalService = optionalService;
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Gets the order in which the services should be processed.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        public int ProcessingPriority { get; }

        /// <summary>
        /// Gets the priority of the service in the override chain.
        /// </summary>
        /// <value>
        /// The override priority.
        /// </value>
        public int OverridePriority { get; }

        /// <summary>
        /// Gets a value indicating whether the decorated service is optional.
        /// </summary>
        /// <value>
        /// <c>true</c> if the service is optional, <c>false</c> if not.
        /// </value>
        public bool OptionalService { get; }

        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the concrete service type implementing the service contract.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type AppServiceImplementationType { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var serviceName = string.IsNullOrEmpty(this.ServiceName) ? string.Empty : $"Name: {this.ServiceName}, ";
            var optional = this.OptionalService ? "Optional, " : string.Empty;
            return $"Override#: {this.OverridePriority}, Processing#: {this.ProcessingPriority}, {serviceName}{optional}Impl: {this.AppServiceImplementationType}";
        }

        /// <summary>
        /// Gets the metadata value for the specific attribute.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>The metadata value if found, otherwise the default value.</returns>
        protected TValue GetMetadataValue<TAttribute, TValue>(IDictionary<string, object> metadata, TValue defaultValue = default)
            where TAttribute : IMetadataValue<TValue>
        {
            var metadataName = AppServiceInfoConventionsRegistrar.GetMetadataNameFromAttributeType(typeof(TAttribute));
            var value = metadata.TryGetValue(metadataName, defaultValue);
            if (value == null && !typeof(TValue).IsByRef)
            {
                return defaultValue;
            }

            return (TValue)value;
        }
    }
}