// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IErrorNotification.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISeverityQualifiedException interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ExceptionHandling;

/// <summary>
/// Interface for notifications, providing a message and a severity.
/// </summary>
public interface IErrorNotification : IHasSeverityLevel
{
    /// <summary>
    /// Gets the notification message.
    /// </summary>
    string? Message { get; }

    /// <summary>
    /// Gets the severity level. The default value is <see cref="SeverityLevel.Error"/>.
    /// </summary>
    SeverityLevel IHasSeverityLevel.Severity => SeverityLevel.Error;

    /// <summary>
    /// Gets the error type.
    /// This can be used to identify the exception type in a more specific way.
    /// </summary>
    public string? ErrorType => GetType().Name;
}
