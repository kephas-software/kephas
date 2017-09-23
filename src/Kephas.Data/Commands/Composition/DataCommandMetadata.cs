// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataCommandMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Implements the data command metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Data.Commands.Composition
{
    using System;
    using System.Collections.Generic;

    using Kephas.Collections;
    using Kephas.Diagnostics.Contracts;
    using Kephas.Services.Composition;

    /// <summary>
    /// Metadata information for <see cref="IDataCommand"/>.
    /// </summary>
    public class DataCommandMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataCommandMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public DataCommandMetadata(IDictionary<string, object> metadata)
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
        /// <param name="processingPriority">The processing priority (optional).</param>
        /// <param name="overridePriority">The override priority (optional).</param>
        /// <param name="optionalService"><c>true</c> if the service is optional, <c>false</c> if not.</param>
        public DataCommandMetadata(Type dataContextType, int processingPriority = 0, int overridePriority = 0, bool optionalService = false)
            : base(processingPriority, overridePriority, optionalService)
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