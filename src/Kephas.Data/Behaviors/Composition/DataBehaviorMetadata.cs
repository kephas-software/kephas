namespace Kephas.Data.Behaviors.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Services;
    using Collections;

    /// <summary>
    /// Metadata for <see cref="IDataBehavior"/> services.
    /// </summary>
    public class DataBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataBehaviorMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.EntityType = (Type)metadata.TryGetValue(nameof(this.EntityType));
        }

        /// <summary>
        /// Gets the type of the entity on which the behavior is applied.
        /// </summary>
        /// <value>
        /// The type of the entity.
        /// </value>
        public Type EntityType { get; }
    }
}