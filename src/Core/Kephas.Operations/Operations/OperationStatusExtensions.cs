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
    /// Determines whether the specified operation status indicates completion.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns>
    ///   <c>true</c> if the specified status indicates completion; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsCompletedPhase(this OperationStatus status) =>
        (status & (OperationStatus)OperationPhase.Completed) > 0;

    /// <summary>
    /// Determines whether the specified operation status indicates failure.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns>
    ///   <c>true</c> if the specified status indicates failure; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsFailedPhase(this OperationStatus status) =>
        (status & (OperationStatus)OperationPhase.Failed) > 0;

    /// <summary>
    /// Determines whether the operation is waiting for an event.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns>
    ///   <c>true</c> if the operation is waiting for an event; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWaitingPhase(this OperationStatus status) =>
        (status & (OperationStatus)OperationPhase.Waiting) > 0;

    /// <summary>
    /// Determines whether the operation is currently executing.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns>
    ///   <c>true</c> if the operation is currently executing; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsExecutingPhase(this OperationStatus status) =>
        (status & (OperationStatus)OperationPhase.Executing) > 0;

    /// <summary>
    /// Determines whether the operation is currently executing or is in a wait state.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns>
    ///   <c>true</c> if the operation is currently executing or is in a wait state; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNonePhase(this OperationStatus status) =>
        status == OperationStatus.None;

    /// <summary>
    /// Determines whether the operation has been started.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns>
    ///   <c>true</c> if the operation has been started; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsStarted(this OperationStatus status) =>
        status != OperationStatus.None;

    /// <summary>
    /// Determines whether the operation is currently executing or is in a wait state.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns>
    ///   <c>true</c> if the operation is currently executing or is in a wait state; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsInProgress(this OperationStatus status) =>
        (status & (OperationStatus)(OperationPhase.Executing | OperationPhase.Waiting)) > 0;

    /// <summary>
    /// Determines whether the specified operation status either completed or failed.
    /// </summary>
    /// <param name="status">The operation status.</param>
    /// <returns>
    ///   <c>true</c> if the specified status indicates either completion or failure; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsFinalized(this OperationStatus status) =>
        (status & (OperationStatus)(OperationPhase.Failed | OperationPhase.Completed)) > 0;
}