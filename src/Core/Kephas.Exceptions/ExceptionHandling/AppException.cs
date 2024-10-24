// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppException.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ExceptionHandling;

/// <summary>
/// Generic application exception implementing <see cref="IErrorNotification"/>.
/// </summary>
public class AppException : Exception, IErrorNotification
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
    /// Gets the severity. The default value is <see cref="SeverityLevel.Error"/>.
    /// </summary>
    public SeverityLevel Severity { get; init; } = SeverityLevel.Error;
}
