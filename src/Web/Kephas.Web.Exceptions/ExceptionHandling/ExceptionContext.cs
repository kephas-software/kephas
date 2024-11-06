// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionContext.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// Default implementation of the <see cref="IExceptionContext"/>.
/// </summary>
/// <param name="Exception">The exception to be handled.</param>
/// <param name="RequestPath">Optional. The application request path.</param>
public record ExceptionContext(Exception Exception, string? RequestPath) : IExceptionContext
{
    /// <summary>
    /// Clones the context with another exception.
    /// </summary>
    /// <param name="exception">The exception to replace the original one.</param>
    /// <returns>A clone of this context with the provided exception.</returns>
    IExceptionContext IExceptionContext.Clone(Exception exception)
        => CloneContext(exception);
    
    /// <summary>
    /// Clones the context with another exception.
    /// </summary>
    /// <param name="exception">The exception to replace the original one.</param>
    /// <returns>A clone of this context with the provided exception.</returns>
    protected virtual IExceptionContext CloneContext(Exception exception)
        => this with {Exception = exception};
}
