// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationStatusExtensions.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

/// <summary>
/// Extension methods for <see cref="OperationStatus"/>.
/// </summary>
public static class OperationStatusExtensions
{
    /// <summary>
    /// Determines whether the specified operation status is a final status.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns>
    ///   <c>true</c> if the specified status is final; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsFinal(this OperationStatus status) => status switch
    {
        OperationStatus.Completed => true,
        OperationStatus.Failed => true,
        OperationStatus.Canceled => true,
        OperationStatus.Aborted => true,
        OperationStatus.TimedOut => true,
        _ => false,
    };
}