// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeverityLevel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the severity level class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.Extensions.Logging;

namespace Kephas.ExceptionHandling
{
    /// <summary>
    /// Values that represent exception severities.
    /// </summary>
    public enum SeverityLevel
    {
        /// <summary>
        /// An enum constant representing the fatal error option.
        /// </summary>
        Fatal = LogLevel.Critical,

        /// <summary>
        /// An enum constant representing the error option.
        /// </summary>
        Error = LogLevel.Error,

        /// <summary>
        /// An enum constant representing the warning option.
        /// </summary>
        Warning = LogLevel.Warning,

        /// <summary>
        /// An enum constant representing the information option.
        /// </summary>
        Info = LogLevel.Information,
    }
}
