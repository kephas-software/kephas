// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityResourceBuilder.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Options
{
    using System;

    using global::IdentityServer4;
    using global::IdentityServer4.Models;
    using Kephas.AspNetCore.IdentityServer4.Configuration;

    /// <summary>
    /// A builder for identity resources
    /// </summary>
    public class IdentityResourceBuilder
    {
        private IdentityResource identityResource;
        private bool isBuilt;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceBuilder"/> class.
        /// </summary>
        public IdentityResourceBuilder()
            : this(new IdentityResource())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceBuilder"/> class.
        /// </summary>
        /// <param name="resource">A preconfigured resource.</param>
        public IdentityResourceBuilder(IdentityResource resource)
        {
            this.identityResource = resource;
        }

        /// <summary>
        /// Creates an openid resource.
        /// </summary>
        /// <returns>The identity resource builder.</returns>
        public static IdentityResourceBuilder OpenId() =>
            IdentityResource(IdentityServerConstants.StandardScopes.OpenId);

        /// <summary>
        /// Creates a profile resource.
        /// </summary>
        /// <returns>The identity resource builder.</returns>
        public static IdentityResourceBuilder Profile() =>
            IdentityResource(IdentityServerConstants.StandardScopes.Profile);

        /// <summary>
        /// Creates an address resource.
        /// </summary>
        /// <returns>The identity resource builder.</returns>
        public static IdentityResourceBuilder Address() =>
            IdentityResource(IdentityServerConstants.StandardScopes.Address);

        /// <summary>
        /// Creates an email resource.
        /// </summary>
        /// <returns>The identity resource builder.</returns>
        public static IdentityResourceBuilder Email() =>
            IdentityResource(IdentityServerConstants.StandardScopes.Email);

        /// <summary>
        /// Creates a phone resource.
        /// </summary>
        /// <returns>The identity resource builder.</returns>
        public static IdentityResourceBuilder Phone() =>
            IdentityResource(IdentityServerConstants.StandardScopes.Phone);

        /// <summary>
        /// Configures the API resource to allow all clients to access it.
        /// </summary>
        /// <returns>The <see cref="IdentityResourceBuilder"/>.</returns>
        public IdentityResourceBuilder AllowAllClients()
        {
            this.identityResource.Properties[ApplicationProfilesPropertyNames.Clients] = ApplicationProfilesPropertyValues.AllowAllApplications;
            return this;
        }

        /// <summary>
        /// Builds the API resource.
        /// </summary>
        /// <returns>The built <see cref="IdentityResource"/>.</returns>
        public IdentityResource Build()
        {
            if (this.isBuilt)
            {
                throw new InvalidOperationException("IdentityResource already built.");
            }

            this.isBuilt = true;
            return this.identityResource;
        }

        internal IdentityResourceBuilder WithAllowedClients(string clientList)
        {
            this.identityResource.Properties[ApplicationProfilesPropertyNames.Clients] = clientList;
            return this;
        }

        internal IdentityResourceBuilder FromConfiguration()
        {
            this.identityResource.Properties[ApplicationProfilesPropertyNames.Source] = ApplicationProfilesPropertyValues.Configuration;
            return this;
        }

        internal IdentityResourceBuilder FromDefault()
        {
            this.identityResource.Properties[ApplicationProfilesPropertyNames.Source] = ApplicationProfilesPropertyValues.Default;
            return this;
        }

        internal static IdentityResourceBuilder IdentityResource(string name)
        {
            var identityResource = GetResource(name);
            return new IdentityResourceBuilder(identityResource);
        }

        private static IdentityResource GetResource(string name)
        {
            switch (name)
            {
                case IdentityServerConstants.StandardScopes.OpenId:
                    return new IdentityResources.OpenId();
                case IdentityServerConstants.StandardScopes.Profile:
                    return new IdentityResources.Profile();
                case IdentityServerConstants.StandardScopes.Address:
                    return new IdentityResources.Address();
                case IdentityServerConstants.StandardScopes.Email:
                    return new IdentityResources.Email();
                case IdentityServerConstants.StandardScopes.Phone:
                    return new IdentityResources.Phone();
                default:
                    throw new InvalidOperationException("Invalid identity resource type.");
            }
        }
    }
}