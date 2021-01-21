// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AspNetConventionsConfigureOptions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Configuration
{
    using System;

    using global::IdentityServer4.Configuration;
    using Kephas.Configuration;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    internal class AspNetConventionsConfigureOptions : IConfigureOptions<IdentityServerOptions>
    {
        private readonly Lazy<IConfiguration<IdentityServerSettings>> lazyConfiguration;

        public AspNetConventionsConfigureOptions(Lazy<IConfiguration<IdentityServerSettings>> lazyConfiguration)
        {
            this.lazyConfiguration = lazyConfiguration;
        }

        public void Configure(IdentityServerOptions options)
        {
            options.Events.RaiseErrorEvents = true;
            options.Events.RaiseInformationEvents = true;
            options.Events.RaiseFailureEvents = true;
            options.Events.RaiseSuccessEvents = true;
            options.Authentication.CookieAuthenticationScheme = IdentityConstants.ApplicationScheme;
            options.UserInteraction.ErrorUrl = this.lazyConfiguration.Value.Settings.UserInteraction?.ErrorUrl ?? "/";
        }
    }
}
