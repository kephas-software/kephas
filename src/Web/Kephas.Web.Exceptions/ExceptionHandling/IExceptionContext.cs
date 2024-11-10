// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExceptionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// Provides a context for the exception handling.
/// </summary>
public interface IExceptionContext
{
    /// <summary>
    /// Gets the exception to be handled.
    /// </summary>
    Exception Exception { get; }

    /// <summary>
    /// Optional. Gets the request path.
    /// </summary>
    string? RequestPath { get; }

    /// <summary>
    /// Clones the context with another exception.
    /// </summary>
    /// <param name="exception">The exception to replace the original one.</param>
    /// <returns>A clone of this context with the provided exception.</returns>
    IExceptionContext Clone(Exception exception);
}