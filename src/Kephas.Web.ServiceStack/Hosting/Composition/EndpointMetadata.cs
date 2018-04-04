// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EndpointMetadata.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the endpoint metadata class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ServiceStack.Hosting.Composition
{
    using System.Collections.Generic;

    using Kephas.Application;
    using Kephas.Services.Composition;

    /// <summary>
    /// A web endpoint metadata.
    /// </summary>
    public class EndpointMetadata : AppServiceMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointMetadata"/> class.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        public EndpointMetadata(IDictionary<string, object> metadata)
            : base(metadata)
        {
            if (metadata == null)
            {
                return;
            }

            this.RequiredFeature = this.GetMetadataValue<RequiredFeatureAttribute, string>(metadata, null);
        }

        /// <summary>
        /// Gets the required feature.
        /// </summary>
        /// <value>
        /// The required feature.
        /// </value>
        public string RequiredFeature { get; }
    }
}