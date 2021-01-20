// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiResourceBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Options
{
    using System;
    using System.Linq;
    using global::IdentityServer4.Models;
    using Kephas.AspNetCore.IdentityServer4.Configuration;

    /// <summary>
    /// A builder for API resources
    /// </summary>
    public class ApiResourceBuilder
    {
        private ApiResource apiResource;
        private bool isBuilt;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResourceBuilder"/> class.
        /// </summary>
        public ApiResourceBuilder()
            : this(new ApiResource())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiResourceBuilder"/> class.
        /// </summary>
        /// <param name="resource">A preconfigured resource.</param>
        public ApiResourceBuilder(ApiResource resource)
        {
            this.apiResource = resource;
        }

        /// <summary>
        /// Creates a new builder for an externally registered API.
        /// </summary>
        /// <param name="name">The name of the API.</param>
        /// <returns>An <see cref="ApiResourceBuilder"/>.</returns>
        public static ApiResourceBuilder ApiResource(string name)
        {
            var apiResource = new ApiResource(name);
            return new ApiResourceBuilder(apiResource)
                .WithApplicationProfile(ApplicationProfiles.API)
                .WithScopes(name);
        }

        /// <summary>
        /// Creates a new builder for an API that coexists with an authorization server.
        /// </summary>
        /// <param name="name">The name of the API.</param>
        /// <returns>An <see cref="ApiResourceBuilder"/>.</returns>
        public static ApiResourceBuilder IdentityServerJwt(string name)
        {
            var apiResource = new ApiResource(name);
            return new ApiResourceBuilder(apiResource)
                .WithApplicationProfile(ApplicationProfiles.IdentityServerJwt);
        }

        /// <summary>
        /// Sets the application profile for the resource.
        /// </summary>
        /// <param name="profile">The the profile for the application from <see cref="ApplicationProfiles"/>.</param>
        /// <returns>The <see cref="ApiResourceBuilder"/>.</returns>
        public ApiResourceBuilder WithApplicationProfile(string profile)
        {
            this.apiResource.Properties.Add(ApplicationProfilesPropertyNames.Profile, profile);
            return this;
        }

        /// <summary>
        /// Adds additional scopes to the API resource.
        /// </summary>
        /// <param name="resourceScopes">The list of scopes.</param>
        /// <returns>The <see cref="ApiResourceBuilder"/>.</returns>
        public ApiResourceBuilder WithScopes(params string[] resourceScopes)
        {
            foreach (var scope in resourceScopes)
            {
                if (this.apiResource.Scopes.Any(s => s == scope))
                {
                    continue;
                }

                this.apiResource.Scopes.Add(scope);
            }

            return this;
        }

        /// <summary>
        /// Replaces the scopes defined for the application with a new set of scopes.
        /// </summary>
        /// <param name="resourceScopes">The list of scopes.</param>
        /// <returns>The <see cref="ApiResourceBuilder"/>.</returns>
        public ApiResourceBuilder ReplaceScopes(params string[] resourceScopes)
        {
            this.apiResource.Scopes.Clear();

            return this.WithScopes(resourceScopes);
        }

        /// <summary>
        /// Configures the API resource to allow all clients to access it.
        /// </summary>
        /// <returns>The <see cref="ApiResourceBuilder"/>.</returns>
        public ApiResourceBuilder AllowAllClients()
        {
            this.apiResource.Properties[ApplicationProfilesPropertyNames.Clients] = ApplicationProfilesPropertyValues.AllowAllApplications;
            return this;
        }

        /// <summary>
        /// Builds the API resource.
        /// </summary>
        /// <returns>The built <see cref="ApiResource"/>.</returns>
        public ApiResource Build()
        {
            if (this.isBuilt)
            {
                throw new InvalidOperationException("ApiResource already built.");
            }

            this.isBuilt = true;
            return this.apiResource;
        }

        internal ApiResourceBuilder WithAllowedClients(string clientList)
        {
            apiResource.Properties[ApplicationProfilesPropertyNames.Clients] = clientList;
            return this;
        }

        internal ApiResourceBuilder FromConfiguration()
        {
            apiResource.Properties[ApplicationProfilesPropertyNames.Source] = ApplicationProfilesPropertyValues.Configuration;
            return this;
        }
    }
}
