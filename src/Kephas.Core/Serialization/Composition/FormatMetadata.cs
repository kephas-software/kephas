// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormatMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the format metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Serialization.Composition
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata for <see cref="IFormat"/> services.
    /// </summary>
    public class FormatMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public FormatMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.SupportedMediaTypes = (string[])metadata.TryGetValue(nameof(this.SupportedMediaTypes));
        }

        /// <summary>
        /// Gets the supported data formats.
        /// </summary>
        /// <value>
        /// The supported data formats.
        /// </value>
        public string[] SupportedMediaTypes { get; }
    }
}