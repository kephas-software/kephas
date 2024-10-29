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
/// We use state when talking about an overall condition of an entity or a result of some operation
/// (such as Idle, Running, Disabled, Verified and so on), while status stands for the current condition of the same
/// with respect to particular circumstances (such as Success, Failed, Warning and so on).
/// </remarks>
public enum OperationStatus
{
    /// <summary>
    /// The operation is not yet started.
    /// </summary>
    None = 0x0000,

    /// <summary>
    /// The operation is starting.
    /// </summary>
    Starting = 0x0010,

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
    /// The operation is completed successfully. This is a final condition.
    /// </summary>
    Completed = 0x0100,

    /// <summary>
    /// The operation failed. This is a final condition.
    /// </summary>
    Failed = -0x0100,

    /// <summary>
    /// The operation was canceled by the user. This is a final condition.
    /// </summary>
    Canceled = -0x0080,

    /// <summary>
    /// The operation was aborted. This is a final condition.
    /// </summary>
    Aborted = -0x0040,

    /// <summary>
    /// The operation timed out. This is a final condition.
    /// </summary>
    TimedOut = -0x0020,

    /// <summary>
    /// The operation did not complete successfully in the last try. Typically, a failed try is followed by a retry.
    /// </summary>
    FailedTry = -0x0010,
}