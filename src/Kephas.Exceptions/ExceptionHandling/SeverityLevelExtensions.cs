// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SeverityLevelExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ExceptionHandling;

/// <summary>
/// Extension methods for <see cref="SeverityLevel"/>.
/// </summary>
public static class SeverityLevelExtensions
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