// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowExceptionController.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Test.Endpoints.Diagnostics.Messages;
using Kephas.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Kephas.Test.Endpoints.Diagnostics.Controllers;

[Route("api/v{version:apiVersion}/test")]
public class ThrowExceptionController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> UnhandledException()
    {
        await Mediator.Send(new ThrowExceptionRequest());
        return Ok();
    }
}