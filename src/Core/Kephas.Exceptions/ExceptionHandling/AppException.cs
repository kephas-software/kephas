// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.ExceptionHandling;

/// <summary>
/// Generic application exception implementing <see cref="IErrorNotification"/>.
/// </summary>
public class AppException : ApplicationException, IErrorNotification
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    /// <param name="message">Optional. The message.</param>
    /// <param name="innerException">Optional. The inner exception.</param>
    public AppException(string? message = null, Exception? innerException = null)
        : base(message ?? "An application exception occurred.", innerException)
    {
    }

    /// <summary>
    /// Gets the severity. The default value is <see cref="LogLevel.Error"/>.
    /// </summary>
    public LogLevel Severity { get; init; } = LogLevel.Error;
}
