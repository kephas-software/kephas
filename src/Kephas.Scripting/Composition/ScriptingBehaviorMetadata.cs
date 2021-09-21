// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScriptingBehaviorMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the scripting behavior metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Composition
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;
    using Kephas.Services.Composition;

    /// <summary>
    /// A scripting behavior metadata.
    /// </summary>
    public class ScriptingBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public ScriptingBehaviorMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.Language = (string[]?)metadata.TryGetValue(nameof(this.Language));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptingBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="language">The supported language set.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="serviceName">Optional. Name of the service.</param>
        public ScriptingBehaviorMetadata(string[] language, Priority processingPriority = 0, Priority overridePriority = 0, string? serviceName = null)
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
        public string[]? Language { get; }
    }
}