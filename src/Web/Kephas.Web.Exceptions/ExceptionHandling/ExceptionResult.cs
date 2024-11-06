// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionResult.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.ExceptionHandling;

namespace Kephas.Web.ExceptionHandling;

/// <summary>
/// The exception result.
/// </summary>
public record ExceptionResult : ExceptionData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionResult"/>.
    /// </summary>
    public ExceptionResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionResult"/>.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public ExceptionResult(Exception exception)
        : base(exception)
    {
        InnerException = exception.InnerException is null ? null : new ExceptionResult(exception.InnerException);
    }

    /// <summary>
    /// Gets or sets the status code.
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the url.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Gets or sets the correlation Guid.
    /// </summary>
    public Guid CorrelationId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the inner exception.
    /// </summary>
    public ExceptionResult? InnerException { get; set; }

    /// <summary>
    /// Gets a <see cref="ExceptionResult"/> from the <see cref="IErrorNotification"/> instance.
    /// </summary>
    /// <param name="errorNotification">The error notification.</param>
    /// <returns>A new instance of <see cref="ExceptionResult"/>.</returns>
    public static ExceptionResult FromErrorNotification(IErrorNotification errorNotification)
    {
        ArgumentNullException.ThrowIfNull(errorNotification);

        var result = new ExceptionResult
        {
            ErrorType = errorNotification.ErrorType,
            Message = errorNotification.Message,
        };

        if (errorNotification is Exception ex)
        {
            result.InnerException = ex.InnerException is null ? null : new ExceptionResult(ex.InnerException);
        }

        return result;
    }
}
