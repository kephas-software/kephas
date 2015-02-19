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

    using Kephas.Composition.Metadata;
    using Kephas.Reflection;

    /// <summary>
    /// Metadata for application services.
    /// </summary>
    public class AppServiceMetadata : ExportMetadataBase
    {
        /// <summary>
        /// The processing priority metadata key.
        /// </summary>
        public static readonly string ProcessingPriorityKey = ReflectionHelper.GetPropertyName<AppServiceMetadata>(m => m.ProcessingPriority);

        /// <summary>
        /// The override priority metadata key.
        /// </summary>
        public static readonly string OverridePriorityKey = ReflectionHelper.GetPropertyName<AppServiceMetadata>(m => m.OverridePriority);

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

            object value;
            if (metadata.TryGetValue(ProcessingPriorityKey, out value))
            {
                this.ProcessingPriority = value == null ? 0 : (int)value;
            }

            if (metadata.TryGetValue(OverridePriorityKey, out value))
            {
                this.OverridePriority = (Priority)value;
            }
            else
            {
                this.OverridePriority = Priority.Normal;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppServiceMetadata" /> class.
        /// </summary>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">The override priority.</param>
        public AppServiceMetadata(int processingPriority = 0, Priority overridePriority = Priority.Normal)
            : base(null)
        {
            this.ProcessingPriority = processingPriority;
            this.OverridePriority = overridePriority;
        }

        /// <summary>
        /// Gets the order in which the services should be processed.
        /// </summary>
        /// <value>
        /// The processing priority.
        /// </value>
        public int ProcessingPriority { get; private set; }

        /// <summary>
        /// Gets the priority of the service in the override chain.
        /// </summary>
        /// <value>
        /// The override priority.
        /// </value>
        public Priority OverridePriority { get; private set; }
    }
}