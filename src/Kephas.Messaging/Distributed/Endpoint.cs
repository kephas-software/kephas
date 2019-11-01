// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Endpoint.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the endpoint class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Messaging.Distributed
{
    using System;

    using Kephas.Application;
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
            (this.Scheme, this.AppId, this.AppInstanceId, this.EndpointId) = this.ComputeAppEndpoint(url);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        /// <param name="appId">Optional. The identifier of the application.</param>
        /// <param name="appInstanceId">Optional. The identifier of the application instance.</param>
        /// <param name="endpointId">Optional. The identifier of the endpoint.</param>
        /// <param name="scheme">Optional. The scheme.</param>
        public Endpoint(string appId = null, string appInstanceId = null, string endpointId = null, string scheme = null)
        {
            this.Scheme = scheme ?? AppScheme;
            this.AppId = appId;
            this.AppInstanceId = appInstanceId;
            this.EndpointId = endpointId;
            this.Url = this.ComputeAppSchemeUrl(scheme, appId, appInstanceId, endpointId);
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
                    (this.Scheme, this.AppId, this.AppInstanceId, this.EndpointId) = this.ComputeAppEndpoint(value);
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

        /// <summary>
        /// Gets the scheme.
        /// </summary>
        /// <value>
        /// The scheme.
        /// </value>
        public string Scheme { get; private set; }

        /// <summary>
        /// Creates a new application instance endpoint.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="endpointId">Optional. The identifier of the endpoint.</param>
        /// <param name="scheme">Optional. The scheme.</param>
        /// <returns>
        /// An endpoint.
        /// </returns>
        public static Endpoint CreateAppInstanceEndpoint(IAppRuntime appRuntime, string endpointId = null, string scheme = null)
        {
            return new Endpoint(appRuntime.GetAppId(), appRuntime.GetAppInstanceId(), endpointId, scheme: scheme);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.Url?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Calculates the application scheme URL.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <param name="appId">The identifier of the application (optional).</param>
        /// <param name="appInstanceId">The identifier of the application instance (optional).</param>
        /// <param name="endpointId">The identifier of the endpoint (optional).</param>
        /// <returns>
        /// The calculated application scheme URL.
        /// </returns>
        private Uri ComputeAppSchemeUrl(string scheme, string appId, string appInstanceId, string endpointId)
        {
            var uriBuilder = new UriBuilder
                                 {
                                     Scheme = scheme ?? AppScheme,
                                     Host = ".",
                                     Path = $"/{appId}/{appInstanceId}/{endpointId}",
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
        private (string scheme, string appId, string appInstanceId, string endpointId) ComputeAppEndpoint(Uri url)
        {
            var pathSegments = url.Segments;
            var appId = this.GetPathSegment(pathSegments, 1);
            var appInstanceId = this.GetPathSegment(pathSegments, 2);
            var endpointId = this.GetPathSegment(pathSegments, 3);
            return (url.Scheme, appId, appInstanceId, endpointId);
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