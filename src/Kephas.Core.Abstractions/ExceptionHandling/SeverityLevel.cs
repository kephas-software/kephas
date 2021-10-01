// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeverityLevel.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the severity level class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ExceptionHandling
{
    using Kephas.Logging;

    /// <summary>
    /// Values that represent exception severities.
    /// </summary>
    public enum SeverityLevel
    {
        /// <summary>
        /// An enum constant representing the fatal error option.
        /// </summary>
        Fatal = LogLevel.Fatal,

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
        Info = LogLevel.Info,
    }

    /// <summary>
    /// Extension methods for <see cref="SeverityLevel"/>.
    /// </summary>
#pragma warning disable SA1649
    public static class SeverityLevelExtensions
#pragma warning restore SA1649
    {
        /// <summary>
        /// Gets a value indicating whether the severity level indicates an error.
        /// </summary>
        /// <param name="severity">The severity level.</param>
        /// <returns>
        /// True if error, false if not.
        /// </returns>
        public static bool IsError(this SeverityLevel severity)
        {
            return severity <= SeverityLevel.Error;
        }
    }
}