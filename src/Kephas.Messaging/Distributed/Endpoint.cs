// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Endpoint.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the endpoint class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;

    using Kephas.Diagnostics.Contracts;

    /// <summary>
    /// A messaging endpoint.
    /// </summary>
    public class Endpoint : IEndpoint
    {
        /// <summary>
        /// The application scheme.
        /// </summary>
        public const string AppScheme = "app";

        /// <summary>
        /// The endpoint URL.
        /// </summary>
        private Uri url;

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        public Endpoint()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="url">The endpoint URL.</param>
        public Endpoint(Uri url)
        {
            Requires.NotNull(url, nameof(url));

            this.Url = url;
            (this.AppId, this.AppInstanceId, this.EndpointId) = this.ComputeAppEndpoint(url);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="appId">The identifier of the application (optional).</param>
        /// <param name="appInstanceId">The identifier of the application instance (optional).</param>
        /// <param name="endpointId">The identifier of the endpoint (optional).</param>
        public Endpoint(string appId = null, string appInstanceId = null, string endpointId = null)
        {
            this.AppId = appId;
            this.AppInstanceId = appInstanceId;
            this.EndpointId = endpointId;
            this.Url = this.ComputeAppSchemeUrl(appId, appInstanceId, endpointId);
        }

        /// <summary>
        /// Gets or sets the URL of the endpoint.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public Uri Url
        {
            get => this.url;
            set
            {
                this.url = value;

                if (value != null)
                {
                    (this.AppId, this.AppInstanceId, this.EndpointId) = this.ComputeAppEndpoint(value);
                }
            }
        }

        /// <summary>
        /// Gets the identifier of the endpoint.
        /// </summary>
        /// <value>
        /// The identifier of the endpoint.
        /// </value>
        public string EndpointId { get; private set; }

        /// <summary>
        /// Gets the identifier of the application instance.
        /// </summary>
        /// <value>
        /// The identifier of the application instance.
        /// </value>
        public string AppInstanceId { get; private set; }

        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        public string AppId { get; private set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.Url?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Calculates the application scheme URL.
        /// </summary>
        /// <param name="appId">The identifier of the application (optional).</param>
        /// <param name="appInstanceId">The identifier of the application instance (optional).</param>
        /// <param name="endpointId">The identifier of the endpoint (optional).</param>
        /// <returns>
        /// The calculated application scheme URL.
        /// </returns>
        private Uri ComputeAppSchemeUrl(string appId, string appInstanceId, string endpointId)
        {
            var uriBuilder = new UriBuilder
                                 {
                                     Scheme = AppScheme, 
                                     Host = ".",
                                     Path = $"/{appId}/{appInstanceId}/{endpointId}"
                                 };
            return uriBuilder.Uri;
        }

        /// <summary>
        /// Calculates the application endpoint.
        /// </summary>
        /// <param name="url">The endpoint URL.</param>
        /// <returns>
        /// The calculated application endpoint.
        /// </returns>
        private (string appId, string appInstanceId, string endpointId) ComputeAppEndpoint(Uri url)
        {
            if (url?.Scheme != AppScheme)
            {
                return (null, null, null);
            }

            var pathSegments = url.Segments;
            var appId = this.GetPathSegment(pathSegments, 1);
            var appInstanceId = this.GetPathSegment(pathSegments, 2);
            var endpointId = this.GetPathSegment(pathSegments, 3);
            return (appId, appInstanceId, endpointId);
        }

        /// <summary>
        /// Gets the path segment with the provided index.
        /// </summary>
        /// <param name="segments">The segments.</param>
        /// <param name="index">Zero-based index of the.</param>
        /// <returns>
        /// The path segment.
        /// </returns>
        private string GetPathSegment(string[] segments, int index)
        {
            if (segments.Length <= index)
            {
                return null;
            }

            var segment = segments[index].Trim('/');
            return string.IsNullOrEmpty(segment) ? null : segment;
        }
    }
}