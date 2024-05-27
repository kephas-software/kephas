// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IErrorNotification.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISeverityQualifiedException interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Kephas.Logging;
using Microsoft.Extensions.Logging;

namespace Kephas.ExceptionHandling;

/// <summary>
/// Interface for notifications, providing a message and a severity.
/// </summary>
public interface IErrorNotification : IHasLogLevel
{
    /// <summary>
    /// Gets the notification message.
    /// </summary>
    string? Message { get; }

    /// <summary>
    /// Gets the severity. The default value is <see cref="LogLevel.Error"/>.
    /// </summary>
    LogLevel Severity => LogLevel.Error;

    /// <summary>
    /// Gets the log level.
    /// </summary>
    LogLevel IHasLogLevel.LogLevel => Severity;
}
