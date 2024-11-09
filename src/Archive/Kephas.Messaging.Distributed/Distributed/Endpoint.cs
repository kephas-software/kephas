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

    /// <summary>
    /// A messaging endpoint.
    /// </summary>
    public class Endpoint : IEndpoint, IEquatable<Endpoint>, IEquatable<IEndpoint>
    {
        /// <summary>
        /// The application scheme.
        /// </summary>
        public const string AppScheme = "app";

        /// <summary>
        /// The endpoint URL.
        /// </summary>
        private Uri? url;

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
            url = url ?? throw new System.ArgumentNullException(nameof(url));

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
        public Endpoint(string? appId = null, string? appInstanceId = null, string? endpointId = null, string? scheme = null)
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
        public string? EndpointId { get; private set; }

        /// <summary>
        /// Gets the identifier of the application instance.
        /// </summary>
        /// <value>
        /// The identifier of the application instance.
        /// </value>
        public string? AppInstanceId { get; private set; }

        /// <summary>
        /// Gets the identifier of the application.
        /// </summary>
        /// <value>
        /// The identifier of the application.
        /// </value>
        public string? AppId { get; private set; }

        /// <summary>
        /// Gets the scheme.
        /// </summary>
        /// <value>
        /// The scheme.
        /// </value>
        public string? Scheme { get; private set; }

        /// <summary>
        /// Implicit cast that converts the given Endpoint to a string.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator string?(Endpoint? endpoint) => endpoint?.ToString();

        /// <summary>
        /// Implicit cast that converts the given string to an Endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Endpoint?(string? endpoint) => endpoint == null ? null : new Endpoint(new Uri(endpoint));

        /// <summary>
        /// Implicit cast that converts the given Endpoint to an URI.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Uri?(Endpoint? endpoint) => endpoint?.Url;

        /// <summary>
        /// Implicit cast that converts the given URI to an Endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        public static implicit operator Endpoint?(Uri? endpoint) => endpoint == null ? null : new Endpoint(endpoint);

        /// <summary>
        /// Creates a new application instance endpoint.
        /// </summary>
        /// <param name="appRuntime">The application runtime.</param>
        /// <param name="endpointId">Optional. The identifier of the endpoint.</param>
        /// <param name="scheme">Optional. The scheme.</param>
        /// <returns>
        /// An endpoint.
        /// </returns>
        public static IEndpoint CreateAppInstanceEndpoint(IAppRuntime appRuntime, string? endpointId = null, string? scheme = null)
        {
            return new Endpoint(appRuntime.GetAppId(), appRuntime.GetAppInstanceId(), endpointId, scheme: scheme);
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(Endpoint? other) => this.Equals((IEndpoint?)other);

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(IEndpoint? other) => other?.Url.Equals(this.Url) ?? false;

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object? obj) => obj is IEndpoint e && this.Equals(e);

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return this.Url?.GetHashCode() ?? 0;
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return this.Url?.ToString() ?? string.Empty;
        }

        private Uri ComputeAppSchemeUrl(string? scheme, string? appId, string? appInstanceId, string? endpointId)
        {
            var uriBuilder = new UriBuilder
                                 {
                                     Scheme = scheme ?? AppScheme,
                                     Host = ".",
                                     Path = $"/{appId}/{appInstanceId}/{endpointId}",
                                 };
            return uriBuilder.Uri;
        }

        private (string? scheme, string? appId, string? appInstanceId, string? endpointId) ComputeAppEndpoint(Uri url)
        {
            var pathSegments = url.Segments;
            var appId = this.GetPathSegment(pathSegments, 1);
            var appInstanceId = this.GetPathSegment(pathSegments, 2);
            var endpointId = this.GetPathSegment(pathSegments, 3);
            return (url.Scheme, appId, appInstanceId, endpointId);
        }

        private string? GetPathSegment(string[] segments, int index)
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