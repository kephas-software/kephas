// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InitialDataHandlerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the initial data handler metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Initialization.Composition
{
    using System.Collections.Generic;

    using Kephas.Data.Initialization.AttributedModel;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata for initial data handler services.
    /// </summary>
    public class InitialDataHandlerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitialDataHandlerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public InitialDataHandlerMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.InitialDataKind = this.GetMetadataValue<InitialDataKindAttribute, string>(metadata);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitialDataHandlerMetadata" /> class.
        /// </summary>
        /// <param name="initialDataKind">The initial data kind.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="optionalService">Optional. <c>true</c> if the service is optional, <c>false</c> if
        ///                               not.</param>
        /// <param name="serviceName">Optional. The name of the service.</param>
        public InitialDataHandlerMetadata(string initialDataKind, int processingPriority = 0, int overridePriority = 0, bool optionalService = false, string serviceName = null)
            : base(processingPriority, overridePriority, optionalService, serviceName)
        {
            this.InitialDataKind = initialDataKind;
        }

        /// <summary>
        /// Gets the data kind.
        /// </summary>
        /// <value>
        /// The data kind.
        /// </value>
        public string InitialDataKind { get; }
    }
}