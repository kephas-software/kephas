// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data command metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands
{
    using System;
    using System.Collections.Generic;
    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services;

    /// <summary>
    /// Metadata information for <see cref="IDataCommand"/>.
    /// </summary>
    public class DataCommandMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommandMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataCommandMetadata(IDictionary<string, object?>? metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.DataContextType = (Type)metadata.TryGetValue(nameof(this.DataContextType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommandMetadata" /> class.
        /// </summary>
        /// <param name="dataContextType">The type of the supported data context.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public DataCommandMetadata(Type dataContextType, Priority processingPriority = 0, Priority overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            Requires.NotNull(dataContextType, nameof(dataContextType));

            this.DataContextType = dataContextType;
        }

        /// <summary>
        /// Gets the type of the supported data context.
        /// </summary>
        /// <value>
        /// The type of the supported data context.
        /// </value>
        public Type DataContextType { get; }
    }
}