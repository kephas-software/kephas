// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorNotificationResultProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;

namespace Kephas.Web.ExceptionHandling.ExceptionResultProviders;

/// <summary>
/// Exception result provider for <see cref="IErrorNotification"/>.
/// </summary>
public class ErrorNotificationResultProvider : IExceptionResultProvider<IErrorNotification>
{
    /// <summary>
    /// Gets the exception result.
    /// </summary>
    /// <param name="exception">The typed exception retrieved from the exception context.</param>
    /// <param name="exceptionContext">The exception context containing the exception to be handled.</param>
    /// <returns>The exception result, or <c>null</c> if the exception could not be handled.</returns>
    public ExceptionResult? TryGetExceptionResult(IErrorNotification exception, IExceptionContext exceptionContext) =>
        ExceptionResult.FromErrorNotification(exception);
}
