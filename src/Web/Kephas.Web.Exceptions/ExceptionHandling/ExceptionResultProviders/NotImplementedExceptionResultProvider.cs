// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotImplementedExceptionResultProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Net;

namespace Kephas.Web.ExceptionHandling.ExceptionResultProviders;

/// <summary>
/// Exception result provider for <see cref="NotImplementedException"/>.
/// </summary>
public class NotImplementedExceptionResultProvider : IExceptionResultProvider<NotImplementedException>
{
    /// <summary>
    /// Gets the exception result for <see cref="NotImplementedException"/>.
    /// </summary>
    /// <param name="exception">The typed exception retrieved from the exception context.</param>
    /// <param name="exceptionContext">The exception context containing the exception to be handled.</param>
    /// <returns>The exception result, or <c>null</c> if the exception could not be handled.</returns>
    public ExceptionResult? TryGetExceptionResult(NotImplementedException exception, IExceptionContext exceptionContext) =>
        new()
        {
            StatusCode = (int)HttpStatusCode.NotImplemented,
            Message = $"An internal server error occurred while accessing '{exceptionContext?.RequestPath}'. Please contact the development department to fix the missing functionality.",
        };
}