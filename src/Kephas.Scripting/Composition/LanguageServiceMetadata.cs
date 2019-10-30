// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LanguageServiceMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the language service metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Composition
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// The language service metadata.
    /// </summary>
    public class LanguageServiceMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageServiceMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public LanguageServiceMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.Language = (string[])metadata.TryGetValue(nameof(this.Language));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageServiceMetadata"/> class.
        /// </summary>
        /// <param name="language">The supported language set.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. Name of the service.</param>
        public LanguageServiceMetadata(string[] language, int processingPriority = 0, int overridePriority = 0, string serviceName = null)
            : base(processingPriority, overridePriority, serviceName)
        {
            Requires.NotNullOrEmpty(language, nameof(language));

            this.Language = language;
        }

        /// <summary>
        /// Gets the language array.
        /// </summary>
        /// <value>
        /// The language array.
        /// </value>
        public string[] Language { get; }
    }
}