// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppServiceMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Metadata for application services.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Services
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Composition.Metadata;
    using Kephas.Services.Composition;

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
            this.OptionalService = this.GetMetadataValue<OptionalServiceAttribute, bool>(metadata);
            this.AppServiceImplementationType = (Type)metadata.TryGetValue(nameof(this.AppServiceImplementationType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceMetadata" /> class.
        /// </summary>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">  The override priority.</param>
        /// <param name="optionalService">   <c>true</c> if the service is optional, <c>false</c> if not.</param>
        public AppServiceMetadata(int processingPriority = 0, int overridePriority = 0, bool optionalService = false)
            : base(null)
        {
            this.ProcessingPriority = processingPriority;
            this.OverridePriority = overridePriority;
            this.OptionalService = optionalService;
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
        /// Gets or sets the concrete service type implementing the service contract.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public Type AppServiceImplementationType { get; set; }

        /// <summary>
        /// Gets the metadata value for the specific attribute.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <returns>The metadata value if found, otherwise the default value.</returns>
        protected TValue GetMetadataValue<TAttribute, TValue>(IDictionary<string, object> metadata, TValue defaultValue = default(TValue))
            where TAttribute : IMetadataValue<TValue>
        {
            var metadataName = AppServiceConventionsRegistrarBase.GetMetadataNameFromAttributeType(typeof(TAttribute));
            var value = metadata.TryGetValue(metadataName, defaultValue);
            if (value == null && !typeof(TValue).IsByRef)
            {
              return defaultValue;
            }

            return (TValue)value;
        }
    }
}