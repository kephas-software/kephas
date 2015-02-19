// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestProcessingFilterMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Composition metadata for <see cref="IRequestProcessingFilter" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Composition metadata for <see cref="IRequestProcessingFilter"/>.
    /// </summary>
    public class RequestProcessingFilterMetadata : AppServiceMetadata
    {
        /// <summary>
        /// The request type metadata key.
        /// </summary>
        public static readonly string RequestTypeKey = ReflectionHelper.GetPropertyName<RequestProcessingFilterMetadata>(m => m.RequestType);

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProcessingFilterMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public RequestProcessingFilterMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            object value;
            if (metadata.TryGetValue(RequestTypeKey, out value))
            {
                this.RequestType = (Type)value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProcessingFilterMetadata" /> class.
        /// </summary>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="processingPriority">The processing priority.</param>
        /// <param name="overridePriority">The override priority.</param>
        public RequestProcessingFilterMetadata(Type requestType, int processingPriority = 0, Priority overridePriority = Priority.Normal)
            : base(processingPriority, overridePriority)
        {
            this.RequestType = requestType;
        }

        /// <summary>
        /// Gets the type of the request.
        /// </summary>
        /// <value>
        /// The type of the request.
        /// </value>
        public Type RequestType { get; private set; }
    }
}