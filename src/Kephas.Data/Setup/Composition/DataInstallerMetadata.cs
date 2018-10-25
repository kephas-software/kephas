// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataInstallerMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data installer metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Setup.Composition
{
    using System.Collections.Generic;

    using Kephas.Data.Setup.AttributedModel;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata for data installer services.
    /// </summary>
    public class DataInstallerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataInstallerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataInstallerMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.DataKind = this.GetMetadataValue<DataKindAttribute, string>(metadata);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataInstallerMetadata" /> class.
        /// </summary>
        /// <param name="dataKind">The data kind.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        /// <param name="optionalService">Optional. <c>true</c> if the service is optional, <c>false</c> if
        ///                               not.</param>
        /// <param name="serviceName">Optional. The name of the service.</param>
        public DataInstallerMetadata(string dataKind, int processingPriority = 0, int overridePriority = 0, bool optionalService = false, string serviceName = null)
            : base(processingPriority, overridePriority, optionalService, serviceName)
        {
            this.DataKind = dataKind;
        }

        /// <summary>
        /// Gets the data kind.
        /// </summary>
        /// <value>
        /// The data kind.
        /// </value>
        public string DataKind { get; }
    }
}