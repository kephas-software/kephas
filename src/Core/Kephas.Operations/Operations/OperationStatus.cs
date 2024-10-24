// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationState.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

/// <summary>
/// The operation status.
/// </summary>
/// <remarks>
/// We use state when talking about an overall condition of an entity or a result of some operation with the entity
/// (such as Idle, Running, Disabled, Verified and so on), while status stands for the current condition of an entity
/// with respect to particular circumstances (such as Success, Failed, Warning and so on).
/// </remarks>
public enum OperationStatus
{
    /// <summary>
    /// The operation is not yet started.
    /// </summary>
    NotStarted = 0x0000,

    /// <summary>
    /// The operation is initiating.
    /// </summary>
    Initiating = 0x0010,

    /// <summary>
    /// The operation started and is in progress.
    /// </summary>
    InProgress = 0x0020,

    /// <summary>
    /// The operation is paused.
    /// </summary>
    Paused = 0x0040,

    /// <summary>
    /// The operation is completing.
    /// </summary>
    Completing = 0x0080,

    /// <summary>
    /// The operation is completed.
    /// </summary>
    Completed = 0x0100,

    /// <summary>
    /// The operation failed.
    /// </summary>
    Failed = -0x0100,

    /// <summary>
    /// The operation was canceled by the user.
    /// </summary>
    Canceled = -0x0080,

    /// <summary>
    /// The operation was aborted.
    /// </summary>
    Aborted = -0x0040,

    /// <summary>
    /// The operation timed out.
    /// </summary>
    TimedOut = -0x0020,

    /// <summary>
    /// The operation did not complete successfully and was scheduled for retry.
    /// </summary>
    ScheduledForRetry = -0x0010,
}
