// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiControllerBase.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kephas.Web.Controllers;

/// <summary>
/// Base class for API controllers.
/// </summary>
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public class ApiControllerBase : Controller
{
    private IMediator? mediator;
    private ILogger? logger;

    /// <summary>
    /// Gets the logger of this controller.
    /// </summary>
    protected ILogger Logger =>
        logger ??= (ILogger)HttpContext.RequestServices.GetRequiredService(typeof(ILogger<>).MakeGenericType(GetType()));

    /// <summary>
    /// Gets the mediator.
    /// </summary>
    protected IMediator Mediator => mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}