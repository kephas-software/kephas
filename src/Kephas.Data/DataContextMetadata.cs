// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContextMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data context metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using Kephas.Collections;
    using Kephas.Services;

    /// <summary>
    /// Metadata for <see cref="IDataContext"/> services.
    /// </summary>
    public class DataContextMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataContextMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.SupportedDataStoreKinds = (IEnumerable<string>)metadata.TryGetValue(nameof(this.SupportedDataStoreKinds), Enumerable.Empty<string>())!;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataContextMetadata" /> class.
        /// </summary>
        /// <param name="supportedDataStoreKinds">The supported data store kinds.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public DataContextMetadata(IEnumerable<string>? supportedDataStoreKinds, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.SupportedDataStoreKinds = supportedDataStoreKinds ?? Enumerable.Empty<string>();
        }

        /// <summary>
        /// Gets the kind of the supported data store.
        /// </summary>
        /// <value>
        /// The data store.
        /// </value>
        public IEnumerable<string> SupportedDataStoreKinds { get; }
    }
}