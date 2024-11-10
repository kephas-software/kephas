// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpExceptionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// An <see cref="IExceptionContext"/> for a HTTP request.
/// </summary>
public record HttpExceptionContext(Exception Exception, string? RequestPath, HttpContext? HttpContext)
    : ExceptionContext(Exception, RequestPath ?? HttpContext?.Request?.Path)
{
    /// <summary>
    /// Clones the context with another exception.
    /// </summary>
    /// <param name="exception">Th exception to replace the original one.</param>
    /// <returns>A clone of this context with the provided exception.</returns>
    protected override IExceptionContext CloneContext(Exception exception)
        => this with {Exception = exception};
}
