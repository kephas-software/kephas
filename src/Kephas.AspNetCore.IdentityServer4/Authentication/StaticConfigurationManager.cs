// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticConfigurationManager.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.Authentication
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.IdentityModel.Protocols;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;

    internal class StaticConfigurationManager : IConfigurationManager<OpenIdConnectConfiguration>
    {
        private readonly Task<OpenIdConnectConfiguration> configuration;

        public StaticConfigurationManager(OpenIdConnectConfiguration configuration) => this.configuration = Task.FromResult(configuration);

        public Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel) => this.configuration;

        public void RequestRefresh()
        {
        }
    }
}
