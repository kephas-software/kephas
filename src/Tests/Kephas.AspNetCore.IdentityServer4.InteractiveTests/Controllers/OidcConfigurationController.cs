// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OidcConfigurationController.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.AspNetCore.IdentityServer4.InteractiveTests.Controllers
{
    using Kephas.AspNetCore.IdentityServer4.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class OidcConfigurationController : Controller
    {
        private readonly ILogger<OidcConfigurationController> logger;

        public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider, ILogger<OidcConfigurationController> logger)
        {
            this.ClientRequestParametersProvider = clientRequestParametersProvider;
            this.logger = logger;
        }

        public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        {
            var parameters = this.ClientRequestParametersProvider.GetClientParameters(this.HttpContext, clientId);
            return this.Ok(parameters);
        }
    }
}
