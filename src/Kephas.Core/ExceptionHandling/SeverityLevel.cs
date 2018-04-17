// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeverityLevel.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Implements the severity level class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
        FatalError,

        /// <summary>
        /// An enum constant representing the error option.
        /// </summary>
        Error,

        /// <summary>
        /// An enum constant representing the warning option.
        /// </summary>
        Warning,
    }
}