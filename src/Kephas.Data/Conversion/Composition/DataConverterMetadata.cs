// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataConverterMetadata.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the data converter metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Conversion.Composition
{
    using System;
    using System.Collections.Generic;
    using Kephas.Collections;
    using Kephas.Services.Composition;

    /// <summary>
    /// A metadata information for <see cref="IDataConverter"/>.
    /// </summary>
    public class DataConverterMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConverterMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataConverterMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.SourceType = (Type)metadata.TryGetValue(nameof(this.SourceType));
            this.TargetType = (Type)metadata.TryGetValue(nameof(this.TargetType));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConverterMetadata"/>
        ///  class.
        /// </summary>
        /// <param name="sourceType">The type of the source.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="processingPriority">Optional. The processing priority.</param>
        /// <param name="overridePriority">Optional. The override priority.</param>
        public DataConverterMetadata(Type sourceType, Type targetType, int processingPriority = 0, int overridePriority = 0)
            : base(processingPriority, overridePriority)
        {
            this.SourceType = sourceType;
            this.TargetType = targetType;
        }

        /// <summary>
        /// Gets the type of the source.
        /// </summary>
        /// <value>
        /// The type of the source.
        /// </value>
        public Type SourceType { get; }

        /// <summary>
        /// Gets the type of the target.
        /// </summary>
        /// <value>
        /// The type of the target.
        /// </value>
        public Type TargetType { get; }
    }
}