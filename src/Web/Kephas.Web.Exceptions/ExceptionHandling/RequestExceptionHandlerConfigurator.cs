// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestExceptionHandlerConfigurator.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// Configurator for the <see cref="ExceptionHandlerOptions"/>.
/// </summary>
public class RequestExceptionHandlerConfigurator(IRequestExceptionHandler exceptionHandler)
    : IConfigureOptions<ExceptionHandlerOptions>
{
    /// <summary>
    /// Invoked to configure a <see cref="ExceptionHandlerOptions"/> instance.
    /// </summary>
    /// <param name="options">The options instance to configure.</param>
    public void Configure(ExceptionHandlerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.ExceptionHandler = httpContext =>
        {
            var exceptionFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
            if (exceptionFeature is null)
            {
                // TODO something happened, should log this
                return Task.CompletedTask;
            }
            
            var pathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
            var originalPath = pathFeature?.Path;

            Task WriteResponse(int status, string jsonResponse)
            {
                httpContext.Response.ContentType = MediaTypeNames.Application.Json;
                httpContext.Response.StatusCode = status;
                return httpContext.Response.WriteAsync(jsonResponse);
            }

            return exceptionHandler.HandleException(new HttpExceptionContext(exceptionFeature.Error, originalPath ?? httpContext.Request.Path, httpContext), WriteResponse);
        };
    }
}