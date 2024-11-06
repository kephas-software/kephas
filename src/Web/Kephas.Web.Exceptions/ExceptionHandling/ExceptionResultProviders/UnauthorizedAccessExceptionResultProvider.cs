// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnauthorizedAccessExceptionResultProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using Kephas.DependencyInjection.Metadata;
using Kephas.ExceptionHandling;

namespace Kephas.Web.ExceptionHandling.ExceptionResultProviders;

/// <summary>
/// Exception result provider for <see cref="UnauthorizedAccessException"/>.
/// </summary>
[Priority(Priority.BelowNormal)]
public class UnauthorizedAccessExceptionResultProvider : IExceptionResultProvider<UnauthorizedAccessException>
{
    /// <summary>
    /// Gets the exception result when the user is not authorized.
    /// </summary>
    /// <param name="exception">The typed exception retrieved from the exception context.</param>
    /// <param name="exceptionContext">The exception context containing the exception to be handled.</param>
    /// <returns>The exception result, or <c>null</c> if the exception could not be handled.</returns>
    public ExceptionResult? TryGetExceptionResult(UnauthorizedAccessException exception, IExceptionContext exceptionContext) =>
        new()
        {
            StatusCode = (int)HttpStatusCode.Unauthorized,
            Message = $"Unauthorized while accessing '{exceptionContext?.RequestPath}'.",
            Severity = SeverityLevel.Information,
        };
}
