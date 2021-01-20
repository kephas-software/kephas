// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentityServerBuilderExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::IdentityServer4.Configuration;
    using global::IdentityServer4.Hosting;
    using global::IdentityServer4.Models;
    using global::IdentityServer4.Stores;
    using global::IdentityServer4.Validation;
    using Kephas.AspNetCore.IdentityServer4.Configuration;
    using Kephas.AspNetCore.IdentityServer4.Options;
    using Kephas.Configuration;
    using Kephas.Cryptography.X509Certificates;
    using Kephas.Serialization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Extension methods for <see cref="IdentityServerBuilder"/>.
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Configures defaults on Identity Server for ASP.NET Core scenarios.
        /// </summary>
        /// <typeparam name="TUser">The <typeparamref name="TUser"/> type.</typeparam>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <param name="configure">Optional. The <see cref="Action{ApplicationsOptions}"/>
        /// to configure the <see cref="ApiAuthorizationOptions"/>.</param>
        /// <returns>The provided <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddApiAuthorization<TUser>(
            this IIdentityServerBuilder builder,
            Action<ApiAuthorizationOptions>? configure = null)
                where TUser : class
        {
            builder.AddAspNetIdentity<TUser>()
                .AddOperationalStores()
                .ConfigureReplacedServices()
                .AddIdentityResources()
                .AddApiResources()
                .AddClients()
                .AddSigningCredentials();

            if (configure != null)
            {
                builder.Services.Configure(configure);
            }

            return builder;
        }

        /// <summary>
        /// Adds operational stores.
        /// </summary>
        /// <param name="builder">The identity server builder.</param>
        /// <returns>The provided identity server builder.</returns>
        public static IIdentityServerBuilder AddOperationalStores(this IIdentityServerBuilder builder)
        {
            var services = builder.Services;

            // Identity Services
            services.AddTransient<IUserStore<AppUserIdentity>, AppUserIdentityStore>();
            services.AddTransient<IRoleStore<AppRole>, AppRoleStore>();

            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();
            services.AddTransient<IDeviceFlowStore, DeviceFlowStore>();
            // services.AddSingleton<IHostedService, TokenCleanupHost>();

            return builder;
        }

        /// <summary>
        /// Adds API resources from the default configuration to the server using the key
        /// IdentityServer:Resources
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <returns>The provided <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddApiResources(
            this IIdentityServerBuilder builder) => builder.AddApiResources(configuration: null);

        /// <summary>
        /// Adds API resources from the given <paramref name="configuration"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <param name="configuration">The <see cref="IdentityServerSettings"/> instance containing the API definitions.</param>
        /// <returns>The provided <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddApiResources(
            this IIdentityServerBuilder builder,
            IdentityServerSettings? configuration)
        {
            builder.ConfigureReplacedServices();
            builder.AddApiScopes();
            builder.AddInMemoryApiResources(Enumerable.Empty<ApiResource>());
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IConfigureOptions<ApiAuthorizationOptions>, ConfigureApiResources>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<ConfigureApiResources>>();
                    var effectiveConfig = configuration ?? sp.GetRequiredService<IConfiguration<IdentityServerSettings>>().Settings;
                    var localApiDescriptor = sp.GetService<IIdentityServerJwtDescriptor>();
                    return new ConfigureApiResources(effectiveConfig, localApiDescriptor, logger);
                }));

            // We take over the setup for the API resources as Identity Server registers the enumerable as a singleton
            // and that prevents normal composition.
            builder.Services.AddSingleton<IEnumerable<ApiResource>>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ApiAuthorizationOptions>>();
                return options.Value.ApiResources;
            });

            return builder;
        }

        /// Adds API scopes from the defined resources to the list of API scopes
        internal static IIdentityServerBuilder AddApiScopes(this IIdentityServerBuilder builder)
        {
            // We take over the setup for the API resources as Identity Server registers the enumerable as a singleton
            // and that prevents normal composition.
            builder.Services.AddSingleton<IEnumerable<ApiScope>>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ApiAuthorizationOptions>>();
                return options.Value.ApiScopes;
            });

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPostConfigureOptions<ApiAuthorizationOptions>, ConfigureApiScopes>());

            return builder;
        }

        /// <summary>
        /// Adds identity resources from the default configuration to the server using the key
        /// IdentityServer:Resources
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <returns>The provided <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddIdentityResources(
            this IIdentityServerBuilder builder) => builder.AddIdentityResources(configuration: null);

        /// <summary>
        /// Adds identity resources from the given <paramref name="configuration"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> instance containing the API definitions.</param>
        /// <returns>The provided <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddIdentityResources(
            this IIdentityServerBuilder builder,
            IConfiguration configuration)
        {
            builder.ConfigureReplacedServices();
            builder.AddInMemoryIdentityResources(Enumerable.Empty<IdentityResource>());
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IConfigureOptions<ApiAuthorizationOptions>, ConfigureIdentityResources>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<ConfigureIdentityResources>>();
                    var effectiveConfig = configuration ?? sp.GetRequiredService<IConfiguration>().GetSection("IdentityServer:Identity");
                    return new ConfigureIdentityResources(effectiveConfig, logger);
                }));

            // We take over the setup for the identity resources as Identity Server registers the enumerable as a singleton
            // and that prevents normal composition.
            builder.Services.AddSingleton<IEnumerable<IdentityResource>>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ApiAuthorizationOptions>>();
                return options.Value.IdentityResources;
            });

            return builder;
        }

        /// <summary>
        /// Adds clients from the default configuration to the server using the key
        /// IdentityServer:Clients
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <returns>The provided <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddClients(
            this IIdentityServerBuilder builder) => builder.AddClients(configuration: null);

        /// <summary>
        /// Adds clients from the given <paramref name="configuration"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <param name="configuration">The <see cref="IdentityServerSettings"/> instance containing the client definitions.</param>
        /// <returns>The provided <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddClients(
            this IIdentityServerBuilder builder,
            IdentityServerSettings? configuration)
        {
            builder.ConfigureReplacedServices();
            builder.AddInMemoryClients(Enumerable.Empty<Client>());

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IPostConfigureOptions<ApiAuthorizationOptions>, ConfigureClientScopes>());

            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IConfigureOptions<ApiAuthorizationOptions>, ConfigureClients>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<ConfigureClients>>();
                    var effectiveConfig = configuration ?? sp.GetRequiredService<IConfiguration<IdentityServerSettings>>().Settings;
                    return new ConfigureClients(effectiveConfig, logger);
                }));

            // We take over the setup for the clients as Identity Server registers the enumerable as a singleton and that prevents normal composition.
            builder.Services.AddSingleton<IEnumerable<Client>>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ApiAuthorizationOptions>>();
                return options.Value.Clients;
            });

            return builder;
        }

        /// <summary>
        /// Adds a signing key from the default configuration to the server using the configuration key
        /// IdentityServer:Key
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <returns>The provided <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddSigningCredentials(
            this IIdentityServerBuilder builder) => builder.AddSigningCredentials(configuration: null);

        /// <summary>
        /// Adds a signing key from the given <paramref name="configuration"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="IIdentityServerBuilder"/>.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/>.</param>
        /// <returns>The provided <see cref="IIdentityServerBuilder"/>.</returns>
        public static IIdentityServerBuilder AddSigningCredentials(
            this IIdentityServerBuilder builder,
            IdentityServerSettings? configuration)
        {
            builder.ConfigureReplacedServices();
            builder.Services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IConfigureOptions<ApiAuthorizationOptions>, ConfigureSigningCredentials>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<ConfigureSigningCredentials>>();
                    var certProvider = sp.GetRequiredService<ICertificateProvider>();
                    var serializationService = sp.GetRequiredService<ISerializationService>();
                    var effectiveConfig = configuration ?? sp.GetRequiredService<IConfiguration<IdentityServerSettings>>().Settings;
                    return new ConfigureSigningCredentials(effectiveConfig, certProvider, serializationService, logger);
                }));

            // We take over the setup for the credentials store as Identity Server registers a singleton
            builder.Services.AddSingleton<ISigningCredentialStore>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ApiAuthorizationOptions>>();
                return new InMemorySigningCredentialsStore(options.Value.SigningCredential);
            });

            // We take over the setup for the validation keys store as Identity Server registers a singleton
            builder.Services.AddSingleton<IValidationKeysStore>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<ApiAuthorizationOptions>>();
                return new InMemoryValidationKeysStore(new[]
                {
                    new SecurityKeyInfo
                    {
                        Key = options.Value.SigningCredential.Key,
                        SigningAlgorithm = options.Value.SigningCredential.Algorithm,
                    },
                });
            });

            return builder;
        }

        internal static IIdentityServerBuilder ConfigureReplacedServices(this IIdentityServerBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<IdentityServerOptions>, AspNetConventionsConfigureOptions>());
            builder.Services.AddSingleton<IRedirectUriValidator, RelativeRedirectUriValidator>();
            ReplaceEndSessionEndpoint(builder);

            return builder;
        }

        private static void ReplaceEndSessionEndpoint(IIdentityServerBuilder builder)
        {
            // We don't have a better way to replace the end session endpoint as far as we know other than looking the descriptor up
            // on the container and replacing the instance. This is due to the fact that we chain on AddIdentityServer which configures the
            // list of endpoints by default.
            var endSessionEndpointDescriptor = builder.Services
                            .Single(s => s.ImplementationInstance is Endpoint e &&
                                    string.Equals(e.Name, "Endsession", StringComparison.OrdinalIgnoreCase) &&
                                    string.Equals("/connect/endsession", e.Path, StringComparison.OrdinalIgnoreCase));

            builder.Services.Remove(endSessionEndpointDescriptor);
            builder.AddEndpoint<AutoRedirectEndSessionEndpoint>("EndSession", "/connect/endsession");
        }
    }
}
