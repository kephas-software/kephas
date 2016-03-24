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
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Composition.Metadata;

    /// <summary>
    /// Metadata for application services.
    /// </summary>
    public class AppServiceMetadata : ExportMetadataBase
    {
        /// <summary>
        /// The processing priority metadata key.
        /// </summary>
        public static readonly string ProcessingPriorityKey = nameof(ProcessingPriority);

        /// <summary>
        /// The override priority metadata key.
        /// </summary>
        public static readonly string OverridePriorityKey = nameof(OverridePriority);

        /// <summary>
        /// The processing priority metadata key.
        /// </summary>
        public static readonly string OptionalServiceKey = nameof(OptionalService);

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

            this.ProcessingPriority = (int)(metadata.TryGetValue(ProcessingPriorityKey, 0) ?? 0);
            this.OverridePriority = (int)(metadata.TryGetValue(OverridePriorityKey, 0) ?? 0);
            this.OptionalService = (bool)(metadata.TryGetValue(OptionalServiceKey, false) ?? false);
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
    }
}