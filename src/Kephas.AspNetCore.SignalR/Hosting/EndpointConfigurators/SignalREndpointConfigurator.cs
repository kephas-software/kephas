// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SignalREndpointConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.SignalR.Hosting.EndpointConfigurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Kephas.Application.AspNetCore;
    using Kephas.Application.AspNetCore.Hosting;
    using Kephas.Reflection;
    using Kephas.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.SignalR;

    using HubMetadata = Kephas.AspNetCore.SignalR.HubMetadata;

    /// <summary>
    /// SignalR endpoint configurator registering all declared hubs.
    /// </summary>
    [ProcessingPriority(ProcessingPriority)]
    public class SignalREndpointConfigurator : IEndpointConfigurator
    {
        /// <summary>
        /// The processing priority of <see cref="SignalREndpointConfigurator"/>.
        /// </summary>
        public const Priority ProcessingPriority = Priority.BelowNormal;

        private static readonly MethodInfo MapHubMethod = ReflectionHelper.GetGenericMethodOf(_ => ((SignalREndpointConfigurator)null!).MapHub<Hub>(null!, null!, null!));
        private readonly ICollection<Lazy<IHubService, HubMetadata>> lazyHubs;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalREndpointConfigurator"/> class.
        /// </summary>
        /// <param name="lazyHubs">The lazy hubs.</param>
        public SignalREndpointConfigurator(ICollection<Lazy<IHubService, HubMetadata>> lazyHubs)
        {
            this.lazyHubs = lazyHubs;
        }

        /// <summary>
        /// Configures the endpoints using the given application context.
        /// </summary>
        /// <param name="endpoints">The endpoints builder.</param>
        /// <param name="appContext">Context for the application.</param>
        public virtual void Configure(IEndpointRouteBuilder endpoints, IAspNetAppContext appContext)
        {
            var hubsMetadata = this.lazyHubs.Order().Select(l => l.Metadata).ToList();
            foreach (var hubMetadata in hubsMetadata)
            {
                var mapHub = MapHubMethod.MakeGenericMethod(hubMetadata.ServiceType!);
                var builder = mapHub.Call(this, endpoints, appContext, hubMetadata);
            }
        }

        /// <summary>
        /// Adds hub map to the endpoints.
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        /// <param name="appContext">The application context.</param>
        /// <param name="metadata">The hub metadata.</param>
        /// <typeparam name="T">The hub type.</typeparam>
        /// <returns>The <see cref="HubEndpointConventionBuilder"/>.</returns>
        protected virtual HubEndpointConventionBuilder MapHub<T>(IEndpointRouteBuilder endpoints, IAspNetAppContext appContext, HubMetadata metadata)
            where T : Hub
        {
            return endpoints.MapHub<T>(metadata.Pattern);
        }
    }
}
