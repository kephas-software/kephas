// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoRedirectEndSessionEndpoint.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Services
{
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using System.Threading.Tasks;

    using global::IdentityServer4.Configuration;
    using global::IdentityServer4.Endpoints.Results;
    using global::IdentityServer4.Extensions;
    using global::IdentityServer4.Hosting;
    using global::IdentityServer4.Services;
    using global::IdentityServer4.Validation;
    using Kephas.AspNetCore.IdentityServer4.Configuration;
    using Kephas.Logging;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;

    internal class AutoRedirectEndSessionEndpoint : Loggable, IEndpointHandler
    {
        private readonly IUserSession session;
        private readonly IOptions<IdentityServerOptions> identityServerOptions;
        private readonly IEndSessionRequestValidator requestValidator;

        public AutoRedirectEndSessionEndpoint(
            IEndSessionRequestValidator requestValidator,
            IOptions<IdentityServerOptions> identityServerOptions,
            IUserSession session,
            ILogManager? logManager = null)
            : base(logManager)
        {
            this.session = session;
            this.identityServerOptions = identityServerOptions;
            this.requestValidator = requestValidator;
        }

        public async Task<IEndpointResult> ProcessAsync(HttpContext ctx)
        {
            var validationResult = this.ValidateRequest(ctx.Request);
            if (validationResult != null)
            {
                return validationResult;
            }

            var parameters = await this.GetParametersAsync(ctx.Request);
            var user = await this.session.GetUserAsync();
            var result = await this.requestValidator.ValidateAsync(parameters, user);
            if (result.IsError)
            {
                this.Logger.Error($"Error ending session {result.Error}");
                return new RedirectResult(this.identityServerOptions.Value.UserInteraction.ErrorUrl);
            }

            var client = result.ValidatedRequest?.Client;
            if (client != null &&
                client.Properties.TryGetValue(ApplicationProfilesPropertyNames.Profile, out var type))
            {
                var signInScheme = this.identityServerOptions.Value.Authentication.CookieAuthenticationScheme;
                if (signInScheme != null)
                {
                    await ctx.SignOutAsync(signInScheme);
                }
                else
                {
                    await ctx.SignOutAsync();
                }

                var postLogOutUri = result.ValidatedRequest?.PostLogOutUri;
                if (result.ValidatedRequest?.State != null)
                {
                    postLogOutUri = QueryHelpers.AddQueryString(postLogOutUri, OpenIdConnectParameterNames.State, result.ValidatedRequest.State);
                }

                return new RedirectResult(postLogOutUri!);
            }
            else
            {
                return new RedirectResult(this.identityServerOptions.Value.UserInteraction.LogoutUrl);
            }
        }

        private async Task<NameValueCollection> GetParametersAsync(HttpRequest request)
        {
            if (HttpMethods.IsGet(request.Method))
            {
                return request.Query.AsNameValueCollection();
            }

            var form = await request.ReadFormAsync();
            return form.AsNameValueCollection();
        }

        private IEndpointResult? ValidateRequest(HttpRequest request)
        {
            if (!HttpMethods.IsPost(request.Method) && !HttpMethods.IsGet(request.Method))
            {
                return new StatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (HttpMethods.IsPost(request.Method) &&
                !string.Equals(request.ContentType, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                return new StatusCodeResult(HttpStatusCode.BadRequest);
            }

            return null;
        }

        internal class RedirectResult : IEndpointResult
        {
            public RedirectResult(string url)
            {
                this.Url = url;
            }

            public string Url { get; }

            public Task ExecuteAsync(HttpContext context)
            {
                context.Response.Redirect(this.Url);
                return Task.CompletedTask;
            }
        }
    }
}
