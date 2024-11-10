// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlingExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.ExceptionHandling;

/// <summary>
/// Extension methods for the exception handling area.
/// </summary>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    /// Gets a value indicating whether this object represents an error.
    /// </summary>
    /// <returns><c>true</c> if this object represents an error, <c>false</c> otherwise.</returns>
    public static bool IsError(this IHasSeverityLevel obj) =>
        obj?.Severity is SeverityLevel.Critical or SeverityLevel.Error;

    /// <summary>
    /// Gets a value indicating whether this object represents a warning.
    /// </summary>
    /// <returns><c>true</c> if this object represents a warning, <c>false</c> otherwise.</returns>
    public static bool IsWarning(this IHasSeverityLevel obj) =>
        obj?.Severity is SeverityLevel.Warning;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="exceptionData"></param>
    /// <returns></returns>
    public static Exception ToException(this ExceptionData exceptionData)
    {
        ArgumentNullException.ThrowIfNull(exceptionData);
        
        return new AppException(exceptionData.Message)
        {
            Severity = exceptionData.Severity,
            ErrorType = exceptionData.ErrorType,
        };
    }
}