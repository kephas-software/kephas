// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestHandlerMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
// </copyright>
// <summary>
//   Composition metadata for <see cref="IRequestHandler" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.RequestProcessing.Server
{
    using System;
    using System.Collections.Generic;

    using Kephas.Reflection;
    using Kephas.Services;

    /// <summary>
    /// Composition metadata for <see cref="IRequestHandler"/>.
    /// </summary>
    public class RequestHandlerMetadata : AppServiceMetadata
    {
        /// <summary>
        /// The request type metadata key.
        /// </summary>
        public static readonly string RequestTypeKey = ReflectionHelper.GetPropertyName<RequestProcessingFilterMetadata>(m => m.RequestType);

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestHandlerMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public RequestHandlerMetadata(IDictionary<string, object> metadata)
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
        /// Initializes a new instance of the <see cref="RequestHandlerMetadata" /> class.
        /// </summary>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="overridePriority">The override priority.</param>
        public RequestHandlerMetadata(Type requestType, Priority overridePriority = Priority.Normal)
            : base(0, overridePriority)
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