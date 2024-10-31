// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationState.cs" company="Kephas Software SRL">
//   Copyright (c) Kephas Software SRL. All rights reserved.
//   Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Kephas.Operations;

/// <summary>
/// Enumerates the values for the operation status.
/// </summary>
/// <remarks>
/// We use state when talking about an overall condition of an entity or a result of some operation
/// (such as Idle, Running, Disabled, Verified and so on), while status stands for the current condition of the same
/// with respect to particular circumstances (such as Success, Failed, Warning and so on).
/// </remarks>
public enum OperationStatus
{
    #region Range 0: The operation is not yet started. 

    /// <summary>
    /// The operation is not yet started.
    /// </summary>
    None = 0x0000,

    #endregion
        
    #region Range 0x0001 - 0x000F: The operation is executing. 
    
    /// <summary>
    /// The operation is initializing, typically by the infrastructure that supports it.
    /// </summary>
    /// <remarks>
    /// In this phase, aspects may run and prepare the operation for execution, for example validation, authentication, authorization.
    /// </remarks>
    Initializing = OperationPhase.Executing | 0x0002,

    /// <summary>
    /// The operation is in progress, the main functionality is being executed.
    /// </summary>
    Executing = OperationPhase.Executing | 0x0004,

    /// <summary>
    /// The main functionality completed the execution and the operation is finalizing, typically by the infrastructure that supports it.
    /// </summary>
    /// <remarks>
    /// In this phase, aspects may run and prepare the operation for finalization, for example auditing and profiling.
    /// </remarks>
    Finalizing = OperationPhase.Executing | 0x0008,

    #endregion

    #region Range 0x001x - 0x00Fx: The execution has been halted and the operation is waiting for an event to resume execution
    
    /// <summary>
    /// The operation has been actively halted by an external event, and is waiting for an external event to resume.
    /// </summary>
    Paused = OperationPhase.Waiting | 0x0020,

    /// <summary>
    /// The execution has been halted by an internal logic, and is waiting for an external or internal event to resume.
    /// </summary>
    Idle = OperationPhase.Waiting | 0x0040,

    /// <summary>
    /// The operation did not complete successfully in the last try and is scheduled for a retry.
    /// </summary>
    FailedTry = OperationPhase.Waiting | 0x0080,

    #endregion

    #region Range 0x01xx - 0x0Fxx: The execution failed

    /// <summary>
    /// The operation failed. This is a final status.
    /// </summary>
    Failed = OperationPhase.Failed,

    /// <summary>
    /// The operation was terminated by the means of the cancellation infrastructure. This is a final status.
    /// </summary>
    Canceled = OperationPhase.Failed | 0x0200,

    /// <summary>
    /// The operation was aborted by an internal logic or by an external event. This is a final status.
    /// </summary>
    Aborted = OperationPhase.Failed | 0x0400,

    /// <summary>
    /// The operation timed out. This is a final status.
    /// </summary>
    TimedOut = OperationPhase.Failed | 0x0800,

    #endregion

    #region Range 0x1xxx - 0xFxxx: The execution completed 

    /// <summary>
    /// The operation completed successfully. This is a final status.
    /// </summary>
    Completed = OperationPhase.Completed,
    
    /// <summary>
    /// The operation completed successfully, but only partially.
    /// Need to check the log for further information. This is a final status.
    /// </summary>
    CompletedPartially = OperationPhase.Completed | 0x2000,

    /// <summary>
    /// The operation completed successfully, but there were recorded some warnings.
    /// Need to check the log for further information. This is a final status.
    /// </summary>
    CompletedWithWarnings = OperationPhase.Completed | 0x4000,

    #endregion
}