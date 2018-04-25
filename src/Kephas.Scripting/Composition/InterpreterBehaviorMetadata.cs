// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterpreterBehaviorMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the interpreter behavior metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Scripting.Composition
{
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// A interpreter behavior metadata.
    /// </summary>
    public class InterpreterBehaviorMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterpreterBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public InterpreterBehaviorMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.Language = (string)metadata.TryGetValue(nameof(this.Language));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpreterBehaviorMetadata"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="processingPriority">(Optional) The processing priority.</param>
        /// <param name="overridePriority">(Optional) The override priority.</param>
        /// <param name="optionalService">(Optional) True to optional service.</param>
        /// <param name="serviceName">(Optional) Name of the service.</param>
        public InterpreterBehaviorMetadata(string language, int processingPriority = 0, int overridePriority = 0, bool optionalService = false, string serviceName = null)
            : base(processingPriority, overridePriority, optionalService, serviceName)
        {
            Requires.NotNullOrEmpty(language, nameof(language));

            this.Language = language;
        }

        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; }
    }
}