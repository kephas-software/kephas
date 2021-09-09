// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureApiResources.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System;
    using System.Collections.Generic;

    using global::IdentityServer4.Models;
    using Kephas.AspNetCore.IdentityServer4.Options;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal class ConfigureApiResources : IConfigureOptions<ApiAuthorizationOptions>
    {
        private readonly IdentityServerSettings configuration;
        private readonly ILogger<ConfigureApiResources> logger;
        private readonly IIdentityServerJwtDescriptor localApiDescriptor;

        public ConfigureApiResources(
            IdentityServerSettings configuration,
            IIdentityServerJwtDescriptor localApiDescriptor,
            ILogger<ConfigureApiResources> logger)
        {
            this.configuration = configuration;
            this.localApiDescriptor = localApiDescriptor;
            this.logger = logger;
        }

        public void Configure(ApiAuthorizationOptions options)
        {
            var resources = this.GetApiResources();
            foreach (var resource in resources)
            {
                options.ApiResources.Add(resource);
            }
        }

        internal IEnumerable<ApiResource> GetApiResources()
        {
            var data = this.configuration.Resources;

            if (data != null)
            {
                foreach (var kvp in data)
                {
                    this.logger.LogInformation($"Configuring API resource '{kvp.Key}'.");
                    yield return this.GetResource(kvp.Key, kvp.Value);
                }
            }

            var localResources = this.localApiDescriptor.GetResourceSettings();
            if (localResources != null)
            {
                foreach (var kvp in localResources)
                {
                    this.logger.LogInformation($"Configuring local API resource '{kvp.Key}'.");
                    yield return GetResource(kvp.Key, kvp.Value);
                }
            }
        }

        public ApiResource GetResource(string name, ResourceSettings definition) =>
            definition.Profile switch
            {
                ApplicationProfiles.API => this.GetAPI(name, definition),
                ApplicationProfiles.IdentityServerJwt => this.GetLocalAPI(name, definition),
                _ => throw new InvalidOperationException($"Type '{definition.Profile}' is not supported.")
            };

        private ApiResource GetAPI(string name, ResourceSettings definition) =>
            ApiResourceBuilder.ApiResource(name)
                .FromConfiguration()
                .WithAllowedClients(ApplicationProfilesPropertyValues.AllowAllApplications)
                .ReplaceScopes(definition.Scopes ?? new[] { name })
                .Build();

        private ApiResource GetLocalAPI(string name, ResourceSettings definition) =>
            ApiResourceBuilder.IdentityServerJwt(name)
                .FromConfiguration()
                .WithAllowedClients(ApplicationProfilesPropertyValues.AllowAllApplications)
                .ReplaceScopes(definition.Scopes ?? new[] { name })
                .Build();
    }
}