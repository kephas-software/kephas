// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestController.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;
using Kephas.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Kephas.Test.Endpoints.Diagnostics.Controllers;

[Route("api/v{version:apiVersion}/test")]
public class TestController : ApiControllerBase
{
    [HttpGet]
    public Task<IActionResult> UnhandledException()
    {
        throw new AppException("Bad logic.");
    }
}