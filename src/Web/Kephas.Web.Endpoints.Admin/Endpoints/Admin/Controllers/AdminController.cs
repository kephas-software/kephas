// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdminController.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET8_0_OR_GREATER
using Asp.Versioning;
#endif

using System.Net;
using Kephas.Web.Controllers;
using Kephas.Web.Endpoints.Admin.Messages;
using Microsoft.AspNetCore.Mvc;

namespace Kephas.Web.Endpoints.Admin.Controllers;

/// <summary>
/// Represents a controller for handling administration related requests.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin")]
public class AdminController : ApiControllerBase
{
    /// <summary>
    /// Gets the application's log level.
    /// </summary>
    /// <returns>A task yielding the <see cref="IActionResult"/> representing the result of the operation.</returns>
    [HttpGet("[action]")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetLogLevel() =>
        Ok(await Mediator.Send(new GetLogLevelRequest()));

    /// <summary>
    /// Gets the application's log level.
    /// </summary>
    /// <returns>A task yielding the <see cref="IActionResult"/> representing the result of the operation.</returns>
    [HttpPut("[action]")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> SetLogLevel([FromQuery] SetLogLevelRequest request) =>
        Ok(await Mediator.Send(request));
}
