// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemplatingEngineMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the templateKind service metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Templating
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// The templating engine service metadata.
    /// </summary>
    public class TemplatingEngineMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatingEngineMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public TemplatingEngineMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.TemplateKind = (string[]?)metadata.TryGetValue(nameof(this.TemplateKind)) ?? Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplatingEngineMetadata"/> class.
        /// </summary>
        /// <param name="templateKind">The supported template kind.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. Name of the service.</param>
        public TemplatingEngineMetadata(string[] templateKind, Priority processingPriority = 0, Priority overridePriority = 0, string? serviceName = null)
            : base(processingPriority, overridePriority, serviceName)
        {
            this.TemplateKind = templateKind ?? throw new ArgumentNullException(nameof(templateKind));
        }

        /// <summary>
        /// Gets the templateKind array.
        /// </summary>
        /// <value>
        /// The templateKind array.
        /// </value>
        public string[] TemplateKind { get; } = Array.Empty<string>();
    }
}