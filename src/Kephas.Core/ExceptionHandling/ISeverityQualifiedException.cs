// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISeverityQualifiedException.cs" company="Quartz Software SRL">
//   Copyright (c) Quartz Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>
//   Declares the ISeverityQualifiedException interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ExceptionHandling
{
    /// <summary>
    /// Interface for severity qualified exception.
    /// </summary>
    public interface ISeverityQualifiedException
    {
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; }

        /// <summary>
        /// Gets the severity.
        /// </summary>
        /// <value>
        /// The severity.
        /// </value>
        SeverityLevel Severity { get; }
    }
}