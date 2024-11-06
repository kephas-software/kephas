// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericExceptionResultProvider.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.DependencyInjection.Metadata;

namespace Kephas.Web.ExceptionHandling.ExceptionResultProviders;

/// <summary>
/// The generic exception result provider.
/// </summary>
[Priority(Priority.Low)]
public class GenericExceptionResultProvider : IExceptionResultProvider<Exception>
{
    internal static readonly IExceptionResultProvider Instance = new GenericExceptionResultProvider();

    /// <summary>
    /// Gets the exception result with default values.
    /// </summary>
    /// <param name="exception">The typed exception retrieved from the exception context.</param>
    /// <param name="exceptionContext">The exception context containing the exception to be handled.</param>
    /// <returns>The exception result, or <c>null</c> if the exception could not be handled.</returns>
    public ExceptionResult? TryGetExceptionResult(Exception exception, IExceptionContext exceptionContext) =>
        new(exception);
}
